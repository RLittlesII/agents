---
name: cleanup
description: "Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change."
---

Convene these roles, in this order:

- `organizer`, whose contract is `.codex/agents/organizer.toml`
- `document-writer`, whose contract is `.codex/agents/document-writer.toml`
- `architect`, whose contract is `.codex/agents/architect.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

The organizer finds what is misfiled or orphaned in the queue. The document
writer finds what is stale in the prose. The architect finds structure that has
drifted from the recorded design. Each reports in its own voice before anything
moves.

**Preconditions and halts.** Nothing changes outside the agreed scope, and no
behaviour changes at all; a finding that needs one becomes a work item.

Work on: the request that invoked this skill
