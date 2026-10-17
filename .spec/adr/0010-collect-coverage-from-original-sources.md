---
title: "ADR-0010: Collect coverage from original sources"
description: "Repository verification instruments linked original F# scripts and measures Python sources without changing production runtimes."
type: adr
---

# ADR-0010: Collect coverage from original sources

## Status

Accepted — 2026-10-16, technical choice approved by the independent
0001-F6 section 12 design verdict. The user authorized repository-wide policy
and enforcement; [decision 0001](../../.github/.spec/decisions/0001-changed-line-coverage-policy.md)
records the adopted policy. This ADR binds coverage evidence across Feature
boundaries and therefore lives at repository scope.

## Context

[ADR-0007](0007-the-companion-declares-the-coverage-threshold.md) requires a
repository-owned coverage gate. [ADR-0003](0003-the-projector-is-an-fsharp-script.md)
keeps the projector's production entrypoint as FSI. Existing test pass counts
provide no executable-line evidence. The installer and new verification tools
also need Python coverage evidence. [0001-F6 section 7](../../.github/.spec/README.md#7-technical-design)
records the feasibility probes, interface contracts and failure boundaries.

## Decision drivers

1. Measure actual original production source lines, including zero-hit lines.
2. Preserve production FSI/Python entrypoints and the existing custom tests.
3. Run one source-bound evaluator locally and in CI without a coverage service.
4. Retain one authoring source for runtime dependency versions.

## Considered options

1. Continue existing pass-count checks without a coverage collector.
2. Collect FSI execution directly using dotnet-coverage.
3. Compile linked original F# sources only for Coverlet instrumentation, and
   collect Python execution with coverage.py.
4. Migrate production scripts and custom tests to packaged projects/frameworks.

## Decision

Choose option 3. Pin Coverlet MSBuild 6.0.4 centrally for coverage-only projects
and coverage.py 7.12.0 centrally for Python. Link original F# sources without
body rewriting; a test-only argument shim supplies the FSI argument contract.
Derive compiled runtime PackageReference items from existing authoritative
script declarations. Production commands and ADR-0003 remain unchanged.

The probes ran all 62 F# tests under instrumentation and emitted original-source
coverage. An original-linked CLI run measured its entrypoint separately.
Python probes measured 23 installer tests and positively reported unimported
synthetic source lines as zero-hit. These establish collector feasibility,
not executable claim coverage or a final changed-line score.

## Consequences

Coverage adds test-only compilation, instrumentation and pinned dependencies;
no hosted service or paid coverage provider is introduced. Source inventories,
report paths, hashes and Git comparison identities must be validated before
coverage arithmetic or N/A. Linux collector execution still needs CI evidence.

Option 1 fails driver 1 and ADR-0007. Option 2 did not produce a report in the
bounded macOS arm64 probe; it remains unproven rather than declared impossible.
Option 4 changes production/test contracts without satisfying an additional
claim. The chosen construction preserves those contracts at the cost of a
coverage-only build and explicit parity checks.
