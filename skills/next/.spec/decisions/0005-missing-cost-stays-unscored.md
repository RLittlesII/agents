---
title: "Decision 0005: Missing cost stays unscored"
description: "An item without a cost leaves every score depending on it uncomputed, while independent work is still ranked."
type: decision
---

# Decision 0005: Missing cost stays unscored

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0005 (2026-10-13).

## The call

An unfinished item without a cost is reported as needing an estimate. Every score
that depends on it stays uncomputed: no default, never zero. Work whose inputs
are complete is still ranked. A missing score does not by itself change an item's
delivery state or make it blocked.

## Why

Ported from Hooked decision 0005. An invented cost can change which Feature
appears closest to completion.

## Rejected

A labelled temporary default. It gives a more complete ranking, but can misstate
which Feature is nearest completion. The chosen call leaves part of the queue
unranked until its costs are supplied.

## Affects

B-028, B-029, B-037.

## Reversal

None.
