---
title: "Decision 0011: Lanes inferred from shared edits"
description: "Lanes are inferred from dependencies and evidenced shared edits with the uncertainty stated, and taken work occupies its lane."
type: decision
---

# Decision 0011: Lanes inferred from shared edits

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0011 (2026-10-13).

## The call

Lanes are inferred from dependencies and evidenced shared edits, and each
inferred conflict states its evidence and its uncertainty. A lane predicts
parallel work; it does not promise a conflict-free merge. Taken work occupies its
lane and stays visible with its state, and no new start is offered there. Items
declare no write set for this.

Not decided here: whether work in review holds its lane (OQ-2), and whether a
Feature is one lane with one active item (OQ-6).

## Why

Ported from Hooked decision 0011. Lane predictions must account for shared edits
as well as dependencies.

## Rejected

A per-item declaration of the files it will edit. It predicts more precisely,
but is another field to author and groom; inference may serialise work
unnecessarily (§ 5 row 5).

## Affects

B-017, B-034, B-035, B-036, § 5 row 5, OQ-2, OQ-6.

## Reversal

None.
