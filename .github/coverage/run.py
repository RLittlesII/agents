"""Importable, unimplemented API stubs for 0001-F6 repository verification."""

from pathlib import Path


def inventory(repo: Path, policy: dict) -> dict[str, str]:
    """Unimplemented: no source inventory has been inspected."""
    return {}


def changed_lines(repo: Path, base_sha: str, inventory: dict[str, str]) -> dict[str, set[int]]:
    """Unimplemented: no Git comparison has been performed."""
    return {}


def collect(repo: Path, snapshot: dict, output_dir: Path) -> dict:
    """Unimplemented: no tests or collectors have run."""
    return {}


def read_reports(repo: Path, snapshot: dict, manifest: dict) -> dict[str, dict[int, bool]]:
    """Unimplemented: no report evidence has been validated."""
    return {}


def evaluate(changed: dict[str, set[int]], executable_hits: dict[str, dict[int, bool]], threshold: int) -> dict:
    """Unimplemented: this placeholder cannot represent a passing gate."""
    return {"verdict": "unimplemented", "covered": 0, "changed": 0, "missed": []}


def main(argv: list[str] | None = None) -> int:
    """Unimplemented: fail without reporting a coverage result."""
    return 2


if __name__ == "__main__":
    raise SystemExit(main())
