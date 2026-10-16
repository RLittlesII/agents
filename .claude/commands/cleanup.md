---
description: "Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change."
argument-hint: "[scope]"
---

Convene these roles as the body directs:

- `auditor`, whose contract is `.claude/agents/auditor.md`
- `organizer`, whose contract is `.claude/agents/organizer.md`
- `document-writer`, whose contract is `.claude/agents/document-writer.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`), `deliver-change` (`.claude/skills/deliver-change/SKILL.md`).

The auditor reads the repository at rest and reports drift, grouped by rule.
The organizer files an item for each finding that needs a behaviour change, and
puts right the queue's own drift: what is misfiled, orphaned, or done in fact.
The document writer applies the rest as one reviewable change inside the given
scope.

Work on: $ARGUMENTS
