---
title: "Decision 0007: Approval follows the reviewed revision"
description: "Substantive changes need review of the changed scope; unchanged scope retains valid approval."
type: decision
---

# Decision 0007: Approval follows the reviewed revision

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Require review of the changed scope against its new revision;
carry forward approval for unchanged scope (Recommended)".

Approval applies to the identified subject revision and scope. A substantive
agreement or design change requires review of its changed scope against the new
revision. Unchanged scope retains applicable earlier approval, with its evidence
preserved. Earlier approval remains historical evidence for what was reviewed;
it does not authorize a changed subject merely because nobody revoked it.

The portable rule lives in
[`spec-and-traceability` § Approval applicability](../../../skills/spec-and-traceability/SKILL.md#approval-applicability).
The sign-off owner records returned verdicts and their applicability; it neither
supplies independent approval nor invents a favorable resolution of conflicting
verdicts. Stage gates read current applicable coverage, not all historical rows
as though they reviewed the same revision.

Keeping approval applicable until explicit revocation was rejected: a changed
subject could proceed before anyone noticed that the approval was stale.
Restarting review for every unchanged section was also rejected because it
throws away valid scoped evidence. No checker, automatic approval service or
new sign-off schema is introduced.
