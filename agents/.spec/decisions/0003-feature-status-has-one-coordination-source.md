---
title: "Decision 0003: Feature status has one coordination source"
description: "A Feature specification mirrors one identified coordination item rather than combining child states."
type: decision
---

# Decision 0003: Feature status has one coordination source

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Mirror one Feature coordination item (Recommended)."

Each Feature's mirrored delivery status comes from one explicitly identified
Feature coordination item. Children retain their own delivery states. The
companion supplies the identity rule and synchronization operation; a missing
or ambiguous source is diagnosed without choosing a child or calculating an
aggregate. The maintenance owner copies the source value rather than authoring
a separate status in the specification.

The portable rule lives in
[`sdd-conventions` § The spec / work-item split](../../../skills/sdd-conventions/SKILL.md#the-spec--work-item-split).
The existing work-item owner maintains this mirror; specification maturity and
numbered-section ownership do not move.

Computing delivery status from children was not selected: mixed states would
require another aggregation policy and would replace the chosen tracker record
as the authority. No new status vocabulary or automatic tracker is introduced.
