---
name: design-ui
description: "Decide how a screen or flow behaves — need, interaction, every state and the exact words — before its components are built."
---

Convene these roles as the body directs:

- `spec-author`, whose contract is `.codex/agents/spec-author.toml`
- `ux`, whose contract is `.codex/agents/ux.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.agents/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The specification author confirms whose need the screen or flow serves and
what failure it removes. The interaction designer decides the flow, every
state including failure and empty states, accessibility and copy, and records
the decided interaction where the build will read it.

Work on: the request that invoked this skill
