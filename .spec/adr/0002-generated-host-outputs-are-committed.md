---
title: "ADR-0002: Generated host outputs are committed"
description: "The projected Claude and Codex files are tracked in the repository and checked for drift in CI"
type: adr
---

# ADR-0002: Generated host outputs are committed

## Status

accepted

## Context

Consumers pull agents and skills from this repository by path and git ref. A
consumer should not need the projector, its runtime, or a build step to obtain a
usable agent. The projector's output must therefore exist in the tree at every
ref. See `scripts/.spec/README.md` C-3 and B-008.

## Decision drivers

1. A consumer needs only `git` or `skillfile`.
2. Drift between source and output is detected before merge.
3. A reviewer sees the host file a change will produce.

## Considered options

1. Gitignore the outputs and have every clone run the projector, as `hooked` does.
2. Commit the outputs and have CI fail when they differ from a fresh projection.
3. Publish the outputs as a release artifact only.

## Decision

Option 2. `.claude/agents/`, `.claude/skills/`, `.codex/agents/` and
`.agents/skills/` are tracked. `dotnet fsi scripts/project.fsx --check` runs in
CI and exits non-zero when any output would change, is missing, or is stale.

## Consequences

- Every skill exists three times in the tree: the source and two copies. The
  copies are never edited; the check enforces it.
- A pull request that changes a role shows the projected diff beside it.
- Option 1 is rejected because it puts the runtime on every consumer. Option 3 is
  rejected because `skillfile` fetches by path and ref, not by release asset.
