---
title: "Decision 0019: An unscoped question leaves readiness unverified"
description: "An open question whose row names no blocked claim leaves the Feature's design and implementation readiness unverified, so those starts are withheld and the gap is named."
type: decision
---

# Decision 0019: An unscoped question leaves readiness unverified

**Date:** 2026-10-15
**Decided by:** the repository maintainer, resolving OQ-4.

## The call

An open question whose § 11 row names no blocked claim leaves the readiness of
its Feature's design and implementation work unverified. Those starts are
withheld, and the report names the question as a gap until its row names the
claims it blocks. Intake and decomposition work, which need no approval (B-010),
are not withheld for it. A row whose Blocks cell reads `None` names that it blocks nothing; it
is scoped, not unscoped.

The rule it follows: unverified is not ready
([decision 0012](0012-isolate-incomplete-delivery-evidence.md)). A question that
does name its claims blocks only those (B-015).

## Why

An open question with no scope cannot be shown not to block a claim. Offering
the work anyway turns a missing field into permission.

## Rejected

- Blocking every claim of the Feature. It withholds the same starts, but
  reports a known block where the true state is a gap in the evidence.
- Blocking nothing. It never stalls work, but an important question is ignored
  only because its row is incomplete.

## Affects

B-011, B-013, B-015, B-044, OQ-4.

## Reversal

None.
