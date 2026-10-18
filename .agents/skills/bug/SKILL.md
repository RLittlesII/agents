---
name: bug
description: "Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson the fix reveals."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

The diagnostic pass reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. Given several reports, it ranks
them and retains unattributed reports as unresolved, with the evidence gaps.
When the report
carries a log, the diagnostic pass reads it first for the sequence and the decisive line. The test owner turns the reproduction into a test that
fails for that cause. The implementation lead makes it pass and commits the fix with its
lesson when the fix reveals a gap; a fix that reveals nothing writes none.

When persistence is affected, the storage specialist supplies read-only
investigation evidence, then bounded persistence changes only against the
accepted cause, recorded design and failing tests. The implementation lead
integrates the correction.

Work on: the request that invoked this skill
