---
name: ship
description: Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation.
roles: [spec-reviewer, adversary, auditor, release]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or version>"
---

The spec reviewer judges the diff against the claims it cites. The adversary
attacks what the release exposes. The auditor reads the repository at rest for
drift. The release engineer then versions, writes the changelog and stages the
rollout, with the rollback stated.

**Preconditions and halts.** A blocking finding from any pass halts the release.
A release is outward-facing and hard to undo: nothing publishes without a
stated rollback and the person's explicit confirmation.

Work on: $ARGUMENTS
