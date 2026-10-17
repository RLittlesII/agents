---
name: pr
description: Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it.
roles: [spec-reviewer, implementer, spec-author, architect, test-writer, document-writer, build]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or pull request>"
---

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
