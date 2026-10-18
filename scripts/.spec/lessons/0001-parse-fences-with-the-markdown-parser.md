---
title: "Lesson 0001: Parse fences with the Markdown parser"
description: "A fence-prefix toggle both rejected examples and concealed real ownership violations."
type: lesson
kind: product
---

# Lesson 0001: Parse fences with the Markdown parser

**Date:** 2026-10-17
**Kind:** product

## Symptom

Three B-016 regressions failed: two valid fenced examples were rejected, and
a real ownership violation after an example was accepted.

## Root cause

Commit `6fb217db` introduced a Boolean toggle on a triple-backtick prefix in
`validateStamps`. It did not recognize tilde fences or distinguish an outer
four-backtick fence from shorter backticks inside it. The original B-016 test
covered only a simple triple-backtick example.

The fix uses Markdig's parsed `FencedCodeBlock` source spans in
[`validateStamps`](../../Projection.fsx), preserving the ownership diagnostics.

## Spec delta

No claim delta: 0001-F1 B-016 already excludes stamps inside fenced blocks.
Its scenarios and executable tests now cover the previously untested forms.

## Claim

- 0001-F1 B-016 — `B016_TildeFencedExampleDoesNotCreateAnOwnershipViolation`.
- 0001-F1 B-016 — `B016_ShorterBackticksDoNotEndAFencedExample`.
- 0001-F1 B-016 — `B016_RealViolationAfterAFencedExampleIsStillRejected`.
