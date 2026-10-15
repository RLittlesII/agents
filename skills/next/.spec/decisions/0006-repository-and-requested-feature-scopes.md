---
title: "Decision 0006: Repository and requested-Feature scopes"
description: "An answer is scoped to the whole repository or to a requested Feature set, and to no saved target."
type: decision
---

# Decision 0006: Repository and requested-Feature scopes

**Date:** 2026-10-14
**Decided by:** the repository maintainer, in Hooked decision 0006 (2026-10-13).

## The call

Two scopes are supported: the whole repository, with choices across lanes; and a
requested Feature or Feature set, focused on completing it and leaving out work
that advances none of it. A requested Feature that does not resolve is a scope
error, not a reason to widen the answer. A valid scope with no start is answered
with its reasons.

## Why

Ported from Hooked decision 0006. The maintainer needed both broad choices and a
path to finishing a chosen set of Features.

## Rejected

- Repository-wide only. It cannot focus on driving one Feature to completion.
- Requested Features only. It cannot offer broad choices across lanes.
- A saved release or goal target as a scope. Explicit selection needs no target
  definition or membership model, at the cost of naming the Features on each
  request (§ 5 row 4).
- Hooked's invocation syntax. It is an invocation form, not a rule.

## Affects

B-004, B-005, B-006, § 5 row 4.

## Reversal

None.
