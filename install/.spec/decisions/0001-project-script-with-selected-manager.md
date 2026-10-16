---
title: "Decision 0001: Project installation through a selected manager"
description: "The user approved project scope, script-driven managers, Python and explicit replacement defaults"
type: decision
---

# Decision 0001: Project installation through a selected manager

## Decision

On 2026-10-16 the stakeholder selected Microsoft apm, project folders and a
script invoking either Skillfile or apm; then approved the specification's
scope and defaults and requested implementation.

The approved defaults are Python 3.11+ without third-party Python dependencies,
explicit host and manager selection, a checkout as source and explicit `--force`
for replacing differing destination files. Installation is epic 0038, separate
from epic 0001's projection boundary.

## Alternatives

- User-wide installation adds a different ownership boundary and was not selected.
- Direct whole-repository installation through only a manager command requires
  package formats beyond the requested wrapper and was not selected.
- F# requires the projection runtime in consuming projects; the user explicitly
  permitted another cross-platform language and approved Python.
- Overwriting differing files by default loses consumer edits; explicit
  replacement was approved instead.
