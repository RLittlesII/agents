---
description: "Decide how a screen or flow behaves — need, interaction, every state and the exact words — before its components are built."
argument-hint: "<the screen or flow>"
---

Convene these roles, in this order:

- `analyst`, whose contract is `.claude/agents/analyst.md`
- `ux`, whose contract is `.claude/agents/ux.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.claude/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The analyst confirms whose need the screen or flow serves and what failure it
removes. The UX designer decides the flow, every state including each failure
and empty state, accessibility and the copy, and records the decided
interaction where the build will read it.

Work on: $ARGUMENTS
