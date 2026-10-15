---
title: "Decision 0004: Cost is an explicit estimate counted in full"
description: "Remaining work is measured by a cost authored per item, apart from risk and delivery state, and counted in full until delivered."
type: decision
---

# Decision 0004: Cost is an explicit estimate counted in full

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0004 (2026-10-13).

## The call

Remaining work is measured by an explicit cost authored for each item at
grooming, separate from its risk and its delivery state. An unfinished item
contributes its full cost until it is delivered; cost does not track partial
progress.

## Why

Ported from Hooked decision 0004. Cost belongs with the reviewable planning
inputs, apart from uncertainty and delivery state.

## Rejected

- An item's risk as its cost. It needs no new field, but couples size to
  uncertainty.
- Counting unfinished items equally. It needs no estimate, but ignores size.
- Updating cost to the work remaining as progress is made. Scores would reflect
  partial progress, at the cost of progress-accounting edits; with full cost a
  score improves only when an item is delivered.
- Fixing the estimate scale and where it is stored. Hooked's relative-point scale
  and storage, and the alternatives it turned down for them, are the companion's
  (C-2, § 5 row 3).

## Affects

B-023, B-024, § 5 row 2.

## Reversal

None.
