---
name: pr
description: "Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it."
---

Convene these roles, in this order:

- `spec-reviewer`, whose contract is `.codex/agents/spec-reviewer.toml`
- `implementer`, whose contract is `.codex/agents/implementer.toml`
- `build`, whose contract is `.codex/agents/build.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The spec reviewer runs the diff pass; the implementer answers each finding with
a commit, and the reviewer re-reads until the list is empty. The implementer
then runs the local gates, pushes, opens the pull request citing the claims the
commits satisfy, and works each round of review feedback: a commit per thread,
each thread resolved with a reply naming only that commit, a disputed finding
answered in its thread and left open. The build engineer joins only when a
gate itself is wrong.

Work on: the request that invoked this skill
