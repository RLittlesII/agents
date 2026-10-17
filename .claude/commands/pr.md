---
description: "Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it."
argument-hint: "<item id, branch or pull request>"
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`
- `implementer`, whose contract is `.claude/agents/implementer.md`
- `spec-author`, whose contract is `.claude/agents/spec-author.md`
- `architect`, whose contract is `.claude/agents/architect.md`
- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `document-writer`, whose contract is `.claude/agents/document-writer.md`
- `build`, whose contract is `.claude/agents/build.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.claude/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The spec reviewer runs the diff pass and routes findings to their artifact
owners. Relevant owners make the bounded corrections; the implementation lead
coordinates the response and integrates their returned revisions. The reviewer
re-reads each correction. The implementation lead
then runs the local gates, pushes, opens the pull request citing the claims the
commits satisfy, and works each round of review feedback: a commit per thread,
each thread resolved with a reply naming only that commit, a disputed finding
answered in its thread and left open. The build engineer joins only when a
gate itself is wrong.

Work on: $ARGUMENTS
