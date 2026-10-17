"""Offline executable coverage for 0038-F1. Written by Kevin."""

import contextlib
import importlib.util
import io
import json
import os
from pathlib import Path
import re
import shutil
import stat
import subprocess
import tempfile
import tomllib
import unittest
from unittest.mock import patch

_SPEC = importlib.util.spec_from_file_location("host_installer", Path(__file__).with_name("install.py"))
installer = importlib.util.module_from_spec(_SPEC)
_SPEC.loader.exec_module(installer)

PREFIX = "Read AGENTS.md first. The authoritative role source is agents/alpha.md. Resolve its relative links against that path in the repository."
INSTRUCTIONS = PREFIX + "\n\nPersona: synthetic.\n[peer](beta.md) [method](../skills/method/SKILL.md)"
ROLE = b"# Synthetic alpha\n[peer](beta.md) [method](../skills/method/SKILL.md)\n"
SKILL = b"---\nname: method\ndescription: Synthetic method\n---\n# Method\n[Self](../method/SKILL.md)\n"
BINARY = bytes([0, 255, 10, 13, 128])


def write(root, relative, content):
    path = root / relative
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(content)


def snapshot(root):
    return {str(path.relative_to(root)): path.read_bytes() for path in root.rglob("*") if path.is_file() and not path.is_symlink()}


class InstallerTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.source = self.root / "checkout with 'quotes' $ and é"
        self.target = self.root / "project with 'quotes' $ and é"
        self.target.mkdir()
        for host in (".claude", ".agents"):
            write(self.source, f"{host}/skills/method/SKILL.md", SKILL)
            write(self.source, f"{host}/skills/method/data/nested.bin", BINARY)
        write(self.source, ".agents/skills/command/SKILL.md", b"# Command\n")
        write(self.source, ".claude/agents/alpha.md", ROLE)
        write(self.source, ".claude/agents/beta.md", b"# Beta\n")
        write(self.source, ".claude/commands/command.md", b"# Command\n[role](../agents/alpha.md)\n")
        for role in ("alpha", "beta"):
            instructions = INSTRUCTIONS.replace("agents/alpha.md.", f"agents/{role}.md.")
            toml = f'name = "{role}"\nmodel = "synthetic-model"\nmodel_reasoning_effort = "high"\ndeveloper_instructions = {json.dumps(instructions)}\n'
            write(self.source, f".codex/agents/{role}.toml", toml.encode())
        write(self.source, "agents/alpha.md", ROLE)
        write(self.source, "agents/beta.md", b"# Beta\n")
        write(self.source, "skills/method/SKILL.md", SKILL)
        write(self.source, "skills/method/data/nested.bin", BINARY)
        for relative in ("AGENTS.md", "CLAUDE.md", "Skillfile", "apm.yml", ".codex/config.toml", ".claude/agents/custom.md", "agents/consumer.md", "skills/custom/SKILL.md"):
            write(self.target, relative, b"consumer-owned\n")
        self.original = snapshot(self.target)
        self.source_original = snapshot(self.source)
        self.calls = []
        self.manager = "skillfile"
        self.host = "claude"
        self.output_mode = "normal"

    def fake_manager(self, args, **kwargs):
        """Parse the real local-manifest grammar and materialize its outputs."""
        self.calls.append((args, kwargs))
        self.assertIsInstance(args, list)
        self.assertEqual(Path(args[0]).name, self.manager)
        self.assertFalse(kwargs.get("shell", False))
        self.assertTrue(kwargs.get("check"))
        cwd = Path(kwargs["cwd"])
        self.assertNotEqual(cwd.resolve(), self.target.resolve())
        self.assertFalse(cwd.resolve().is_relative_to(self.source.resolve()))
        if self.manager == "skillfile":
            self.assertEqual(args[1:], ["validate"] if len(self.calls) % 2 else ["install"])
            lines = (cwd / "Skillfile").read_text().splitlines()
            declarations = [line.split() for line in lines if line.startswith("local ")]
            self.assertTrue(declarations)
            for tokens in declarations:
                self.assertEqual(tokens[:2], ["local", "skill"])
                self.assertEqual(len(tokens), 4)
                self.assertEqual(tokens[3], f"package/skills/{tokens[2]}")
            expected_host = "claude-code" if self.host == "claude" else "codex"
            self.assertIn(f"install {expected_host} local", lines)
            if args[1:] == ["validate"]:
                return subprocess.CompletedProcess(args, 0)
            inputs = cwd / "package/skills"
            output = cwd / (".claude/skills" if self.host == "claude" else ".codex/skills")
        else:
            inputs = cwd / "package/skills"
            projected_skills = self.source / (".claude/skills" if self.host == "claude" else ".agents/skills")
            paths = [f"./package/skills/{path.name}" for path in sorted(projected_skills.iterdir())]
            self.assertEqual(args[1:], ["install", *paths, "--target", self.host, "--only", "apm"])
            fields = dict(line.split(":", 1) for line in (cwd / "apm.yml").read_text().splitlines() if line.strip())
            self.assertEqual(fields, {"name": " sdd-install", "version": " 1.0.0"})
            output = cwd / (".claude/skills" if self.host == "claude" else ".agents/skills")
        shutil.copytree(inputs, output, dirs_exist_ok=True)
        write(cwd, "AGENTS.md", b"manager-generated instructions")
        if self.output_mode == "changed":
            (output / "method/data/nested.bin").write_bytes(b"changed")
        elif self.output_mode == "missing":
            (output / "method/data/nested.bin").unlink()
        elif self.output_mode == "linked":
            linked = output / "method/data/nested.bin"
            linked.unlink()
            linked.symlink_to(inputs / "method/data/nested.bin")
        elif self.output_mode == "ambiguous":
            alternate = cwd / ".agents/skills"
            shutil.copytree(inputs, alternate)
            (alternate / "method/SKILL.md").write_bytes(b"different")
        return subprocess.CompletedProcess(args, 0)

    def run_install(self, **overrides):
        options = dict(source=self.source, target=self.target, host=self.host, manager=self.manager)
        options.update(overrides)
        with patch("shutil.which", return_value=f"/synthetic tools/{self.manager}"), patch("subprocess.run", side_effect=self.fake_manager):
            return installer.install(**options)

    def assert_rejected(self, **overrides):
        with self.assertRaises((ValueError, OSError, RuntimeError, subprocess.SubprocessError)) as caught:
            self.run_install(**overrides)
        self.assertNotIsInstance(caught.exception, NotImplementedError)
        self.assertEqual(snapshot(self.target), self.original)

    def assert_complete(self):
        skill_root = ".claude" if self.host == "claude" else ".agents"
        self.assertEqual((self.target / skill_root / "skills/method/SKILL.md").read_bytes(), SKILL)
        self.assertEqual((self.target / skill_root / "skills/method/data/nested.bin").read_bytes(), BINARY)
        for relative, content in self.original.items():
            self.assertEqual((self.target / relative).read_bytes(), content)
        self.assertEqual(snapshot(self.source), self.source_original)
        if self.host == "claude":
            for relative, content in self.source_original.items():
                if relative.startswith(".claude/"):
                    self.assertEqual((self.target / relative).read_bytes(), content)
            self.assertEqual((self.target / ".claude/agents/alpha.md").read_bytes(), ROLE)
            self.assertEqual((self.target / ".claude/commands/command.md").read_bytes(), (self.source / ".claude/commands/command.md").read_bytes())
            self.assertFalse((self.target / ".agents").exists())
            self.assertFalse((self.target / ".codex/agents").exists())
        else:
            for relative, content in self.source_original.items():
                if relative.startswith(".agents/"):
                    self.assertEqual((self.target / relative).read_bytes(), content)
                elif relative.startswith(("agents/", "skills/")):
                    self.assertEqual((self.target / ".sdd" / relative).read_bytes(), content)
                elif relative.startswith(".codex/agents/"):
                    self.assertTrue((self.target / relative).is_file())
            self.assertTrue((self.target / ".agents/skills/command/SKILL.md").is_file())
            self.assertFalse((self.target / ".claude/skills").exists())
            self.assertFalse((self.target / ".claude/commands").exists())
        self.assertTrue(self.calls, "The selected manager must actually run")

    def test_B001_B003_claude_skillfile_complete_installation(self):
        # Given a checkout and consumer; When installing; Then preserve complete bytes.
        self.assertEqual(self.run_install(), 0)
        self.assert_complete()
        self.assertEqual(len(self.calls), 2)

    def test_B002_B003_codex_skillfile_complete_installation(self):
        self.host = "codex"
        self.assertEqual(self.run_install(), 0)
        self.assert_complete()

    def test_B001_B004_claude_apm_complete_installation(self):
        self.manager = "apm"
        self.assertEqual(self.run_install(), 0)
        self.assert_complete()
        self.assertEqual(len(self.calls), 1)

    def test_B002_B004_codex_apm_complete_installation(self):
        self.host, self.manager = "codex", "apm"
        self.assertEqual(self.run_install(), 0)
        self.assert_complete()

    def test_B006_codex_rebases_only_source_prefix_and_supplies_dependencies(self):
        # Given source contracts; When installing; Then decoded instructions and links survive.
        self.host = "codex"
        self.assertEqual(self.run_install(), 0)
        before = tomllib.loads((self.source / ".codex/agents/alpha.toml").read_text())
        after = tomllib.loads((self.target / ".codex/agents/alpha.toml").read_text())
        before["developer_instructions"] = INSTRUCTIONS.replace("source is agents/alpha.md.", "source is .sdd/agents/alpha.md.", 1)
        self.assertEqual(after, before)
        self.assertEqual((self.target / ".sdd/agents/alpha.md").read_bytes(), ROLE)
        self.assertTrue((self.target / ".sdd/agents/beta.md").is_file())
        self.assertEqual((self.target / ".sdd/skills/method/SKILL.md").read_bytes(), SKILL)
        self.assertEqual((self.target / ".sdd/skills/method/data/nested.bin").read_bytes(), BINARY)

    def test_B006_unexpected_codex_source_prefix_is_rejected(self):
        self.host = "codex"
        path = self.source / ".codex/agents/alpha.toml"
        path.write_text(path.read_text().replace("source is agents/alpha.md.", "source is elsewhere/alpha.md."))
        self.assert_rejected()
        self.assertFalse(self.calls)

    def test_B006_missing_authoritative_role_is_rejected(self):
        # Given a projected role without its source; When installing; Then fail before effects.
        self.host = "codex"
        (self.source / "agents/alpha.md").unlink()
        self.assert_rejected()
        self.assertFalse(self.calls)

    def test_B006_missing_referenced_private_method_is_rejected(self):
        # Given a role referring to an absent method; When installing; Then reject its broken closure.
        self.host = "codex"
        shutil.rmtree(self.source / "skills/method")
        write(self.source, "skills/another/SKILL.md", b"# Another method\n")
        self.assert_rejected()
        self.assertFalse(self.calls)

    def test_B007_consumer_index_uses_selected_runtime_inventory_with_both_managers(self):
        # Given producer-only docs; When installed; Then index links runtime artifacts only.
        self.host = "codex"
        write(self.source, "agents/README.md", b"# Maintainer index\n[Spec](.spec/README.md)\n")
        write(self.source, "agents/producer-notes.md", b"# Producer-only notes\n")
        before = snapshot(self.source)
        for manager in ("skillfile", "apm"):
            with self.subTest(manager=manager):
                self.manager = manager
                self.calls.clear()
                self.assertEqual(self.run_install(), 0)
                index = self.target / ".sdd/agents/README.md"
                links = re.findall(r"\]\(([^)]+)\)", index.read_text())
                self.assertEqual(set(links), {"alpha.md", "beta.md", "../skills/method/SKILL.md"})
                for link in links:
                    self.assertTrue((index.parent / link).is_file(), link)
                self.assertNotIn("Maintainer index", index.read_text())
                self.assertFalse((self.target / ".sdd/agents/producer-notes.md").exists())
                self.assertEqual((self.target / ".sdd/agents/alpha.md").read_bytes(), ROLE)
                self.assertEqual(snapshot(self.source), before)
                for relative, content in self.original.items():
                    self.assertEqual((self.target / relative).read_bytes(), content)

    def test_B007_private_specs_are_excluded_at_every_depth_and_runtime_bytes_survive(self):
        # Given mixed metadata/resources; When installed; Then only reserved metadata is omitted.
        self.host = "codex"
        for path in ("skills/.spec/README.md", "skills/method/.spec/README.md", "skills/method/references/.spec/README.md"):
            write(self.source, path, b"# Producer specification\n")
        guide = b"# Runtime guide\n[Method](../SKILL.md)\n"
        write(self.source, "skills/method/references/guide.md", guide)
        write(self.source, "skills/method/.runtime/payload.bin", BINARY)
        before = snapshot(self.source)
        self.assertEqual(self.run_install(), 0)
        private = self.target / ".sdd"
        self.assertFalse(any(".spec" in path.parts for path in private.rglob("*")))
        self.assertEqual((private / "skills/method/references/guide.md").read_bytes(), guide)
        self.assertEqual((private / "skills/method/.runtime/payload.bin").read_bytes(), BINARY)
        self.assertEqual((private / "skills/method/data/nested.bin").read_bytes(), BINARY)
        self.assertEqual(snapshot(self.source), before)

    def test_B007_native_role_cannot_overwrite_the_consumer_index(self):
        # Given a native contract using the reserved index path; Then reject before effects.
        self.host = "codex"
        write(self.source, "agents/README.md", b"# Conflicting role\n")
        instructions = INSTRUCTIONS.replace("agents/alpha.md.", "agents/README.md.")
        write(self.source, ".codex/agents/README.toml", f'name = "README"\ndeveloper_instructions = {json.dumps(instructions)}\n'.encode())
        self.assert_rejected()
        self.assertFalse(self.calls)

    def test_B007_checkout_runtime_bundle_is_closed_over_private_file_links(self):
        # Given this pool; When installed offline; Then its private runtime links resolve.
        self.host = "codex"
        self.source = Path(__file__).resolve().parent.parent
        self.assertEqual(self.run_install(), 0)
        private = self.target / ".sdd"
        self.assertFalse(any(".spec" in path.parts for path in private.rglob("*")))
        for document in private.rglob("*.md"):
            for link in re.findall(r"\]\(([^)\s#]+)(?:#[^)]*)?\)", document.read_text()):
                if ":" not in link:
                    destination = (document.parent / link).resolve()
                    self.assertTrue(destination.is_relative_to(private.resolve()), f"{document}: {link}")
                    self.assertTrue(destination.is_file(), f"{document}: {link}")

    def test_C5_missing_manager_leaves_consumer_unchanged(self):
        with patch("shutil.which", return_value=None), self.assertRaises((ValueError, OSError, RuntimeError)) as caught:
            installer.install(self.source, self.target, "claude", "skillfile")
        self.assertNotIsInstance(caught.exception, NotImplementedError)
        self.assertEqual(snapshot(self.target), self.original)

    def test_C5_failed_manager_leaves_consumer_unchanged(self):
        for manager, failed_command in (("skillfile", "validate"), ("skillfile", "install"), ("apm", "install")):
            with self.subTest(manager=manager, command=failed_command):
                self.manager = manager
                def fail(args, **kwargs):
                    if args[1] == failed_command:
                        raise subprocess.CalledProcessError(2, args)
                    return self.fake_manager(args, **kwargs)
                with patch("shutil.which", return_value=manager), patch("subprocess.run", side_effect=fail), self.assertRaises((RuntimeError, subprocess.SubprocessError)) as caught:
                    installer.install(self.source, self.target, "claude", manager)
                self.assertNotIsInstance(caught.exception, NotImplementedError)
                self.assertEqual(snapshot(self.target), self.original)
                self.calls.clear()

    def test_C5_unverified_manager_output_leaves_consumer_unchanged(self):
        for mode in ("changed", "missing", "ambiguous"):
            with self.subTest(mode=mode):
                self.host = "codex"
                self.output_mode = mode
                self.assert_rejected()
                self.calls.clear()

    def test_C3_all_conflicts_are_reported_before_manager_or_writes(self):
        write(self.target, ".claude/agents/alpha.md", b"local role")
        write(self.target, ".claude/skills/method/SKILL.md", b"local skill")
        self.original = snapshot(self.target)
        with self.assertRaises((ValueError, RuntimeError)) as caught:
            self.run_install()
        self.assertIn(".claude/agents/alpha.md", str(caught.exception))
        self.assertIn(".claude/skills/method/SKILL.md", str(caught.exception))
        self.assertEqual(snapshot(self.target), self.original)
        self.assertFalse(self.calls)

    def test_C3_identical_rerun_is_successful(self):
        self.assertEqual(self.run_install(), 0)
        installed = snapshot(self.target)
        self.assertEqual(self.run_install(), 0)
        self.assertEqual(snapshot(self.target), installed)

    def test_C3_force_replaces_only_planned_files(self):
        write(self.target, ".claude/agents/alpha.md", b"local role")
        self.assertEqual(self.run_install(force=True), 0)
        self.assertEqual((self.target / ".claude/agents/alpha.md").read_bytes(), ROLE)
        self.assertEqual((self.target / ".claude/agents/custom.md").read_bytes(), b"consumer-owned\n")

    def test_C3_publication_failure_restores_replacements_and_removes_additions(self):
        # Given mixed additions/replacements; When a later replacement fails; Then rollback.
        write(self.target, ".claude/agents/alpha.md", b"local role")
        self.original = snapshot(self.target)
        real_replace = os.replace
        completed = []
        def fail_later(source, destination):
            if len(completed) == 2:
                completed.append("injected failure")
                raise OSError("synthetic publication failure")
            result = real_replace(source, destination)
            completed.append(str(destination))
            return result
        with patch("os.replace", side_effect=fail_later):
            self.assert_rejected(force=True)
        self.assertGreaterEqual(len(completed), 3)
        self.assertFalse((self.target / ".claude/skills").exists())

    def test_C4_structural_destination_errors_reject_even_force(self):
        for relative, directory in ((".claude/skills", False), (".claude/agents/alpha.md", True)):
            with self.subTest(relative=relative):
                path = self.target / relative
                if directory:
                    path.mkdir(parents=True)
                else:
                    write(self.target, relative, b"file instead of parent")
                self.original = snapshot(self.target)
                self.assert_rejected(force=True)
                if directory:
                    path.rmdir()
                else:
                    path.unlink()
                self.calls.clear()

    def test_C4_source_links_including_dangling_are_rejected(self):
        for dangling in (False, True):
            with self.subTest(dangling=dangling):
                path = self.source / ".claude/skills/method/data/nested.bin"
                path.unlink()
                outside = self.root / ("absent" if dangling else "outside.bin")
                if not dangling:
                    outside.write_bytes(BINARY)
                try:
                    path.symlink_to(outside)
                except OSError as error:
                    self.skipTest(f"Symlink creation unavailable: {error}")
                self.assert_rejected()
                self.assertFalse(self.calls)
                path.unlink()
                path.write_bytes(BINARY)

    def test_C4_destination_link_cannot_redirect_outside_project(self):
        outside = self.root / "outside"
        outside.mkdir()
        path = self.target / ".claude/skills"
        try:
            path.symlink_to(outside, target_is_directory=True)
        except OSError as error:
            self.skipTest(f"Symlink creation unavailable: {error}")
        self.assert_rejected(force=True)
        self.assertEqual(snapshot(outside), {})
        self.assertFalse(self.calls)

    def test_C4_linked_staged_output_is_rejected(self):
        probe = self.root / "link-probe"
        try:
            probe.symlink_to(self.source)
        except OSError as error:
            self.skipTest(f"Symlink creation unavailable: {error}")
        probe.unlink()
        self.output_mode = "linked"
        self.assert_rejected()

    def test_C4_reparse_source_is_rejected(self):
        real_lstat = Path.lstat
        chosen = (self.source / ".claude/skills/method/SKILL.md").resolve()
        class ReparseStat:
            def __init__(self, original):
                self.original = original
                self.st_file_attributes = getattr(stat, "FILE_ATTRIBUTE_REPARSE_POINT", 1024)
            def __getattr__(self, name):
                return getattr(self.original, name)
        def lstat(path, *args, **kwargs):
            result = real_lstat(path, *args, **kwargs)
            return ReparseStat(result) if path == chosen else result
        with patch.object(Path, "lstat", lstat):
            self.assert_rejected()
        self.assertFalse(self.calls)

    def test_C2_empty_required_category_is_rejected(self):
        shutil.rmtree(self.source / ".claude/commands")
        self.assert_rejected()
        self.assertFalse(self.calls)

    def test_B005_cli_accepts_host_manager_target_and_force(self):
        for host in ("claude", "codex"):
            for manager in ("skillfile", "apm"):
                with self.subTest(host=host, manager=manager), patch.object(installer, "install", return_value=0) as install_call:
                    self.assertEqual(installer.main(["--host", host, "--manager", manager, "--target", str(self.target), "--force"]), 0)
                    call = install_call.call_args
                    self.assertIsNotNone(call)
                    supplied = dict(zip(("source", "target", "host", "manager", "force"), call.args)) | call.kwargs
                    self.assertEqual(supplied["host"], host)
                    self.assertEqual(supplied["manager"], manager)
                    self.assertEqual(Path(supplied["target"]), self.target)
                    self.assertTrue(supplied["force"])

    def test_B005_cli_help_invalid_arguments_and_failure_exit(self):
        with contextlib.redirect_stdout(io.StringIO()), self.assertRaises(SystemExit) as help_exit:
            installer.main(["--help"])
        self.assertEqual(help_exit.exception.code, 0)
        with contextlib.redirect_stderr(io.StringIO()), self.assertRaises(SystemExit) as invalid_exit:
            installer.main(["--host", "other"])
        self.assertEqual(invalid_exit.exception.code, 2)
        stderr = io.StringIO()
        with patch.object(installer, "install", side_effect=RuntimeError("synthetic failed phase")), contextlib.redirect_stderr(stderr):
            self.assertEqual(installer.main(["--host", "claude", "--manager", "apm", "--target", str(self.target)]), 1)
        self.assertIn("synthetic failed phase", stderr.getvalue())


if __name__ == "__main__":
    unittest.main()
