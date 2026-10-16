"""Host installer interfaces for the test-first handoff."""

from pathlib import Path


def install(source: Path, target: Path, host: str, manager: str, force: bool = False) -> int:
    raise NotImplementedError("Installation behavior is not implemented")


def main(argv: list[str] | None = None) -> int:
    raise NotImplementedError("Installer command-line behavior is not implemented")


if __name__ == "__main__":
    raise SystemExit(main())
