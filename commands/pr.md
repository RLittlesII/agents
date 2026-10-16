---
name: pr
description: Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it.
roles: [spec-reviewer, implementer, build]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or pull request>"
---

The spec reviewer runs the diff pass; the implementer answers each finding with
a commit, and the reviewer re-reads until the list is empty. The implementer
then runs the local gates, pushes, opens the pull request citing the claims the
commits satisfy, and works each round of review feedback: a commit per thread,
each thread resolved with a reply naming only that commit, a disputed finding
answered in its thread and left open. The build engineer joins only when a
gate itself is wrong.

Work on: $ARGUMENTS
