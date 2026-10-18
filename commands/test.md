---
name: test
description: Turn agreed claims into failing scenarios and tests, then run and explore the change and report what ran, what passed and what failed.
roles: [test-writer, tester]
skills: [test-from-scenarios, spec-and-traceability]
argument-hint: "<claim ids, a Feature, or a change>"
---

The test owner writes failing step definitions and failing tests for each
claim's scenario in scope and records them in the traceability matrix. The
verification pass then runs the suites and explores beyond them, and reports the exact
failing line of anything that fails.

Work on: $ARGUMENTS
