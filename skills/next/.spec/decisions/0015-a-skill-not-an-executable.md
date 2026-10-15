---
title: "Decision 0015: A skill, not an executable"
description: "Next work is a skill an agent follows; nothing outside it computes the answer."
type: decision
---

# Decision 0015: A skill, not an executable

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0015 (2026-10-13).

## The call

Next work is a skill an agent follows. No executable, tool or command computes
the answer. Repository-specific access and planning metadata are the
companion's.

## Why

Ported from Hooked decision 0015. The ask was to adapt an agent workflow; it
introduced no tool to build.

## Rejected

- A standalone calculator with its own project, dependencies and tests. It is
  an invented delivery surface (§ 5 row 6). The chosen call means schema checks
  do not establish the skill's correctness; its instructions need direct review.
- Hooked's placement: its directory, and allocating no Feature or epic. Not
  carried: here the agreement is Feature F5 of epic 0001, because the method's
  skills are this repository's deliverable.

## Affects

§ 5 row 6, § 7, § 8.

## Reversal

None.
