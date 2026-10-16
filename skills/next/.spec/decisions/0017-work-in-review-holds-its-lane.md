---
title: "Decision 0017: Work in review holds its lane"
description: "Work in review is taken and occupies its lane until it is delivered."
type: decision
---

# Decision 0017: Work in review holds its lane

**Date:** 2026-10-15
**Decided by:** the repository maintainer, resolving OQ-2.

## The call

Work in review is taken. It occupies its lane until it is delivered, and no new
start is offered in that lane meanwhile.

The rule it follows: a lane frees only on delivery. Review is not delivery
(B-018), and its cost is already counted in full until delivered (B-024).

## Why

Review can send changes back to the files the work touched. A start offered in
the same lane would then collide with that rework.

## Rejected

Freeing the lane once work enters review. It allows more parallel work, but
raises the chance of a conflict whenever review asks for changes.

## Affects

B-017, B-018, B-024, OQ-2,
[decision 0011](0011-lanes-inferred-from-shared-edits.md).

## Reversal

None.
