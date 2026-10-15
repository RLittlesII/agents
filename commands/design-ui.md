---
name: design-ui
description: Decide how a screen or flow behaves — need, interaction, every state and the exact words — before its components are built.
roles: [analyst, ux, ui]
skills: [clarify-requirements, spec-and-traceability]
argument-hint: "<the screen or flow>"
---

The analyst confirms whose need the screen or flow serves and what failure it
removes. The UX designer decides the flow, every state including each failure
and empty state, accessibility and the copy. The UI engineer then builds the
components and the state behind them, reusing before adding.

**Preconditions and halts.** The interaction is decided before anything is
built, and every failure state is drawn. An undecided state halts the build and
goes back to the UX designer.

Work on: $ARGUMENTS
