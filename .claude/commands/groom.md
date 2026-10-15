---
description: "Re-score, split, merge, rank and close work items that have drifted from the current specifications, file a new item, or report the queue's health."
argument-hint: "[scope: an epic, a Feature or a milestone] | file <claim or observation> | health"
---

Act as the `organizer` role, whose contract is `.claude/agents/organizer.md`. Read it before acting, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `next` (`.claude/skills/next/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

Groom the queue in the given scope, or the whole queue when none is given.
Every re-score states its basis; derived fields are recomputed, never typed.
Report the queue before and after.

Given `file`, cut one item from the named claims, or from an observation for a
bug, spike or chore, and claim its id. Given `health`, report read-only what was
last worked on, what is in progress, what is waiting and why, and what is
tracked nowhere: a claim no item delivers, a branch no item names.

**Preconditions and halts.** An existing queue. Propose splits and closures with
the signal behind each, and wait for agreement before applying them. A health
report changes nothing.

Work on: $ARGUMENTS
