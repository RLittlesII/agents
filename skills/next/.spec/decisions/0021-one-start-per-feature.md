---
title: "Decision 0021: One start per Feature"
description: "Lanes come from dependencies and evidenced shared edits, not the Feature boundary, but at most one start is offered per Feature."
type: decision
---

# Decision 0021: One start per Feature

**Date:** 2026-10-15
**Decided by:** the repository maintainer, resolving OQ-6.

## The call

Two startable items in one Feature with no shared edit sit in two lanes: lanes
come only from dependencies and evidenced shared edits
([decision 0011](0011-lanes-inferred-from-shared-edits.md)). The recommendation
still offers at most one start per Feature, because delivery allows one item in
progress at a time per Feature
([`deliver-change`](../../../deliver-change/SKILL.md) § "The shape of it",
rule 3). The Feature's other startable items are shown queued behind its head.
A Feature that already has taken work is offered no further start.

The rule it follows: the delivery rule wins on starts, and inference wins on
lanes.

## Why

Recommending two starts that delivery then forbids sends people to work they
cannot take. Keeping the lanes inferred from edits still shows what could run in
parallel once the Feature's head lands.

## Rejected

- A Feature is one lane. It serialises unrelated work and adds a lane rule that
  decision 0011 does not have.
- Two starts per Feature, amending `deliver-change` rule 3 to one item per lane.
  It allows more parallel work, but it changes delivery text and every role that
  cites the rule.

## Affects

B-034, B-035, B-046, OQ-6.

## Reversal

None.
