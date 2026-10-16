---
name: bug
description: "Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson the fix reveals."
---

Convene these roles as the body directs:

- `bug-finder`, whose contract is `.codex/agents/bug-finder.toml`
- `test-writer`, whose contract is `.codex/agents/test-writer.toml`
- `implementer`, whose contract is `.codex/agents/implementer.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

The bug finder reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. Given several reports, it ranks
them and drops any it cannot attribute, with the reason. When the report
carries a log, the finder reads it first for the sequence and the decisive line. The test writer turns the reproduction into a test that
fails for that cause. The implementer makes it pass and commits the fix with its
lesson when the fix reveals a gap; a fix that reveals nothing writes none.

Work on: the request that invoked this skill
