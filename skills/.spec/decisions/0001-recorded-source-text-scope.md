---
title: "Decision 0001: Record the original source-text lint scope"
description: "Align the claims with exclusions present in the original design and implementation without changing runtime behavior."
type: decision
---

# Decision 0001: Record the original source-text lint scope

## Context

Baseline review found B-001's general file wording broader than §7's recorded
scope. Initial specification `593df19` already excluded `.spec/`; initial
implementation `ecb996c` already skipped files containing NUL bytes. Both are
ancestors of merged `origin/main` `c890c82`.

## Decision

C-2 states those existing exclusions. B-001 and B-003 refer to that scope;
their IDs and scenarios remain because the recorded behavior does not change.
The implementation, design and tests retain their existing contracts.

## Alternatives

- Include specification records: rejected because the original §7 excludes
  them, and records deliberately cite role identities and ownership.
- Interpret binary assets as skill instructions: rejected because the initial
  implementation excludes them; adding such lint behavior needs a new agreement.
- Leave the claim wording general: rejected because it permits contradictory
  readings and overstates the guarantee provided by the existing tests.

## Consequences

This correction supports agreement and evidence review without changing the
lint or declaring prerequisite 0001-F2 complete. ADR-0007's missing repository
coverage policy remains OQ-1; no threshold, gate or waiver is invented.
