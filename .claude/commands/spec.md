---
description: "Take a raw ask to a decided need, then to the agreement sections and the scenario file of a Feature specification."
argument-hint: "<the raw ask, or a path to notes>"
---

Convene these roles as the body directs:

- `analyst`, whose contract is `.claude/agents/analyst.md`
- `spec-author`, whose contract is `.claude/agents/spec-author.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.claude/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The analyst turns the ask into discovery notes: who wants it, what failure
state it removes, and what is still unknown. The spec author starts only from a
decided need and writes the agreement and its scenarios. The output is a
specification in the repository, not a conversation.

Work on: $ARGUMENTS
