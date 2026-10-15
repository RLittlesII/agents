---
title: "Decision 0003: Priority tier before completion score"
description: "Ranked work orders by authored priority tier first and completion score second, and ranking never overwrites the tier."
type: decision
---

# Decision 0003: Priority tier before completion score

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0003 (2026-10-13).

## The call

Order ranked work by the Feature's authored priority tier first, then by
completion score within the tier. A prerequisite takes the highest tier among the
selected Features it advances, then the best score within that tier. Ranking
never overwrites an authored tier.

## Why

Ported from Hooked decision 0003. Explicit urgency must survive a change in the
ranking formula.

## Rejected

- Ordering by completion score alone, across tiers. It finishes closer Features
  sooner, but discards urgency someone authored; the chosen order sometimes
  delays a closer, lower-tier Feature.
- Fixing the tier vocabulary here. Hooked's four tier names are its own; the
  companion names the vocabulary (C-2), and the model fixes only that tiers are
  ordered (C-3).

## Affects

B-022, B-025, B-040, C-3.

## Reversal

None.
