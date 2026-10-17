---
title: "Decision 0002: Refactors use existing contract evidence"
description: "Behavior-preserving refactors use preserved constraints and adequate tests, adding characterization only for gaps."
type: decision
---

# Decision 0002: Refactors use existing contract evidence

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Use preserved constraints and adequate tests; add
characterization tests for gaps (Recommended)."

`implementer` leads behavior-preserving refactors within the tracked item's
scope. The entry evidence is the preserved contract and adequate passing tests;
no new claim or deliberately failing test is required for unchanged behavior.
Test gaps return to `test-writer` before production changes. Specialist
contributions retain their existing write boundaries and use the same evidence.

The portable rule lives in
[`deliver-change` § Behavior-preserving refactors](../../../skills/deliver-change/SKILL.md#behavior-preserving-refactors).
Existing design and approval requirements remain applicable. A changed public
contract or behavior leaves this path and requires the normal agreement,
design and failing-test work.

Alternatives not selected: requiring new characterization tests for every
refactor, which adds tests even when existing coverage is adequate; keeping
refactors outside the implementation contract, which leaves another handoff.
This decision adds no role or delivery-state vocabulary.
