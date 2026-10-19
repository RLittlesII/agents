---
title: "Decision 0006: Published identifiers are permanent"
description: "IDs become permanent on merge or citation outside the authoring branch."
type: decision
---

# Decision 0006: Published identifiers are permanent

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Once merged or cited outside its authoring branch; unpublished IDs may change with all local references updated (Recommended)".

An ID becomes permanent when merged or cited outside its authoring branch.
Before that point, a new unpublished ID may change to resolve a collision only
with every local reference updated. A shared review, work item or external
citation counts as publication even if its branch has not merged. A withdrawn
permanent ID remains retired.

The portable rule lives in
[`sdd-conventions` § ID schemes](../../../skills/sdd-conventions/SKILL.md#id-schemes).
Rebase and inspect shared reservations before allocating; do not assume a branch
is unpublished merely because it is unmerged. Two colliding permanent IDs need
an explicit identity-reconciliation decision, not automatic renumbering.

Permanence at initial allocation with mandatory shared reservation was rejected:
it would require every private draft to coordinate before authoring. Treating
all unmerged IDs as provisional was also rejected because reviews and other
shared records may already cite them. Existing permanent citations are preserved;
this decision performs no renumbering.
