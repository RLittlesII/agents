---
title: "Decision 0013: Recommend without taking work"
description: "Asking what is next only reports, leading with Feature comparisons, and takes no work."
type: decision
---

# Decision 0013: Recommend without taking work

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0013 (2026-10-13)
and its reversal of the same day.

## The call

Asking what is next only reports. It claims no item, edits no record, writes back
no derived value, and starts no branch, worktree or dispatch; taking a
recommendation is a separate delivery step.

The report leads with Feature comparisons: stable id, importance, remaining
work, blockers, completion path and next step, with importance, proximity to
completion and blocker count kept apart. Lanes, blockers, occupied work and
unscored work follow. The report names when its inputs were read. Expanding
detail shows the same choices from the same inputs.

## Why

Ported from Hooked decision 0013, including its reversal. The person chooses a
Feature to drive to completion; lanes explain the parallel work available.

## Rejected

- Offering to claim and start the chosen work. It saves a step, but makes
  discovery write delivery state; taking work is delivery's (§ 5 row 1).
- Lanes first, Hooked's original order. It makes an item the opening choice when
  the person wants to finish Features. Hooked reversed it before the port.
- Hooked's compact default with an expansion flag, and its naming of the
  repository revision. They are output and invocation forms, not rules; the read
  time is kept.

## Affects

B-001, B-002, B-008, B-039, B-040, § 5 row 1.

## Reversal

None in this Feature. Hooked's reversal is part of the call above.
