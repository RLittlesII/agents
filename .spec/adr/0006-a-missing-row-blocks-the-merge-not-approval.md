---
title: "ADR-0006: A Missing traceability row blocks the merge, not approval"
description: "A § 9 Missing row blocks the pull request that delivers its claim and the item reaching done; it does not block a specification being approved"
type: adr
---

# ADR-0006: A Missing traceability row blocks the merge, not approval

## Status

accepted - 2026-10-14, by the repository owner (item 0005).

## Context

§ 9 of a specification carries one row per § 3 claim, naming its scenario and
its test. `Missing` in the test column is honest until the test exists
(`agents/test-writer.md`).

Three rules disagree about what a `Missing` row blocks:

- `agents/spec-reviewer.md` refuses to block `spec_status: approved` on
  `Missing` rows alone: a `Missing` row blocks the item reaching done.
- `specht` rule `SPEC060` (default severity error, and CI runs `--strict`)
  fails an approved specification that still carries a `Missing` cell.
- LTI's reviewer, which `spec-reviewer` was ported from, blocked the merge of a
  diff citing a claim whose row was still `Missing`. The port dropped that rule,
  so today such a diff draws no finding.

All four specifications in this repository are draft, so the conflict with
`SPEC060` is dormant.

## Decision drivers

1. Approval is agreement on the claims. A test cannot exist before the claim it
   tests is agreed, so tying approval to tests inverts the workflow.
2. A claim reaches users through a merge. That is the last point a missing test
   can be caught before the claim is reported as delivered.
3. One rule, stated once, that the reviewer and the checker do not contradict.

## Considered options

1. A `Missing` row blocks approval, as `SPEC060` enforces.
2. A `Missing` row blocks the pull request that delivers its claim, and the item
   reaching done; it does not block approval.
3. A `Missing` row blocks only the item reaching done.

## Decision

Option 2. A specification may be approved with `Missing` rows. A pull request
that delivers a claim carries that claim's § 9 row naming a test, and an item is
not done while a claim it delivers is `Missing`. `spec-reviewer` flags a diff
delivering a claim whose row is still `Missing`, and keeps its refusal to block
approval on `Missing` rows alone.

## Consequences

- `SPEC060` contradicts this decision. It stays at its default level while
  every specification is draft. Before the first specification is approved,
  `.specht/manifest.json` sets `"rules": { "SPEC060": "off" }`: `--strict`
  fails on a warning too, so `off` is the only level that agrees. ADR-0004
  limits hand edits under `.specht/` to `exclusions`; that change amends it.
- The merge gate is review, not the checker: `specht` does not read which
  claims a diff delivers.
- Option 1 is rejected by driver 1. Option 3 is rejected by driver 2: an item
  closes after its pull request merges, too late to stop an untested claim
  shipping.
