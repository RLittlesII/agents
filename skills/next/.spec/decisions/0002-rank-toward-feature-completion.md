---
title: "Decision 0002: Rank toward Feature completion"
description: "A Feature scores its value over the summed cost of its unfinished closure, and the work it needs inherits its standing."
type: decision
---

# Decision 0002: Rank toward Feature completion

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0002 (2026-10-13).

## The call

Rank work toward finishing valuable Features sooner. A Feature's completion
score is its value divided by the summed cost of its unfinished closure: its open
children and every open item they wait on, in any Feature, each counted once and
at full cost until delivered. A prerequisite a selected Feature needs inherits
that Feature's standing, so it is not ranked below the Feature it unlocks because
another Feature owns it. An unselected Feature confers no standing.

## Why

Ported from Hooked decision 0002. The maintainer wanted valuable Features
finished, including the work that unlocks them.

## Rejected

- Ordering by existing Feature scores and authored priority alone. It needs no
  definition of remaining work, but does not favour the Feature nearest
  completion.
- Ordering by least remaining cost alone. It ignores value; the ratio balances
  value against proximity to completion, at the cost of sometimes choosing a
  larger, more valuable Feature.
- An item's risk as a proxy for its remaining work, and product-goal priority
  tiers. Neither measures distance to a finished Feature.

## Affects

B-023, B-024, B-025, B-026.

## Reversal

None.
