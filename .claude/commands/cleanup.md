---
description: "Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change."
argument-hint: "[scope]"
---

Convene these roles, in this order:

- `organizer`, whose contract is `.claude/agents/organizer.md`
- `document-writer`, whose contract is `.claude/agents/document-writer.md`
- `architect`, whose contract is `.claude/agents/architect.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`), `deliver-change` (`.claude/skills/deliver-change/SKILL.md`).

The organizer finds what is misfiled or orphaned in the queue. The document
writer finds what is stale in the prose. The architect finds structure that has
drifted from the recorded design. Each reports in its own voice before anything
moves.

**Preconditions and halts.** Nothing changes outside the agreed scope, and no
behaviour changes at all; a finding that needs one becomes a work item.

Work on: $ARGUMENTS
