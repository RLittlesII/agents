---
title: "Decision 0016: The tracker is the only delivery source"
description: "Delivery state is read only from the tracker; a disagreeing mirror is a diagnostic and an unreadable tracker offers no start."
type: decision
---

# Decision 0016: The tracker is the only delivery source

**Date:** 2026-10-15
**Decided by:** the repository maintainer, resolving OQ-1.

## The call

Delivery state is read only from the tracker (C-1). A specification's mirrored
`status` that disagrees with the tracker is named as a diagnostic and is not an
input. When the tracker cannot be read, the affected work has unverified
delivery state: no start is offered for it, and the gap is named.

The rule it follows: one authoring source for each fact
([decision 0008](0008-one-authoring-source-for-dependencies.md)), and incomplete
evidence withholds only the affected start
([decision 0012](0012-isolate-incomplete-delivery-evidence.md)).

## Why

A mirror is a copy written for reading without a network call. Treating it as a
fallback makes a stale copy authoritative exactly when nothing can check it.

## Rejected

Falling back to the mirrored status when the tracker cannot be read, with the
answer marked degraded. It keeps answering while the tracker is down, but it can
offer a start on work that is already taken or delivered.

## Affects

B-030, B-042, C-1, OQ-1.

## Reversal

None.
