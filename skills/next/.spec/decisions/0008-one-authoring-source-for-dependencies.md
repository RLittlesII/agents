---
title: "Decision 0008: One authoring source for dependencies"
description: "Prerequisites are authored once by stable id, delivery state is read from the tracker, and a stored derived relationship is not an input."
type: decision
---

# Decision 0008: One authoring source for dependencies

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0008 (2026-10-13).

## The call

An item's prerequisites are authored once, by stable id, in the one source the
companion names. Each prerequisite's delivery state is read from the tracker
(C-1). A relationship stored anywhere else, including a derived field such as a
waiting status, is not an input. Which record wins when a mirrored status
disagrees with the tracker is not decided here (OQ-1).

## Why

Ported from Hooked decision 0008. One authoring source for dependencies avoids
reconciling competing graphs.

## Rejected

- Authoring dependencies in two sources, as Hooked would have by authoring task
  dependencies in its tracker and Feature dependencies in specifications. Either
  source could be edited, but the two graphs must then be reconciled.
- Hooked's tracker reference syntax for a standalone prerequisite, and how its
  aliases resolve to one identity. They are the companion's identity contract
  (C-2).

## Affects

B-019, B-021, C-1, OQ-1.

## Reversal

None.
