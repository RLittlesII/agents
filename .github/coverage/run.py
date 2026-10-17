"""Fresh source-bound changed-line coverage, shared by local verification and CI."""

import argparse
import fnmatch
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET


PROCESSES = {"python-installer-tests", "python-gate-tests", "python-combine", "python-report", "fsharp-unit", "fsharp-cli"}
COLLECTORS = {"python", "fsharp-unit", "fsharp-cli"}


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def git(repo, *args):
    nul_paths = "-z" in args
    result = subprocess.run(["git", "--literal-pathspecs", "-C", str(repo), *args], capture_output=True, text=not nul_paths)
    if result.returncode:
        raise ValueError(f"Git {' '.join(args)} failed: {result.stderr.strip()}")
    return os.fsdecode(result.stdout) if nul_paths else result.stdout.strip()


def matches(path, patterns):
    return any(fnmatch.fnmatchcase(path, pattern) or (pattern.startswith("**/") and fnmatch.fnmatchcase(path, pattern[3:])) for pattern in patterns)


def inventory(repo: Path, policy: dict) -> dict[str, str]:
    paths = git(repo, "ls-files", "-z", "--cached", "--others", "--exclude-standard").split("\0")
    result = {}
    for name in sorted(set(paths)):
        if not name or not matches(name, policy["include"]) or matches(name, policy["exclude"]):
            continue
        source = repo / name
        if not source.exists():
            continue
        if source.is_symlink() or not source.resolve().is_relative_to(repo.resolve()):
            raise ValueError(f"Source escapes checkout: {name}")
        result[name] = digest(source)
    return result


def changed_lines(repo: Path, base_sha: str, inventory: dict[str, str]) -> dict[str, set[int]]:
    changed = {}
    tracked = set(git(repo, "ls-files", "-z").split("\0"))
    for name in inventory:
        raw = (repo / name).read_bytes()
        if b"\0" in raw:
            raise ValueError(f"Binary production source: {name}")
        try:
            count = len(raw.decode("utf-8").splitlines())
        except UnicodeDecodeError as error:
            raise ValueError(f"Non-UTF8 production source: {name}") from error
        if name not in tracked:
            changed[name] = set(range(1, count + 1))
            continue
        diff = git(repo, "diff", "--no-ext-diff", "--no-renames", "--unified=0", base_sha, "--", name)
        lines = set()
        if any(line.startswith("Binary files ") or line == "GIT binary patch" for line in diff.splitlines()):
            raise ValueError(f"Binary source diff: {name}")
        for line in diff.splitlines():
            if not line.startswith("@@"):
                continue
            match = re.fullmatch(r"@@ -\d+(?:,\d+)? \+(\d+)(?:,(\d+))? @@.*", line)
            if not match:
                raise ValueError(f"Malformed diff hunk: {name}")
            start, length = int(match[1]), int(match[2] or 1)
            if length and (start < 1 or start + length - 1 > count):
                raise ValueError(f"Out-of-bounds diff hunk: {name}")
            lines.update(range(start, start + length))
        if lines:
            changed[name] = lines
    return changed


def collect(repo: Path, snapshot: dict, output_dir: Path) -> dict:
    if output_dir.resolve() != snapshot["run_dir"].resolve():
        raise ValueError("Collection directory differs from trusted run directory")
    # MSBuild Exec uses a shell: only these portable path characters are supported.
    path_pattern = r"[A-Za-z0-9_./ :+@\\-]+" if os.name == "nt" else r"[A-Za-z0-9_./ :+@-]+"
    for path in (repo, output_dir, Path(sys.executable)):
        if not re.fullmatch(path_pattern, str(path)):
            raise ValueError("Coverage build paths support ASCII letters, digits, spaces and _./ :+@- only (native backslashes on Windows)")
    coverage_version = subprocess.run([sys.executable, "-m", "coverage", "--version"], capture_output=True, text=True, check=True).stdout
    required = (repo / ".github/coverage/requirements.txt").read_text().strip().split("==")[1]
    coverlet = ET.parse(repo / ".github/coverage/fsharp/Coverage.props").find(".//PackageReference[@Include='coverlet.msbuild']").attrib["Version"]
    if f"version {required} " not in coverage_version:
        raise ValueError(f"coverage.py version must be {required}; install .github/coverage/requirements.txt")
    sdk = subprocess.run(["dotnet", "--version"], cwd=repo, capture_output=True, text=True, check=True).stdout.strip()
    snapshot["tools"] = {"python": required, "coverlet": coverlet, "dotnet": sdk}
    processes = []
    environment = os.environ | {"COVERAGE_FILE": str(output_dir / ".coverage")}

    def run(name, command):
        with (output_dir / (name + ".log")).open("w") as log:
            result = subprocess.run(command, cwd=repo, env=environment, stdout=log, stderr=subprocess.STDOUT)
        processes.append({"name": name, "exit_code": result.returncode})
        if result.returncode:
            raise ValueError(f"{name} failed ({result.returncode}); inspect .artifacts/coverage/{output_dir.name}/{name}.log")

    roots = sorted({str(Path(name).parent) for name in snapshot["sources"] if name.endswith(".py")})
    source_arg = "--source=" + ",".join(roots)
    omit_arg = "--omit=" + ",".join(snapshot["policy"]["exclude"])
    packages = ET.Element("Project")
    items = ET.SubElement(packages, "ItemGroup")
    declarations = {}
    # Derive generated references from authoritative production script declarations.
    for name in snapshot["sources"]:
        if name.endswith(".fsx"):
            for package, version in re.findall(r'^#r "nuget:\s*([^,\"]+),\s*([^\"]+)"', (repo / name).read_text(), re.MULTILINE):
                if package in declarations and declarations[package] != version:
                    raise ValueError(f"Conflicting runtime dependency: {package}")
                declarations[package] = version
    for package, version in sorted(declarations.items()):
        ET.SubElement(items, "PackageReference", Include=package, Version=version)
    ET.ElementTree(packages).write(output_dir / "RuntimePackages.props", encoding="unicode")
    for kind in ("unit", "cli"):
        folder = output_dir / kind
        folder.mkdir()
        project = ET.Element("Project", Sdk="Microsoft.NET.Sdk")
        ET.SubElement(project, "Import", Project=str(repo / ".github/coverage/fsharp/Coverage.props"))
        ET.SubElement(project, "Import", Project=str(output_dir / "RuntimePackages.props"))
        properties = ET.SubElement(project, "PropertyGroup")
        ET.SubElement(properties, "CoverletOutput").text = str(output_dir / ("fsharp-" + kind + ".xml"))
        ET.SubElement(properties, "ExcludeByFile").text = "**/project.tests.fsx,**/FsiArguments.fs"
        ET.SubElement(properties, "CoverageKind").text = kind
        ET.SubElement(properties, "PythonExecutable").text = sys.executable
        ET.SubElement(properties, "CliDriver").text = str(repo / ".github/coverage/test_cli_driver.py")
        ET.SubElement(properties, "RepositoryRoot").text = str(repo)
        files = ET.SubElement(project, "ItemGroup")
        links = [repo / "scripts/Projection.fsx"]
        if kind == "cli":
            links.append(repo / ".github/coverage/fsharp/FsiArguments.fs")
        links.append(repo / ("scripts/project.tests.fsx" if kind == "unit" else "scripts/project.fsx"))
        for source in links:
            ET.SubElement(files, "Compile", Include=str(source))
        project_path = folder / "Coverage.fsproj"
        ET.ElementTree(project).write(project_path, encoding="unicode")
        run("fsharp-" + kind, ["dotnet", "msbuild", str(project_path), "-restore", "-t:RunCovered", "-nologo"])
    # Restore real collector dependencies before the synthetic offline smoke suite.
    for name, directory in (("python-installer-tests", "install"), ("python-gate-tests", ".github/coverage")):
        run(name, [sys.executable, "-m", "coverage", "run", "--parallel-mode", source_arg, omit_arg, "-m", "unittest", "discover", "-s", directory, "-p", "test_*.py", "-v"])
    run("python-combine", [sys.executable, "-m", "coverage", "combine"])
    run("python-report", [sys.executable, "-m", "coverage", "xml", "-o", str(output_dir / "python.xml")])
    if inventory(repo, snapshot["policy"]) != snapshot["sources"] or git(repo, "rev-parse", "HEAD") != snapshot["head_sha"]:
        raise ValueError("Source inventory/bytes or HEAD changed during collection")
    manifest = {key: snapshot[key] for key in ("requested_base_sha", "comparison_base_sha", "head_sha", "policy_sha256", "sources", "tools")}
    manifest.update(schema_version=1, process_results=processes, reports=[{"path": name + ".xml", "sha256": digest(output_dir / (name + ".xml")), "collector": name} for name in sorted(COLLECTORS)])
    temporary = output_dir / "manifest.tmp"
    temporary.write_text(json.dumps(manifest, indent=2) + "\n")
    temporary.replace(output_dir / "manifest.json")
    return manifest


def read_reports(repo: Path, snapshot: dict, manifest: dict) -> dict[str, dict[int, bool]]:
    if type(manifest.get("schema_version")) is not int or manifest["schema_version"] != 1:
        raise ValueError("Unsupported manifest schema")
    for key in ("requested_base_sha", "comparison_base_sha", "head_sha", "policy_sha256", "sources", "tools"):
        if manifest.get(key) != snapshot[key]:
            raise ValueError(f"Stale or mismatched manifest: {key}")
    if inventory(repo, snapshot["policy"]) != snapshot["sources"] or git(repo, "rev-parse", "HEAD") != snapshot["head_sha"]:
        raise ValueError("Source inventory or HEAD changed after collection")
    for name, expected in snapshot["sources"].items():
        if digest(repo / name) != expected:
            raise ValueError(f"Source bytes changed: {name}")
    processes = manifest.get("process_results", [])
    if len(processes) != len(PROCESSES) or {item.get("name") for item in processes} != PROCESSES or any(type(item.get("exit_code")) is not int or item["exit_code"] != 0 for item in processes):
        raise ValueError("Missing, duplicated or failed required test/collector process")
    reports = manifest.get("reports", [])
    if len(reports) != len(COLLECTORS) or {item.get("collector") for item in reports} != COLLECTORS or len({item.get("path") for item in reports}) != len(reports):
        raise ValueError("Missing or duplicate required coverage reports")
    hits = {}
    root = repo.resolve()
    for report in reports:
        relative = Path(report["path"])
        path = snapshot["run_dir"] / relative
        if relative.is_absolute() or ".." in relative.parts or not path.resolve().is_relative_to(snapshot["run_dir"].resolve()):
            raise ValueError("Report path escapes run directory")
        try:
            if digest(path) != report["sha256"]:
                raise ValueError(f"Report digest mismatch: {relative}")
            xml = ET.parse(path).getroot()
        except (OSError, ET.ParseError) as error:
            raise ValueError(f"Unreadable coverage report: {relative}") from error
        if xml.tag != "coverage":
            raise ValueError(f"Invalid Cobertura root: {relative}")
        source_roots = [Path(element.text) for element in xml.findall("./sources/source") if element.text]
        for entry in xml.findall("./packages/package/classes/class"):
            filename = Path(entry.attrib.get("filename", ""))
            if not str(filename) or ".." in filename.parts:
                raise ValueError("Invalid coverage source path")
            candidates = [filename] if filename.is_absolute() else [root / filename] + [base / filename if base.is_absolute() else root / base / filename for base in source_roots]
            mapped = {candidate.resolve().relative_to(root).as_posix() for candidate in candidates if candidate.resolve().is_relative_to(root)}
            valid = mapped & snapshot["sources"].keys()
            if len(valid) != 1:
                if not valid and any(matches(name, snapshot["policy"]["exclude"]) for name in mapped):
                    continue
                raise ValueError(f"Unmapped or ambiguous coverage source: {filename}")
            name = valid.pop()
            lines = hits.setdefault(name, {})
            bound = len((repo / name).read_text().splitlines())
            for line in entry.findall("./lines/line"):
                number, value = line.attrib.get("number", ""), line.attrib.get("hits", "")
                if not re.fullmatch(r"[0-9]+", number) or not re.fullmatch(r"[0-9]+", value) or not 1 <= int(number) <= bound:
                    raise ValueError(f"Invalid executable line in {name}")
                lines[int(number)] = lines.get(int(number), False) or int(value) > 0
    if hits.keys() != snapshot["sources"].keys():
        raise ValueError("Missing source evidence: " + ", ".join(sorted(snapshot["sources"].keys() - hits.keys())))
    if any(not lines for lines in hits.values()):
        raise ValueError("Empty executable source evidence: " + ", ".join(sorted(name for name, lines in hits.items() if not lines)))
    return hits


def evaluate(changed: dict[str, set[int]], executable_hits: dict[str, dict[int, bool]], threshold: int) -> dict:
    missed = []
    total = covered = 0
    for name, lines in changed.items():
        for line in lines & executable_hits.get(name, {}).keys():
            total += 1
            if executable_hits[name][line]:
                covered += 1
            else:
                missed.append((name, line))
    verdict = "not-applicable" if not total else "pass" if 100 * covered >= threshold * total else "fail"
    return {"verdict": verdict, "covered": covered, "changed": total, "missed": sorted(missed)}


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--base", required=True)
    args = parser.parse_args(argv)
    repo = Path(__file__).resolve().parents[2]
    try:
        policy_path = repo / ".github/coverage/policy.json"
        policy = json.loads(policy_path.read_text())
        if policy.get("schema_version") != 1 or type(policy.get("threshold_percent")) is not int or policy["threshold_percent"] != 80 or any(not isinstance(policy.get(key), list) or not all(isinstance(value, str) for value in policy[key]) for key in ("include", "exclude")):
            raise ValueError("Invalid coverage policy schema or threshold")
        requested = git(repo, "rev-parse", "--verify", args.base + "^{commit}")
        head = git(repo, "rev-parse", "HEAD")
        bases = git(repo, "merge-base", "--all", requested, head).splitlines()
        if len(bases) != 1:
            raise ValueError("Comparison requires exactly one merge base")
        run_dir = repo / ".artifacts/coverage" / uuid.uuid4().hex
        run_dir.mkdir(parents=True)
        snapshot = {"requested_base_sha": requested, "comparison_base_sha": bases[0], "head_sha": head, "policy_sha256": digest(policy_path), "sources": inventory(repo, policy), "policy": policy, "tools": {}, "run_dir": run_dir}
        manifest = collect(repo, snapshot, run_dir)
        evidence = read_reports(repo, snapshot, manifest)
        if digest(policy_path) != snapshot["policy_sha256"]:
            raise ValueError("Policy changed during collection")
        result = evaluate(changed_lines(repo, bases[0], snapshot["sources"]), evidence, policy["threshold_percent"])
        print(f"Comparison {bases[0]} (requested {requested})")
        if result["verdict"] == "not-applicable":
            print("N/A: 0 changed executable production lines (covered 0 / changed 0)")
        else:
            print(f"{result['verdict'].upper()}: {result['covered']} / {result['changed']} changed executable production lines; required {policy['threshold_percent']}%")
        for name, line in result["missed"]:
            print(f"Missed {name}:{line}")
        return int(result["verdict"] == "fail")
    except (ValueError, OSError, subprocess.SubprocessError, KeyError, TypeError) as error:
        print(f"Coverage gate failed: {error}. Reproduce: python3 .github/coverage/run.py --base {args.base}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
