---
description: "Re-score, split, merge, rank and close work items that have drifted from the current specifications, and report the queue before and after."
argument-hint: "[scope: an epic, a Feature or a milestone]"
---

Act as the `organizer` role, whose contract is `.claude/agents/organizer.md`. Read it before acting, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `next` (`.claude/skills/next/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

Groom the queue in the given scope, or the whole queue when none is given.
Every re-score states its basis; derived fields are recomputed, never typed.

**Preconditions and halts.** An existing queue. Propose splits and closures with
the signal behind each, and wait for agreement before applying them.

Work on: $ARGUMENTS
