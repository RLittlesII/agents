---
title: "Decision 0018: A duplicate id is diagnosed"
description: "Two records claiming one stable id are named as a diagnostic, and the work they affect is withheld; neither record is chosen."
type: decision
---

# Decision 0018: A duplicate id is diagnosed

**Date:** 2026-10-15
**Decided by:** the repository maintainer, resolving OQ-3.

## The call

Given two records claiming one stable id, this Feature names the duplicate as a
diagnostic and withholds the starts that depend on either record. It never
chooses one. Which record is right, and how ids are allocated, stays the
companion's identity contract (C-2).

The rule it follows: incomplete or ambiguous evidence withholds only the
affected start ([decision 0012](0012-isolate-incomplete-delivery-evidence.md)).

## Why

Choosing a record silently ranks work on inputs nobody agreed to. Naming the
duplicate sends it to whoever owns the ids, and independent work is still
offered.

## Rejected

Leaving duplicate ids wholly to the companion and assuming uniqueness here. It
keeps the skill smaller, but a duplicate would go unseen and one record would be
used by accident.

## Affects

B-029, B-032, B-043, C-2, OQ-3.

## Reversal

None.
