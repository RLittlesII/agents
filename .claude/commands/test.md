---
description: "Turn agreed claims into failing scenarios and tests, then run and explore the change and report what ran, what passed and what failed."
argument-hint: "<claim ids, a Feature, or a change>"
---

Convene these roles, in this order:

- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `tester`, whose contract is `.claude/agents/tester.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `test-from-scenarios` (`.claude/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The test writer cuts a failing scenario and failing tests for each claim in
scope and records them in the traceability matrix. The tester then runs the
suites and explores beyond them, and reports the exact failing line of anything
that fails.

**Preconditions and halts.** A claim with no recorded design halts the test
writer; it goes to the design owner. A skipped test is not a passing one.

Work on: $ARGUMENTS
