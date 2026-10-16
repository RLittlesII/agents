#!/usr/bin/env python3
"""Install committed host projections through Skillfile or Microsoft apm."""

import argparse
import os
from pathlib import Path
import re
import shutil
import stat
import subprocess
import sys
import tempfile

if sys.version_info < (3, 11):
    raise SystemExit("The installer requires Python 3.11 or newer.")

import tomllib


def checked_path(root: Path, path: Path) -> Path:
    """Reject linked components, including dangling links and Windows junctions."""
    relative = path.relative_to(root)
    if ".." in relative.parts:
        raise ValueError(f"Path escapes installation boundary: {path}")
    current = root
    for part in (None, *relative.parts):
        if part is not None:
            current /= part
        try:
            info = current.lstat()
        except FileNotFoundError:
            continue
        if stat.S_ISLNK(info.st_mode) or getattr(info, "st_file_attributes", 0) & 1024:
            raise ValueError(f"Linked installation path is not allowed: {current}")
        if current != path and not stat.S_ISDIR(info.st_mode):
            raise ValueError(f"Expected a directory: {current}")
    return path


def read_tree(root: Path, directory: Path) -> dict[Path, bytes]:
    """Read a selected source tree without following links or special files."""
    checked_path(root, directory)
    if not directory.is_dir():
        raise ValueError(f"Missing artifact directory: {directory}")
    files = {}

    def visit(folder: Path) -> None:
        for path in sorted(folder.iterdir()):
            checked_path(root, path)
            if path.is_dir():
                visit(path)
            elif path.is_file():
                files[path.relative_to(directory)] = path.read_bytes()
            else:
                raise ValueError(f"Expected an ordinary artifact file: {path}")

    visit(directory)
    return files


def read_flat(root: Path, relative: str, suffix: str) -> dict[Path, bytes]:
    directory = checked_path(root, root / relative)
    if not directory.is_dir():
        raise ValueError(f"Missing artifact directory: {directory}")
    files = {}
    for path in sorted(directory.glob(f"*{suffix}")):
        checked_path(root, path)
        if not path.is_file():
            raise ValueError(f"Expected an ordinary artifact file: {path}")
        files[path.relative_to(root)] = path.read_bytes()
    if not files:
        raise ValueError(f"No {suffix} artifacts in {directory}")
    return files


def rebase_codex(relative: Path, content: bytes) -> bytes:
    """Adjust only the generated source reference, checking decoded TOML values."""
    text = content.decode("utf-8")
    before = tomllib.loads(text)
    role = relative.stem
    marker = f"The authoritative role source is agents/{role}.md."
    prefix = f"Read AGENTS.md first. {marker} Resolve its relative links against that path in the repository."
    instructions = before.get("developer_instructions")
    if before.get("name") != role or not isinstance(instructions, str) or not instructions.startswith(prefix):
        raise ValueError(f"Unexpected Codex source prefix: {relative}")
    replacement = f"The authoritative role source is .sdd/agents/{role}.md."
    lines = text.splitlines(keepends=True)
    matches = [i for i, line in enumerate(lines) if line.startswith("developer_instructions = ")]
    if len(matches) != 1 or marker not in lines[matches[0]]:
        raise ValueError(f"Unexpected Codex instruction encoding: {relative}")
    index = matches[0]
    lines[index] = lines[index].replace(marker, replacement, 1)
    result = "".join(lines)
    expected = before | {"developer_instructions": instructions.replace(marker, replacement, 1)}
    if tomllib.loads(result) != expected:
        raise ValueError(f"Codex source rebasing changed other values: {relative}")
    return result.encode("utf-8")


def build_plan(source: Path, host: str) -> tuple[dict[Path, bytes], dict[Path, bytes]]:
    skill_root = Path(".claude/skills" if host == "claude" else ".agents/skills")
    skills = read_tree(source, source / skill_root)
    names = {path.parts[0] for path in skills}
    if not names:
        raise ValueError(f"No skills in {skill_root}")
    for name in sorted(names):
        # These names enter both managers' manifests as unquoted relative tokens.
        if not re.fullmatch(r"[a-z0-9]+(?:-[a-z0-9]+)*", name) or Path(name, "SKILL.md") not in skills:
            raise ValueError(f"Expected a named skill directory containing SKILL.md: {name}")
    plan = {skill_root / path: data for path, data in skills.items()}
    if host == "claude":
        plan.update(read_flat(source, ".claude/agents", ".md"))
        plan.update(read_flat(source, ".claude/commands", ".md"))
    else:
        roles = read_flat(source, "agents", ".md")
        plan.update({Path(".sdd") / path: data for path, data in roles.items()})
        methods_root = checked_path(source, source / "skills")
        if not methods_root.is_dir():
            raise ValueError(f"Missing method skills: {methods_root}")
        for method in sorted(methods_root.iterdir()):
            if method.name.startswith("."):
                continue
            checked_path(source, method)
            if method.is_dir():
                files = read_tree(source, method)
                if Path("SKILL.md") not in files:
                    raise ValueError(f"Missing method SKILL.md: {method.name}")
                plan.update({Path(".sdd/skills") / method.name / path: data for path, data in files.items()})
        for relative, data in roles.items():
            # Role contracts use sibling Markdown and ../skills links; prose
            # about a consumer's README or companion remains consumer-owned.
            for link in re.findall(r"\]\((\.\./skills/[^)\s#]+|[^/)\s#]+\.md)(?:#[^)]*)?\)", data.decode("utf-8")):
                dependency = Path(os.path.normpath(str(Path(".sdd/agents") / link)))
                if dependency not in plan:
                    raise ValueError(f"Missing contract dependency in {relative}: {link}")
        for path, data in read_flat(source, ".codex/agents", ".toml").items():
            if Path("agents", f"{path.stem}.md") not in roles:
                raise ValueError(f"Missing authoritative source for {path}")
            plan[path] = rebase_codex(path, data)
    return dict(sorted(plan.items())), skills


def preflight(target: Path, plan: dict[Path, bytes], force: bool) -> dict[Path, bytes | None]:
    originals = {}
    conflicts = []
    for relative, content in plan.items():
        path = checked_path(target, target / relative)
        if path.exists() and not path.is_file():
            raise ValueError(f"Expected an ordinary destination file: {relative}")
        old = path.read_bytes() if path.exists() else None
        if old == content:
            continue
        if old is not None and not force:
            conflicts.append(relative.as_posix())
        originals[relative] = old
    if conflicts:
        raise ValueError("Conflicting files (review them, then use --force to replace):\n" + "\n".join(conflicts))
    return originals


def stage_skills(work: Path, skills: dict[Path, bytes], host: str, manager: str, executable: str) -> None:
    package = work / "package/skills"
    names = sorted({path.parts[0] for path in skills})
    for relative, content in skills.items():
        path = package / relative
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(content)
    if manager == "skillfile":
        platform = "claude-code" if host == "claude" else "codex"
        entries = [f"install {platform} local"]
        entries += [f"local skill {name} package/skills/{name}" for name in names]
        (work / "Skillfile").write_text("\n".join(entries) + "\n", encoding="utf-8")
        commands = [[executable, "validate"], [executable, "install"]]
    else:
        (work / "apm.yml").write_text("name: sdd-install\nversion: 1.0.0\n", encoding="utf-8")
        # Each skill is its own apm package: sibling links then remain outside
        # the package boundary and are preserved rather than rebased to staging.
        packages = [f"./package/skills/{name}" for name in names]
        commands = [[executable, "install", *packages, "--target", host, "--only", "apm"]]
    for command in commands:
        try:
            subprocess.run(command, cwd=work, check=True, shell=False, capture_output=True, text=True, encoding="utf-8", errors="replace")
        except subprocess.CalledProcessError as error:
            detail = (error.stderr or error.stdout or "").strip()
            raise RuntimeError(f"{manager} {command[1]} failed (exit {error.returncode}). {detail}") from error
    if host == "claude":
        roots = [work / ".claude/skills"]
    else:
        roots = [work / ".agents/skills"]
        if manager == "skillfile":
            roots.append(work / ".codex/skills")
    found = False
    for root in roots:
        checked_path(work, root)
        if not root.exists():
            continue
        # Verify every known candidate: an ambiguous stale/altered tree is a failure.
        for relative, expected in skills.items():
            path = checked_path(work, root / relative)
            if not path.is_file() or path.read_bytes() != expected:
                raise ValueError(f"Unverified {manager} output: {path.relative_to(work)}")
        found = True
    if not found:
        raise ValueError(f"{manager} did not install the expected skills")


def publish(target: Path, plan: dict[Path, bytes], originals: dict[Path, bytes | None]) -> None:
    created = []
    pending = []
    replaced = []

    def prepare(path: Path, content: bytes) -> Path:
        checked_path(target, path)
        current = target
        for part in path.parent.relative_to(target).parts:
            current /= part
            if not current.exists():
                current.mkdir()
                created.append(current)
        descriptor, name = tempfile.mkstemp(prefix=".sdd-install-", dir=path.parent)
        temporary = Path(name)
        pending.append(temporary)
        with os.fdopen(descriptor, "wb") as output:
            output.write(content)
        return temporary

    try:
        replacements = [(relative, prepare(target / relative, plan[relative])) for relative in originals]
        for relative, temporary in replacements:
            checked_path(target, target / relative)
            os.replace(temporary, target / relative)
            replaced.append(relative)
    except Exception as error:
        rollback_errors = []
        for relative in reversed(replaced):
            try:
                path = checked_path(target, target / relative)
                if originals[relative] is None:
                    path.unlink()
                else:
                    os.replace(prepare(path, originals[relative]), path)
            except (OSError, ValueError) as rollback:
                rollback_errors.append(f"{relative}: {rollback}")
        raise RuntimeError(f"Publication failed: {error}" + (f"; rollback failed: {', '.join(rollback_errors)}" if rollback_errors else "")) from error
    finally:
        for temporary in pending:
            temporary.unlink(missing_ok=True)
        for directory in reversed(created):
            try:
                directory.rmdir()
            except OSError:
                pass  # Successful files or unrelated files keep their parent directories.


def install(source: Path, target: Path, host: str, manager: str, force: bool = False) -> int:
    """Install one host; raise on any failure, returning zero only on success."""
    if host not in ("claude", "codex") or manager not in ("skillfile", "apm"):
        raise ValueError("Choose --host claude|codex and --manager skillfile|apm")
    source = Path(source).expanduser().resolve(strict=True)
    target = Path(target).expanduser().resolve(strict=True)
    if not source.is_dir() or not target.is_dir():
        raise ValueError("Source and target must be existing directories")
    if source == target:
        raise ValueError("The consumer project must differ from the source checkout")
    plan, skills = build_plan(source, host)
    preflight(target, plan, force)
    executable = shutil.which(manager)
    if executable is None:
        raise RuntimeError(f"{manager} was not found on PATH; install it before running this script")
    with tempfile.TemporaryDirectory(prefix="sdd-install-") as folder:
        stage_skills(Path(folder).resolve(), skills, host, manager, executable)
        originals = preflight(target, plan, force)
        publish(target, plan, originals)
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--host", required=True, choices=("claude", "codex"))
    parser.add_argument("--manager", required=True, choices=("skillfile", "apm"))
    parser.add_argument("--target", required=True, type=Path, help="Existing consumer project directory")
    parser.add_argument("--force", action="store_true", help="Replace conflicting planned files after review")
    options = parser.parse_args(argv)
    try:
        result = install(Path(__file__).resolve().parent.parent, options.target, options.host, options.manager, options.force)
    except (ValueError, OSError, RuntimeError, subprocess.SubprocessError) as error:
        print(f"Installation failed: {error}", file=sys.stderr)
        return 1
    print(f"Installed {options.host} through {options.manager} into {options.target}")
    return result


if __name__ == "__main__":
    raise SystemExit(main())
