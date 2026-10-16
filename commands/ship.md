---
name: ship
description: Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation.
roles: [spec-reviewer, adversary, auditor, benchmarker, release]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or version>"
---

The spec reviewer judges the diff against the claims it cites. The adversary
joins when the release touches a trust boundary, a credential, a permission or
input from outside; say which, or that it touches none. The auditor reads the
repository at rest for drift. The benchmarker states the baseline and what to
read after the rollout to tell whether it helped. The release engineer then
versions, writes the changelog and stages the rollout, with the rollback
stated.

Work on: $ARGUMENTS
