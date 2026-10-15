---
name: test
description: Turn agreed claims into failing scenarios and tests, then run and explore the change and report what ran, what passed and what failed.
roles: [test-writer, tester]
skills: [test-from-scenarios, spec-and-traceability]
argument-hint: "<claim ids, a Feature, or a change>"
---

The test writer cuts a failing scenario and failing tests for each claim in
scope and records them in the traceability matrix. The tester then runs the
suites and explores beyond them, and reports the exact failing line of anything
that fails.

**Preconditions and halts.** A claim with no recorded design halts the test
writer; it goes to the design owner. A skipped test is not a passing one.

Work on: $ARGUMENTS
