---
name: cleanup
description: "Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

The repository pass reads the repository at rest and reports drift, grouped by rule.
The queue owner files findings needing behavior changes or edits outside the
queue and documentation owners' boundaries for their existing artifact owners,
and puts right the queue's own drift: what is misfiled, orphaned, or done in fact.
The documentation owner applies documentation corrections as one reviewable change
inside the given scope.

Work on: the request that invoked this skill
