---
description: "Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson the fix reveals."
argument-hint: "<the report, or a path to it>"
---

Convene these roles as the body directs:

- `bug-finder`, whose contract is `.claude/agents/bug-finder.md`
- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `implementer`, whose contract is `.claude/agents/implementer.md`
- `database`, whose contract is `.claude/agents/database.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `test-from-scenarios` (`.claude/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`), `deliver-change` (`.claude/skills/deliver-change/SKILL.md`).

The bug finder reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. Given several reports, it ranks
them and retains unattributed reports as unresolved, with the evidence gaps.
When the report
carries a log, the finder reads it first for the sequence and the decisive line. The test writer turns the reproduction into a test that
fails for that cause. The implementer makes it pass and commits the fix with its
lesson when the fix reveals a gap; a fix that reveals nothing writes none.

When persistence is affected, the database specialist supplies read-only
investigation evidence, then bounded persistence changes only against the
accepted cause, recorded design and failing tests. The implementation lead
integrates the correction.

Work on: $ARGUMENTS
