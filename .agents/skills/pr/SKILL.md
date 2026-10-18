---
name: pr
description: "Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The independent reviewer runs the diff pass and routes findings to their artifact
owners. Relevant owners make the bounded corrections; the implementation lead
coordinates the response and integrates their returned revisions. The reviewer
re-reads each correction. The implementation lead
then runs the local gates, pushes, opens the pull request citing the claims the
commits satisfy, and works each round of review feedback: a commit per thread,
each thread resolved with a reply naming only that commit, a disputed finding
answered in its thread and left open. The build owner joins only when a
gate itself is wrong.

Work on: the request that invoked this skill
