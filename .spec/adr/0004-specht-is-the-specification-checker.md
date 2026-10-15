---
title: "ADR-0004: specht is the specification checker"
description: "Specifications in this repository are checked by the specht.tool local tool, restored from GitHub Packages"
type: adr
---

# ADR-0004: specht is the specification checker

## Status

accepted

## Context

This repository ships the specification-driven method and must follow it. Each
tooling agreement is a `.spec/README.md` with a companion `.feature` file. Those
files need a checker that is offline, deterministic and schema-pinned. See
`scripts/.spec/README.md` and `agents/.spec/README.md`.

## Decision drivers

1. One checker, versioned, shared across every consuming repository.
2. Templates, frontmatter schemas and section order owned by the checker.
3. CI fails on a specification violation.

## Considered options

1. `specht.tool` as a local tool from GitHub Packages, pinned in `.config/dotnet-tools.json`.
2. `specht` downloaded as a release asset with a hash pin, as `hooked` does.
3. A vendored copy of the rule set in this repository.

## Decision

Option 1. `nuget.config` maps `specht.tool` to the `specht` source. The CI
credential is `NuGetPackageSourceCredentials_specht`, built from the
`SPECHT_PACKAGES_USER` variable and the `SPECHT_PACKAGES_TOKEN` secret. The
manifest, schemas and templates under `.specht/` are written by
`dotnet specht init --all` and never hand-edited except for `exclusions`.

## Consequences

- A clone needs a GitHub token with `read:packages` before `dotnet tool restore`.
- The twelve-section contract is owned by `.specht/manifest.json`; the companion
  skill links it rather than restating it.
- Option 2 is rejected because it adds a download step the package feed already
  covers. Option 3 is rejected because the rule set would drift from the tool.
