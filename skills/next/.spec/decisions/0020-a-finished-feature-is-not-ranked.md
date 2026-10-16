---
title: "Decision 0020: A finished Feature is not ranked"
description: "A Feature with an empty unfinished closure and every active claim delivered is reported finished and left out of ranking."
type: decision
---

# Decision 0020: A finished Feature is not ranked

**Date:** 2026-10-15
**Decided by:** the repository maintainer, resolving OQ-5.

## The call

A Feature whose unfinished closure is empty and whose every active claim is
delivered is reported as finished and left out of ranking. A Feature whose
children are delivered but which still has an active claim with neither planned
work nor verified delivery is not finished (B-033).

The rule it follows: delivery evidence for the whole agreement closes a
Feature, and a finished Feature has no remaining cost to divide by.

## Why

The completion score divides by the closure's cost. An empty closure would give
an infinite score, which the ranking model forbids; leaving the Feature out
removes that case rather than inventing a value.

## Rejected

Keeping the Feature in the comparison with an uncomputed score and a delivered
state. It stays visible, but it puts work with nothing left to do into a list of
choices.

## Affects

B-028, B-033, B-045, OQ-5.

## Reversal

None.
