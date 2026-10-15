---
name: spec
description: "Take a raw ask to a decided need, then to the agreement sections and the scenario file of a Feature specification."
---

Convene these roles, in this order:

- `analyst`, whose contract is `.codex/agents/analyst.toml`
- `spec-author`, whose contract is `.codex/agents/spec-author.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `clarify-requirements` (`.agents/skills/clarify-requirements/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The analyst turns the ask into discovery notes: who wants it, what failure
state it removes, and what is still unknown. The spec author starts only from a
decided need and writes the agreement and its scenarios.

**Preconditions and halts.** Halt while the need still has two readings that
would build different systems; ask the one question that separates them. The
output is a specification in the repository, not a conversation: a decision
reached and not written did not happen.

Work on: the request that invoked this skill
