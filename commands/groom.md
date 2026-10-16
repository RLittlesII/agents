---
name: groom
description: Re-score, split, merge, rank and close work items that have drifted from the current specifications, file a new item, or report the queue's health.
roles: [organizer]
skills: [next, spec-and-traceability]
argument-hint: "[scope: an epic, a Feature or a milestone] | file <claim or observation> | health"
---

Groom the queue in the given scope, or the whole queue when none is given.
Every re-score states its basis; derived fields are recomputed, never typed.
Propose splits and closures with the signal behind each, and apply them on
agreement. Report the queue before and after.

Given `file`, cut one item from the named claims, or from an observation for a
bug, spike or chore, and claim its id. Given `health`, report — changing
nothing — what was last worked on, what is in progress, what is waiting and
why, and what is tracked nowhere: a claim no item delivers, a branch no item
names.

Work on: $ARGUMENTS
