---
name: bug
description: "Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson."
---

Convene these roles, in this order:

- `bug-finder`, whose contract is `.codex/agents/bug-finder.toml`
- `test-writer`, whose contract is `.codex/agents/test-writer.toml`
- `bug-fixer`, whose contract is `.codex/agents/bug-fixer.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The bug finder reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. The test writer turns the
reproduction into a test that fails for that cause. The bug fixer makes it pass
and writes the lesson.

**Preconditions and halts.** No fix before the reproduction is reliable, and no
close on a cause nobody proved. A failure that a claim does not cover goes to
the agreement owner before it is fixed.

Work on: the request that invoked this skill
