---
description: "Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson."
argument-hint: "<the report, or a path to it>"
---

Convene these roles, in this order:

- `bug-finder`, whose contract is `.claude/agents/bug-finder.md`
- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `bug-fixer`, whose contract is `.claude/agents/bug-fixer.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `test-from-scenarios` (`.claude/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The bug finder reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. Given several reports, it ranks
them and drops any it cannot attribute, with the reason. The test writer turns the
reproduction into a test that fails for that cause. The bug fixer makes it pass
and writes the lesson.

**Preconditions and halts.** No fix before the reproduction is reliable, and no
close on a cause nobody proved. A failure that a claim does not cover goes to
the agreement owner before it is fixed.

Work on: $ARGUMENTS
