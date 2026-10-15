---
title: "ADR-0007: The companion declares the coverage threshold"
description: "The method requires a changed-line coverage threshold; each consuming repository's companion names the number and where it is checked"
type: adr
---

# ADR-0007: The companion declares the coverage threshold

## Status

accepted - 2026-10-14, by the repository owner (item 0006).

## Context

LTI's test writer held an 80% changed-line coverage gate as the single source
of truth for coverage, with a coverage skill behind it. The port to this
repository dropped the gate with no decision record and no Out of Scope row, so
nothing in the method now says whether coverage is measured at all.

This repository carries no product technology: the role and skill files name
no tool, path or command, and a consuming repository's companion skill names
them (`sdd-conventions` § "Your companion"). A coverage number depends on the
stack, the test tiers and what the tool counts.

## Decision drivers

1. A gap no gate catches is a claim nobody proved. Coverage of the lines a
   change touches is the cheapest signal that one was missed.
2. The method names no tool or number that belongs to one consuming repository.
3. A coverage number is a floor, never a target: `test-from-scenarios` already
   forbids a test that exists only to raise it.

## Considered options

1. Drop the gate; coverage is each repository's own affair.
2. Fix a number in the method, as LTI did.
3. Require a changed-line threshold in the method; the companion names the
   number and where it is checked.

## Decision

Option 3. Every consuming repository has a changed-line coverage threshold. Its
companion § "Testing" names the number and the gate that checks it. The test
writer cites that threshold in § 8 and reports a change that falls below it;
the reviewer of the diff treats a gate lowered to pass as a weakened test.

## Consequences

- `sdd-conventions` § "Your companion" lists the threshold under Testing.
- A companion that names no threshold is incomplete, as a companion that names
  no test project is.
- Option 1 is rejected by driver 1, option 2 by driver 2.
- LTI's per-claim case classes (happy, edge, corner) are not adopted here;
  `test-from-scenarios` keeps one test per behaviour a claim states.
