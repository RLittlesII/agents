---
title: "Decision 0009: Explicit claim assignment"
description: "An item names its claims by fully qualified id, and a claim is never inferred from the item's prose."
type: decision
---

# Decision 0009: Explicit claim assignment

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0009 (2026-10-13).

## The call

Each item names the claims it delivers as an explicit list of fully qualified
claim ids, resolved against the specifications that own them. The list carries
references, not claim text. A claim is never inferred from an item's prose, and an
item whose list is missing or names no existing claim is unlinked.

## Why

Ported from Hooked decision 0009. Readiness is checked per linked claim, so the
links must resolve without interpretation.

## Rejected

- Extracting claim links from acceptance-criteria prose. It needs no new field,
  but the links are unreliable. Explicit links cost a field and a grooming pass
  over existing references.
- Fixing where the list is stored. Hooked's field is the companion's (C-2).

## Affects

B-031; the linked-claim gates of B-011, B-013 and B-015 read it.

## Reversal

None.
