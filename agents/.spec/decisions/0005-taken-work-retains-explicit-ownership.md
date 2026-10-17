---
title: "Decision 0005: Taken work retains explicit ownership"
description: "The recorded owner may resume; transfers require a release or authorized reassignment."
type: decision
---

# Decision 0005: Taken work retains explicit ownership

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Allow the recorded owner to resume; require an explicit release or authorized reassignment for another owner, never a timeout alone (Recommended)".

The recorded owner may resume an interrupted assignment after checking current
ownership and delivery evidence. Another owner needs the recorded owner's
explicit release or an authorized reassignment before claiming or editing it.
Elapsed time, silence or an inactive session alone grants no ownership.

The portable rule lives in
[`deliver-change` § Resume, release and reassignment](../../../skills/deliver-change/SKILL.md#resume-release-and-reassignment).
The companion identifies the ownership record and its authorized maintenance.
Missing or conflicting ownership is diagnosed, not guessed. Resuming retains
existing scope, artifact boundaries and applicable stage gates. A release does
not deliver the work or satisfy its dependents.

Requiring user permission for every same-owner continuation was rejected because
it blocks ordinary recovery despite recorded ownership. Automatic stale-work
reclamation was rejected because inactivity does not prove relinquishment.
No automatic lease, timeout, new role or mandatory tracker field is introduced.
