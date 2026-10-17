---
name: cleanup
description: "Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change."
---

Convene these roles as the body directs:

- `auditor`, whose contract is `.codex/agents/auditor.toml`
- `organizer`, whose contract is `.codex/agents/organizer.toml`
- `document-writer`, whose contract is `.codex/agents/document-writer.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

The auditor reads the repository at rest and reports drift, grouped by rule.
The organizer files findings needing behavior changes or edits outside the
queue and documentation owners' boundaries for their existing artifact owners,
and puts right the queue's own drift: what is misfiled, orphaned, or done in fact.
The document writer applies documentation corrections as one reviewable change
inside the given scope.

Work on: the request that invoked this skill
