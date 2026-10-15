---
name: groom
description: Re-score, split, merge, rank and close work items that have drifted from the current specifications, and report the queue before and after.
roles: [organizer]
skills: [next, spec-and-traceability]
argument-hint: "[scope: an epic, a Feature or a milestone]"
---

Groom the queue in the given scope, or the whole queue when none is given.
Every re-score states its basis; derived fields are recomputed, never typed.

**Preconditions and halts.** An existing queue. Propose splits and closures with
the signal behind each, and wait for agreement before applying them.

Work on: $ARGUMENTS
