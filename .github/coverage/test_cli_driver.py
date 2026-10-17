"""Finite compiled-entrypoint cases used while Coverlet instruments original F# sources."""

import argparse
from pathlib import Path
import shutil
import subprocess
import tempfile


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dll", required=True, type=Path)
    parser.add_argument("--repo", required=True, type=Path)
    args = parser.parse_args(argv)
    with tempfile.TemporaryDirectory(prefix="coverage-cli-") as temporary:
        root = Path(temporary) / "synthetic checkout with spaces"
        root.mkdir()
        for relative in ("agents", "skills", "commands", "hosts", "schemas", ".specht/templates", ".claude", ".codex", ".agents"):
            source = args.repo / relative
            if source.is_dir():
                shutil.copytree(source, root / relative)

        def invoke(expected, *arguments):
            result = subprocess.run(["dotnet", str(args.dll.resolve()), *arguments], capture_output=True, text=True)
            assert result.returncode == expected, (arguments, result.returncode, result.stdout, result.stderr)
            return result.stdout + result.stderr

        # Given valid inputs at an explicit root, when writing, then output is produced there.
        written = invoke(0, "--root", str(root))
        assert "outputs" in written
        assert (root / ".codex" / "agents" / "architect.toml").exists()

        # Given those outputs, when checking, then the original entrypoint reports current.
        current = invoke(0, "--root", str(root), "--check")
        assert "up to date:" in current

        # Given synthetic drift, when checking, then the original entrypoint reports a mismatch.
        output = root / ".codex" / "agents" / "architect.toml"
        output.write_bytes(output.read_bytes() + b"\n# invented drift\n")
        drift = invoke(1, "--root", str(root), "--check")
        assert "outputs differ" in drift

        # Given drift, when writing, then the same CLI repairs it.
        repaired = invoke(0, "--root", str(root))
        assert "written" in repaired
        assert b"invented drift" not in output.read_bytes()

        # Given an unsupported argument, when parsing, then the CLI rejects it.
        invalid = invoke(2, "--synthetic-invalid-argument")
        assert "unknown argument" in invalid

        # Given a required input is missing, when projecting, then ProjectionError is reported.
        shutil.rmtree(root / "hosts")
        failure = invoke(2, "--root", str(root))
        assert "error:" in failure

    print("6 compiled CLI cases passed")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
