"""Executable synthetic cases for 0001-F6; report fixtures are not collector proof."""

import contextlib
import copy
import hashlib
import importlib.util
import io
import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch
import xml.etree.ElementTree as ET


_SPEC = importlib.util.spec_from_file_location("repository_coverage", Path(__file__).with_name("run.py"))
gate = importlib.util.module_from_spec(_SPEC)
_SPEC.loader.exec_module(gate)

PROCESSES = ("python-installer-tests", "python-gate-tests", "python-combine", "python-report", "fsharp-unit", "fsharp-cli")
POLICY = {"schema_version": 1, "threshold_percent": 80,
          "include": ["**/*.py", "**/*.fsx", "**/*.fs"],
          "exclude": ["**/test_*.py", "**/*.tests.fsx", ".artifacts/**", ".agents/**", ".claude/**", ".codex/**", ".github/coverage/fsharp/FsiArguments.fs"]}


def git(repo, *args):
    """Local synthetic history with deterministic author/committer metadata."""
    environment = os.environ | {"GIT_AUTHOR_NAME": "Synthetic", "GIT_AUTHOR_EMAIL": "synthetic@example.invalid",
                               "GIT_COMMITTER_NAME": "Synthetic", "GIT_COMMITTER_EMAIL": "synthetic@example.invalid",
                               "GIT_AUTHOR_DATE": "2000-01-01T00:00:00Z", "GIT_COMMITTER_DATE": "2000-01-01T00:00:00Z"}
    return subprocess.run(["git", "-c", "commit.gpgsign=false", "-c", "core.hooksPath=/dev/null", "-C", str(repo), *args], check=True, capture_output=True, text=True, env=environment).stdout.strip()


def checkout(test):
    temporary = tempfile.TemporaryDirectory()
    test.addCleanup(temporary.cleanup)
    repo = Path(temporary.name)
    git(repo, "init", "--quiet")
    (repo / "tool.py").write_text("first = 1\nsecond = 2\n")
    git(repo, "add", ".")
    git(repo, "commit", "--quiet", "-m", "Synthetic baseline")
    return repo, git(repo, "rev-parse", "HEAD")


class EvaluationTests(unittest.TestCase):
    def test_B001_exact_floor_pools_languages_by_line_count(self):
        # Given one covered Python line and three of four covered F# lines.
        changed = {"tool.py": {1}, "tool.fsx": {1, 2, 3, 4}}
        hits = {"tool.py": {1: True}, "tool.fsx": {1: True, 2: True, 3: True, 4: False}}
        # When the shared evaluator applies the 80% floor.
        result = gate.evaluate(changed, hits, 80)
        # Then the pooled 4/5 passes and identifies the missed executable line.
        self.assertEqual(result, {"verdict": "pass", "covered": 4, "changed": 5, "missed": [("tool.fsx", 4)]})

    def test_B001_below_floor_does_not_round_up(self):
        # Given 799 hits among 1000 changed executable lines.
        changed = {"tool.py": set(range(1, 1001))}
        hits = {"tool.py": dict.fromkeys(range(1, 800), True) | dict.fromkeys(range(800, 1001), False)}
        # When coverage is evaluated before display rounding.
        result = gate.evaluate(changed, hits, 80)
        # Then 79.9% fails with exact counts.
        self.assertEqual((result["verdict"], result["covered"], result["changed"]), ("fail", 799, 1000))

    def test_B001_only_changed_executable_lines_enter_denominator(self):
        # Given a comment change and an unchanged missed executable line.
        changed = {"tool.py": {1, 2}}
        hits = {"tool.py": {2: True, 9: False}}
        # When changed and executable line identities are intersected.
        result = gate.evaluate(changed, hits, 80)
        # Then the single changed executable line is covered.
        self.assertEqual(result, {"verdict": "pass", "covered": 1, "changed": 1, "missed": []})

    def test_B003_no_changed_executable_lines_is_not_applicable(self):
        # Given valid evidence with only a changed non-executable line.
        # When evaluating the changed executable intersection.
        result = gate.evaluate({"tool.py": {1}}, {"tool.py": {2: False}}, 80)
        # Then the result has a zero denominator, not a percentage.
        self.assertEqual(result, {"verdict": "not-applicable", "covered": 0, "changed": 0, "missed": []})


class EvidenceTests(unittest.TestCase):
    def evidence(self):
        """Arrange reusable, wholly invented source and collector wire formats."""
        temporary = tempfile.TemporaryDirectory()
        self.addCleanup(temporary.cleanup)
        repo = Path(temporary.name)
        files = {"tool.py": "value = 1\nvalue = 2\n", "unit.fsx": "let value = 1\nlet other = 2\n", "cli.fsx": "let value = 1\nlet other = 2\n"}
        for name, content in files.items():
            (repo / name).write_text(content)
        sources = {name: hashlib.sha256((repo / name).read_bytes()).hexdigest() for name in files}
        output = repo / ".artifacts" / "coverage" / "synthetic"
        output.mkdir(parents=True)
        policy_file = repo / ".github" / "coverage" / "policy.json"
        policy_file.parent.mkdir(parents=True)
        policy_file.write_text(json.dumps(POLICY))
        snapshot = {"requested_base_sha": "1" * 40, "comparison_base_sha": "1" * 40, "head_sha": "2" * 40,
                    "policy_sha256": hashlib.sha256(policy_file.read_bytes()).hexdigest(), "sources": sources, "policy": copy.deepcopy(POLICY),
                    "tools": {"python": "7.12.0", "coverlet": "6.0.4", "dotnet": "10.0.401"}, "run_dir": output}
        reports = []
        for collector, name in (("python", "tool.py"), ("fsharp-unit", "unit.fsx"), ("fsharp-cli", "cli.fsx")):
            report = output / (collector + ".xml")
            report.write_text(f'<coverage><sources><source>{repo}</source></sources><packages><package><classes><class filename="{name}"><lines><line number="1" hits="1"/><line number="2" hits="0"/></lines></class></classes></package></packages></coverage>')
            reports.append({"path": report.name, "sha256": hashlib.sha256(report.read_bytes()).hexdigest(), "collector": collector})
        manifest = {key: copy.deepcopy(snapshot[key]) for key in ("requested_base_sha", "comparison_base_sha", "head_sha", "policy_sha256", "sources", "tools")}
        manifest.update(schema_version=1, reports=reports, process_results=[{"name": name, "exit_code": 0} for name in PROCESSES])
        return repo, snapshot, manifest

    def read(self, repo, snapshot, manifest):
        # Isolate Git and inventory effects; source bytes and reports stay real.
        current = {str(path.relative_to(repo)): hashlib.sha256(path.read_bytes()).hexdigest()
                   for path in repo.iterdir() if path.is_file() and path.suffix in (".py", ".fs", ".fsx")}
        with patch.object(gate, "inventory", return_value=current), patch("subprocess.run", return_value=subprocess.CompletedProcess([], 0, snapshot["head_sha"] + "\n", "")):
            return gate.read_reports(repo, snapshot, manifest)

    def rewrite_report(self, snapshot, manifest, text):
        report = snapshot["run_dir"] / manifest["reports"][0]["path"]
        report.write_text(text)
        manifest["reports"][0]["sha256"] = hashlib.sha256(report.read_bytes()).hexdigest()

    def test_B002_valid_reports_preserve_hit_and_miss_identity(self):
        # Given complete reports bound to a current synthetic snapshot.
        repo, snapshot, manifest = self.evidence()
        # When the evidence is validated and normalized.
        hits = self.read(repo, snapshot, manifest)
        # Then uncovered executable lines remain in every source's evidence.
        self.assertEqual(hits, {name: {1: True, 2: False} for name in snapshot["sources"]})

    def test_B002_missing_report_is_rejected(self):
        # Given a required report file that is absent.
        repo, snapshot, manifest = self.evidence()
        (snapshot["run_dir"] / manifest["reports"][0]["path"]).unlink()
        # When evidence is checked, then it cannot pass.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_malformed_report_is_rejected(self):
        # Given malformed XML with a matching report digest.
        repo, snapshot, manifest = self.evidence()
        self.rewrite_report(snapshot, manifest, "<coverage>")
        # When evidence is checked, then it cannot pass.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_stale_source_bytes_are_rejected_despite_same_mtime(self):
        # Given different bytes while source mtime remains unchanged.
        repo, snapshot, manifest = self.evidence()
        source = repo / "tool.py"
        timestamp = source.stat().st_mtime_ns
        source.write_text("value = 9\nvalue = 2\n")
        os.utime(source, ns=(timestamp, timestamp))
        # When evidence is checked, then content identity detects staleness.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_report_digest_mismatch_is_rejected(self):
        # Given a report whose bytes changed after the manifest was recorded.
        repo, snapshot, manifest = self.evidence()
        (snapshot["run_dir"] / manifest["reports"][0]["path"]).write_text("<coverage/>")
        # When evidence is checked, then its digest mismatch fails.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_report_outside_run_directory_is_rejected(self):
        # Given a manifest report escaping its trusted artifact directory.
        repo, snapshot, manifest = self.evidence()
        manifest["reports"][0]["path"] = "../python.xml"
        # When evidence is checked, then an external report cannot pass.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_new_production_source_absent_from_reports_is_rejected(self):
        # Given a newly inventoried file that no collector represents.
        repo, snapshot, manifest = self.evidence()
        (repo / "new.py").write_text("missed = 42\n")
        digest = hashlib.sha256((repo / "new.py").read_bytes()).hexdigest()
        snapshot["sources"]["new.py"] = digest
        manifest["sources"]["new.py"] = digest
        # When evidence is checked, then the new file cannot disappear.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_mismapped_source_is_rejected(self):
        # Given valid XML naming a source outside the measured checkout.
        repo, snapshot, manifest = self.evidence()
        self.rewrite_report(snapshot, manifest, '<coverage><packages><package><classes><class filename="../../outside.py"><lines><line number="1" hits="1"/></lines></class></classes></package></packages></coverage>')
        # When evidence is checked, then it cannot substitute external source.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_noninteger_line_is_rejected(self):
        # Given a report line whose identity is not a positive integer.
        repo, snapshot, manifest = self.evidence()
        report = snapshot["run_dir"] / manifest["reports"][0]["path"]
        self.rewrite_report(snapshot, manifest, report.read_text().replace('number="1"', 'number="bad"'))
        # When evidence is checked, then malformed identities fail.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_failed_test_process_is_rejected(self):
        # Given a required suite that failed despite complete XML.
        repo, snapshot, manifest = self.evidence()
        manifest["process_results"][0]["exit_code"] = 1
        # When evidence is checked, then coverage cannot override test failure.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_omitted_required_process_is_rejected(self):
        # Given a manifest that omits a required collector/test process.
        repo, snapshot, manifest = self.evidence()
        manifest["process_results"].pop()
        # When evidence is checked, then omission cannot count as success.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_different_comparison_revision_is_rejected(self):
        # Given reports made for a different comparison revision.
        repo, snapshot, manifest = self.evidence()
        manifest["comparison_base_sha"] = "4" * 40
        # When evidence is checked, then stale revision identity fails.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)

    def test_B002_duplicate_lines_union_hits_across_reports(self):
        # Given an initially missed line hit in another collector report.
        repo, snapshot, manifest = self.evidence()
        report = snapshot["run_dir"] / manifest["reports"][0]["path"]
        text = report.read_text().replace('</classes>', '<class filename="tool.py"><lines><line number="2" hits="1"/></lines></class></classes>')
        self.rewrite_report(snapshot, manifest, text)
        # When evidence is normalized, then each line has one OR-combined hit.
        hits = self.read(repo, snapshot, manifest)
        self.assertEqual(hits.get("tool.py"), {1: True, 2: True})

    def test_B002_source_with_only_empty_classes_is_rejected(self):
        # Given an inventoried executable source with no validated line evidence.
        repo, snapshot, manifest = self.evidence()
        report = snapshot["run_dir"] / manifest["reports"][0]["path"]
        xml = ET.fromstring(report.read_text())
        xml.find(".//class/lines").clear()
        self.rewrite_report(snapshot, manifest, ET.tostring(xml, encoding="unicode"))
        # When evidence is checked, then an empty class cannot create a false N/A.
        with self.assertRaises(ValueError):
            self.read(repo, snapshot, manifest)


class SourceTests(unittest.TestCase):
    def test_B004_inventory_preserves_carriage_return_in_literal_git_filename(self):
        # Given an invented valid local filename containing a carriage return.
        repo, _ = checkout(self)
        name = "literal\rsource.py"
        (repo / name).write_text("value = 1\n")
        # When NUL-delimited Git output is decoded without newline translation.
        sources = gate.inventory(repo, POLICY)
        # Then its exact bytes identity is inventoried rather than silently dropped.
        self.assertEqual(sources.get(name), hashlib.sha256((repo / name).read_bytes()).hexdigest())

    def test_B001_diff_treats_git_pathspec_magic_filename_literally(self):
        # Given a tracked source whose literal name resembles Git pathspec syntax.
        repo, _ = checkout(self)
        name = ":(literal)tool.py"
        (repo / name).write_text("first = 1\nsecond = 2\n")
        git(repo, "--literal-pathspecs", "add", "--", name)
        git(repo, "commit", "--quiet", "-m", "Synthetic literal filename")
        base = git(repo, "rev-parse", "HEAD")
        (repo / name).write_text("first = 1\nsecond = 9\n")
        sources = {name: hashlib.sha256((repo / name).read_bytes()).hexdigest()}
        # When its diff is measured, then Git must compare that exact source.
        self.assertEqual(gate.changed_lines(repo, base, sources), {name: {2}})

    def test_B001_binary_marker_in_added_source_text_remains_a_text_diff(self):
        # Given an ordinary source string containing words used by Git binary headers.
        repo, base = checkout(self)
        (repo / "tool.py").write_text('first = 1\nsecond = "Binary files are text here"\n')
        sources = {"tool.py": hashlib.sha256((repo / "tool.py").read_bytes()).hexdigest()}
        # When the diff is parsed, then source text remains a changed text line.
        self.assertEqual(gate.changed_lines(repo, base, sources), {"tool.py": {2}})

    def test_B004_inventory_includes_gate_and_excludes_test_generated_sources(self):
        # Given tracked source and new gate, test, generated and F# shim files.
        repo, _ = checkout(self)
        files = {".github/coverage/run.py": "value = 1\n", ".github/coverage/test_gate.py": "test = 1\n",
                 ".agents/generated.py": "generated = 1\n", ".github/coverage/fsharp/FsiArguments.fs": "module fsi\n",
                 "module.fs": "module Synthetic\nlet value = 1\n"}
        for name, text in files.items():
            (repo / name).parent.mkdir(parents=True, exist_ok=True)
            (repo / name).write_text(text)
        # When the production inventory is built from the declared policy.
        result = gate.inventory(repo, POLICY)
        # Then gate and F# production bytes count, while declared exclusions do not.
        expected = {name: hashlib.sha256((repo / name).read_bytes()).hexdigest()
                    for name in ("tool.py", ".github/coverage/run.py", "module.fs")}
        self.assertEqual(result, expected)

    def test_B001_diff_includes_modified_current_lines_and_all_new_file_lines(self):
        # Given a changed tracked line and an entirely new untracked source.
        repo, base = checkout(self)
        (repo / "tool.py").write_text("first = 9\nsecond = 2\n")
        (repo / "new.py").write_text("one = 1\ntwo = 2\n")
        sources = {name: hashlib.sha256((repo / name).read_bytes()).hexdigest() for name in ("tool.py", "new.py")}
        # When current added lines are compared to the baseline.
        changed = gate.changed_lines(repo, base, sources)
        # Then unchanged lines are absent and every new source line is included.
        self.assertEqual(changed, {"tool.py": {1}, "new.py": {1, 2}})

    def test_B001_deleted_lines_do_not_create_current_denominator(self):
        # Given a tracked source with one deleted line and no added line.
        repo, base = checkout(self)
        (repo / "tool.py").write_text("second = 2\n")
        sources = {"tool.py": hashlib.sha256((repo / "tool.py").read_bytes()).hexdigest()}
        # When the current diff is computed, then deleted lines contribute nothing.
        changed = gate.changed_lines(repo, base, sources)
        self.assertFalse(any(changed.values()))

    def test_B002_binary_source_diff_is_rejected(self):
        # Given source bytes that Git reports as a binary change.
        repo, base = checkout(self)
        (repo / "tool.py").write_bytes(b"first = 1\x00\n")
        sources = {"tool.py": hashlib.sha256((repo / "tool.py").read_bytes()).hexdigest()}
        # When changed lines are requested, then missing text evidence cannot pass.
        with self.assertRaises(ValueError):
            gate.changed_lines(repo, base, sources)


class GateAcceptanceTests(unittest.TestCase):
    def run_gate(self, changed, hits, environment):
        repo, base = checkout(self)
        location = repo / ".github" / "coverage"
        location.mkdir(parents=True)
        (location / "policy.json").write_text(json.dumps(POLICY))
        output = io.StringIO()
        errors = io.StringIO()
        # Given a synthetic checkout and deterministic declared collection seams.
        with patch.object(gate, "__file__", str(location / "run.py")), \
             patch.object(gate, "inventory", return_value={}), \
             patch.object(gate, "changed_lines", return_value=changed), \
             patch.object(gate, "collect", return_value={}), \
             patch.object(gate, "read_reports", return_value=hits), \
             patch.dict(os.environ, environment, clear=False), \
             contextlib.redirect_stdout(output), contextlib.redirect_stderr(errors):
            # When the caller invokes the shared entry point.
            code = gate.main(["--base", base])
        return code, output.getvalue() + errors.getvalue(), base

    def test_B001_gate_exit_and_counts_observe_exact_floor(self):
        # Given valid pooled evidence at exactly the agreed threshold.
        code, output, base = self.run_gate({"tool.py": {1, 2, 3, 4, 5}},
                                          {"tool.py": {1: True, 2: True, 3: True, 4: True, 5: False}}, {})
        # Then acceptance exit, comparison identity and exact counts are observable.
        self.assertEqual(code, 0)
        self.assertIn(base, output)
        self.assertRegex(output, r"4\s*/\s*5|covered\D+4.*changed\D+5")

    def test_B003_gate_reports_zero_denominator_without_percentage(self):
        # Given valid collection with no changed executable lines.
        code, output, _ = self.run_gate({}, {"tool.py": {1: False}}, {})
        # Then the caller receives successful, explicit N/A without a percentage.
        self.assertEqual(code, 0)
        self.assertIn("N/A", output)
        self.assertIn("0", output)
        self.assertNotIn("%", output)

    def test_B004_local_and_ci_share_policy_evaluator_and_output(self):
        # Given identical checkout evidence in local and CI invocation contexts.
        changed, hits = {"tool.py": {1}}, {"tool.py": {1: True}}
        local = self.run_gate(changed, hits, {"CI": "false"})
        ci = self.run_gate(changed, hits, {"CI": "true", "GITHUB_ACTIONS": "true"})
        # Then both obey the same evaluator policy and expose comparison/counts.
        self.assertEqual(local[0], 0)
        self.assertEqual(ci[0], 0)
        self.assertEqual(local[1], ci[1])
        self.assertIn(local[2], local[1])
        self.assertRegex(local[1], r"1\s*/\s*1|covered\D+1.*changed\D+1")


class CollectionTests(unittest.TestCase):
    def test_B002_unsupported_shell_path_is_rejected_before_any_process(self):
        # Given an inert literal dollar sign in a proposed runtime directory.
        with tempfile.TemporaryDirectory() as temporary:
            repo = Path(temporary) / "literal$dollar"
            output = repo / ".artifacts" / "coverage" / "synthetic"
            snapshot = {"run_dir": output}
            # When collection checks its shell-bound paths, then no process starts.
            with patch("subprocess.run") as child, self.assertRaises(ValueError):
                gate.collect(repo, snapshot, output)
            child.assert_not_called()

    def collection(self):
        # Child execution is faked; use a supported synthetic executable path.
        interpreter = patch.object(gate.sys, "executable", "/synthetic/python")
        interpreter.start()
        self.addCleanup(interpreter.stop)
        repo, base = checkout(self)
        directory = repo / ".github" / "coverage"
        (directory / "fsharp").mkdir(parents=True)
        real = Path(__file__).parent
        (directory / "requirements.txt").write_bytes((real / "requirements.txt").read_bytes())
        (directory / "fsharp" / "Coverage.props").write_bytes((real / "fsharp" / "Coverage.props").read_bytes())
        (repo / "scripts").mkdir()
        (repo / "scripts" / "Projection.fsx").write_text('#r "nuget: Synthetic.Dependency, 1.2.3"\nlet value = 1\n')
        (repo / "scripts" / "project.fsx").write_text("let entry = 1\n")
        output = repo / ".artifacts" / "coverage" / "synthetic"
        output.mkdir(parents=True)
        snapshot = {"requested_base_sha": base, "comparison_base_sha": base, "head_sha": base,
                    "policy_sha256": "3" * 64, "sources": gate.inventory(repo, POLICY),
                    "policy": copy.deepcopy(POLICY), "tools": {}, "run_dir": output}
        original_run = subprocess.run
        required = (directory / "requirements.txt").read_text().strip().split("==")[1]

        def child(command, **kwargs):
            if command[0] == "git":
                return original_run(command, **kwargs)
            if command[-1] == "--version":
                text = f"Coverage.py, version {required} synthetic\n" if "coverage" in command else "10.0.401\n"
                return subprocess.CompletedProcess(command, 0, text, "")
            if "xml" in command:
                Path(command[command.index("-o") + 1]).write_text("<coverage/>\n")
            if "msbuild" in command:
                project = ET.parse(command[2])
                Path(project.find(".//CoverletOutput").text).write_text("<coverage/>\n")
            return subprocess.CompletedProcess(command, 0, "", "")

        return repo, snapshot, output, child

    def test_B002_collection_publishes_complete_source_bound_manifest(self):
        # Given synthetic successful child processes and authoritative #r declarations.
        repo, snapshot, output, child = self.collection()
        # When actual orchestration collects fresh evidence through subprocess seams.
        with patch("subprocess.run", side_effect=child):
            manifest = gate.collect(repo, snapshot, output)
        # Then complete required evidence is bound to the snapshot and atomically published.
        self.assertEqual(manifest["sources"], snapshot["sources"])
        self.assertEqual({item["name"] for item in manifest["process_results"]}, set(PROCESSES))
        self.assertTrue(all(item["exit_code"] == 0 for item in manifest["process_results"]))
        self.assertEqual({item["collector"] for item in manifest["reports"]}, {"python", "fsharp-unit", "fsharp-cli"})
        self.assertEqual(json.loads((output / "manifest.json").read_text()), manifest)
        self.assertFalse((output / "manifest.tmp").exists())
        # Runtime versions come from the production declaration, preserving C-3.
        references = ET.parse(output / "RuntimePackages.props").findall(".//PackageReference")
        self.assertEqual([(item.attrib["Include"], item.attrib["Version"]) for item in references], [("Synthetic.Dependency", "1.2.3")])

    def test_B002_failed_child_cannot_publish_manifest(self):
        # Given a child test/collector failing before fresh evidence completes.
        repo, snapshot, output, child = self.collection()

        def failure(command, **kwargs):
            if "discover" in command:
                return subprocess.CompletedProcess(command, 7, "", "")
            return child(command, **kwargs)

        # When orchestration encounters failure, then it cannot publish passing evidence.
        with patch("subprocess.run", side_effect=failure), self.assertRaises(ValueError):
            gate.collect(repo, snapshot, output)
        self.assertFalse((output / "manifest.json").exists())

    def test_B002_source_mutation_during_collection_cannot_publish_manifest(self):
        # Given source bytes changing after the pre-collection snapshot.
        repo, snapshot, output, child = self.collection()

        def mutation(command, **kwargs):
            result = child(command, **kwargs)
            if "msbuild" in command:
                (repo / "tool.py").write_text("mutated = 1\n")
            return result

        # When actual orchestration rechecks source identity, then stale evidence fails.
        with patch("subprocess.run", side_effect=mutation), self.assertRaises(ValueError):
            gate.collect(repo, snapshot, output)
        self.assertFalse((output / "manifest.json").exists())


if __name__ == "__main__":
    unittest.main()
