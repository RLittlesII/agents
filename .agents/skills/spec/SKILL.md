---
name: spec
description: "Take a raw ask to a decided need, then to the agreement sections and the scenario file of a Feature specification."
---

Act as the `spec-author` role, whose contract is `.codex/agents/spec-author.toml`. Read it before acting, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.agents/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The specification author preserves the supplied requirements source and
provenance, then checks it for ambiguity, conflict, feasibility, atomicity and
testability using the declared clarification guidance. Surface owned blocking
questions before deriving criteria. In discovery mode, establish who wants the
change, what failure state it removes, settled answers and material unknowns.
Once the asker confirms the recorded need and blocking questions have
authoritative answers, the author writes the agreement and scenarios.
The output is a specification in the repository.

Work on: the request that invoked this skill
