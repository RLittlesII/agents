---
name: test
description: "Turn agreed claims into failing scenarios and tests, then run and explore the change and report what ran, what passed and what failed."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The test owner writes failing step definitions and failing tests for each
claim's scenario in scope and records them in the traceability matrix. The
verification pass then runs the suites and explores beyond them, and reports the exact
failing line of anything that fails.

Work on: the request that invoked this skill
