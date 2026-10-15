---
name: pr
description: Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it.
roles: [spec-reviewer, build, implementer]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or pull request>"
---

The spec reviewer runs the diff pass first; its findings go back to the
implementer before anything is pushed. The build engineer then runs the local
gates, pushes, opens the pull request citing the claims the commits satisfy, and
watches the checks. For each round of review feedback, the implementer makes
the change, commits, pushes, and resolves each thread the commit addressed with
a reply naming only that commit; a disputed finding is answered in its thread
and left open.

**Preconditions and halts.** A clean working tree and green local gates. Halt
on a claim the change delivers whose traceability row is still `Missing`.
Merging is the person's call: never merge without their explicit approval.

Work on: $ARGUMENTS
