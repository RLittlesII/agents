---
description: "Take a raw ask to a decided need, then to the agreement sections and the scenario file of a Feature specification."
argument-hint: "<the raw ask, or a path to notes>"
---

Act as the `spec-author` role, whose contract is `.claude/agents/spec-author.md`. Read it before acting, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.claude/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The specification author explores the ask in discovery mode: who wants it,
what failure state it removes, settled answers and material unknowns. Once the
asker confirms the recorded need, the author writes the agreement and scenarios.
The output is a specification in the repository.

Work on: $ARGUMENTS
