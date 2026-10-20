---
name: pr
description: "Take a finished change through required review passes to an open pull request, and work each round of feedback until the person merges it."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The implementation lead supplies one identified review bundle. Run the diff pass
before the conditional surface pass when a boundary, credential, permission or
external input changes. Artifact owners address findings; reviewers re-read
corrections. After local gates, push, open the pull request and handle feedback
under the delivery rule.

Work on: the request that invoked this skill
