---
title: "Decision 0010: Readiness by stage and linked claims"
description: "Each stage has its own gate, read from evidence scoped to the work's linked claims, and only work finished as completed is delivered."
type: decision
---

# Decision 0010: Readiness by stage and linked claims

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0010 (2026-10-13).

## The call

Readiness is evaluated separately for each stage, from evidence scoped to the
work's linked claims:

- Design needs §§ 1-5 approved for its claims and no open question blocking them.
  It can proceed while implementation prerequisites are unfinished.
- Implementation needs §§ 1-7 approved, the design Ready, no blocking question,
  and every prerequisite delivered.
- Delivered means finished as completed. Work in review, closed as not planned
  or reopened is not delivered.
- An open question blocking only other claims withholds nothing.
- A status that reads as approved is not approval; missing or ambiguous evidence
  is reported, never inferred.

## Why

Ported from Hooked decision 0010. Agreed inputs can permit design while delivery
prerequisites still hold implementation.

## Rejected

- Holding every dependent stage until implementation prerequisites are
  delivered. Simpler, but stops design its inputs already permit.
- Accepting a prerequisite in review as delivered. Dependents start sooner, but
  build on changes review can still alter.
- Also requiring the prerequisite's change to be merged. It excludes
  investigation and documentation outcomes that merge nothing; the chosen call
  relies on the completion state being accurate.
- Requiring the whole specification approved and every question resolved.
  Simpler to check, but stops independent progress; the chosen call needs
  precise claim links (decision 0009) and blocking-question references (OQ-4; a question naming no
  claim leaves readiness unverified, added 2026-10-15,
  [decision 0019](0019-an-unscoped-question-leaves-readiness-unverified.md)).

## Affects

B-011, B-012, B-013, B-014, B-015, B-018, B-020.

## Reversal

None.
