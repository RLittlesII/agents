"""Real pinned-tool regression evidence; generated reports never become fixtures."""

import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import unittest
import xml.etree.ElementTree as ET


HERE = Path(__file__).resolve().parent


class CollectorIntegrationTests(unittest.TestCase):
    def test_B002_python_collector_reports_missed_branch_and_unimported_new_source(self):
        # Given committed invented source, including an entirely unimported file.
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            shutil.copyfile(HERE / "fixtures/subject.py.fixture", root / "subject.py")
            shutil.copyfile(HERE / "fixtures/never.py.fixture", root / "never.py")
            (root / "runner.py").write_text("from subject import choose\nassert choose(True) == 1\n")
            environment = os.environ | {"COVERAGE_FILE": str(root / "coverage.data")}
            # When the real installed Python collector executes and reports.
            execution = subprocess.run([sys.executable, "-m", "coverage", "run", "--source=.", "runner.py"], cwd=root, env=environment, capture_output=True, text=True)
            self.assertEqual(execution.returncode, 0, execution.stdout + execution.stderr)
            reporting = subprocess.run([sys.executable, "-m", "coverage", "xml", "-o", "coverage.xml"], cwd=root, env=environment, capture_output=True, text=True)
            self.assertEqual(reporting.returncode, 0, reporting.stdout + reporting.stderr)
            classes = {Path(item.attrib["filename"]).name: item for item in ET.parse(root / "coverage.xml").findall(".//class")}
            # Then actual executable identities preserve the missed branch/new file.
            subject = {int(line.attrib["number"]): int(line.attrib["hits"]) for line in classes["subject.py"].findall("./lines/line")}
            never = {int(line.attrib["number"]): int(line.attrib["hits"]) for line in classes["never.py"].findall("./lines/line")}
            self.assertGreater(subject[3], 0)
            self.assertEqual(subject[4], 0)
            self.assertTrue(never)
            self.assertTrue(all(hits == 0 for hits in never.values()))

    def test_B002_fsharp_collector_reports_real_missed_branch_and_unused_new_source(self):
        # Given invented compiled F# source and the production collector target.
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            for name in ("Subject.fs", "Never.fs", "Program.fs"):
                shutil.copyfile(HERE / "fixtures" / (name + ".fixture"), root / name)
            props = HERE / "fsharp" / "Coverage.props"
            project = root / "Synthetic.fsproj"
            project.write_text(f'<Project Sdk="Microsoft.NET.Sdk"><Import Project="{props}"/><ItemGroup><Compile Include="Subject.fs"/><Compile Include="Never.fs"/><Compile Include="Program.fs"/></ItemGroup></Project>')
            local_feed = root / "offline-feed"
            local_feed.mkdir()
            # When installed cached packages build and collect without a network feed.
            restore = subprocess.run(["dotnet", "restore", str(project), "--source", str(local_feed), "--nologo"], cwd=HERE.parents[1], capture_output=True, text=True)
            self.assertEqual(restore.returncode, 0, restore.stdout + restore.stderr)
            dll = root / "bin" / "Debug" / "net10.0" / "Synthetic.dll"
            report = root / "coverage.xml"
            collection = subprocess.run(["dotnet", "msbuild", str(project), "-target:RunCovered", "-nologo",
                                         f"-property:RunnerCommand=dotnet {dll}", f"-property:CoverletOutput={report}"], cwd=HERE.parents[1], capture_output=True, text=True)
            self.assertEqual(collection.returncode, 0, collection.stdout + collection.stderr)
            classes = ET.parse(report).findall(".//class")
            subject = {int(line.attrib["number"]): int(line.attrib["hits"]) for item in classes if Path(item.attrib["filename"]).name == "Subject.fs" for line in item.findall("./lines/line")}
            never = {int(line.attrib["number"]): int(line.attrib["hits"]) for item in classes if Path(item.attrib["filename"]).name == "Never.fs" for line in item.findall("./lines/line")}
            # Then real sequence points retain both missed branches and unused files.
            self.assertGreater(subject[5], 0)
            self.assertEqual(subject[7], 0)
            self.assertTrue(never)
            self.assertTrue(all(hits == 0 for hits in never.values()))


if __name__ == "__main__":
    unittest.main()
