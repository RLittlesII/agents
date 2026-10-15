---
title: "ADR-0003: The projector is an F# script"
description: "Projection runs as dotnet fsi scripts/project.fsx with no packaged tool and no second runtime"
type: adr
---

# ADR-0003: The projector is an F# script

## Status

accepted

## Context

Something must turn a role file plus a host overlay into host-native files. The
repository already requires the .NET SDK for `specht`. The only code in the
repository is this projector and its tests. See `scripts/.spec/README.md` § 7.

## Decision drivers

1. No runtime beyond the one `specht` already needs.
2. No build, pack or publish step before the projector can run.
3. Tests that cite claims by name.

## Considered options

1. A Python script, as `hooked/scripts/install_agents.py`.
2. Bash generators, as the LTI roster uses.
3. A packaged .NET tool with its own release cycle.
4. The `skillfile` CLI as the projector.
5. An F# script run by `dotnet fsi`.

## Decision

Option 5. `scripts/Projection.fsx` holds the logic with no top-level effects;
`scripts/project.fsx` is the entry point; `scripts/project.tests.fsx` loads the
library and runs claim-named tests. YAML is read with YamlDotNet, pinned by
`#r "nuget:"`.

## Consequences

- The first run restores YamlDotNet from nuget.org; CI already reaches it.
- `dotnet fsi` startup is slower than a compiled tool; acceptable for a check
  that runs once per change.
- Option 1 adds a runtime. Option 2 parses frontmatter with regular expressions.
  Option 3 adds a release cycle for one script. Option 4 does not emit Codex
  agents and translates no model settings.
