---
name: design-ui
description: "Decide how a screen or flow behaves — need, interaction, every state and the exact words — before its components are built."
---

Convene these roles, in this order:

- `analyst`, whose contract is `.codex/agents/analyst.toml`
- `ux`, whose contract is `.codex/agents/ux.toml`
- `ui`, whose contract is `.codex/agents/ui.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.agents/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The analyst confirms whose need the screen or flow serves and what failure it
removes. The UX designer decides the flow, every state including each failure
and empty state, accessibility and the copy. The UI engineer then builds the
components and the state behind them, reusing before adding.

**Preconditions and halts.** The interaction is decided before anything is
built, and every failure state is drawn. An undecided state halts the build and
goes back to the UX designer.

Work on: the request that invoked this skill
