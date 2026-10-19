---
title: "Decision 0004: Test details trace to approved claims"
description: "Approved supporting details may authorize assertions under an existing claim."
type: decision
---

# Decision 0004: Test details trace to approved claims

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Allow approved details traceable to an existing claim
(Recommended)."

An existing claim anchors each behavior-change assertion. Approved constraints,
technical design and signal declarations may supply supporting details when
traceable to that claim. Evidence cites the claim, the supporting source and
its recorded approval. Draft details and readiness verdicts alone do not supply
approval; new user-visible behavior still requires an agreement amendment.

The portable rule lives in
[`test-from-scenarios` § A claim comes first](../../../skills/test-from-scenarios/SKILL.md#a-claim-comes-first).
Test and section ownership remain unchanged.

Requiring every implementation or signal detail in claim text was rejected:
it would duplicate approved design and prevent the declared-signal assertion
handoff. Allowing unapproved details or new behavior inferred from design was
also rejected because it bypasses agreement authority.
