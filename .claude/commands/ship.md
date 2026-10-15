---
description: "Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation."
argument-hint: "<item id, branch or version>"
---

Convene these roles, in this order:

- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`
- `adversary`, whose contract is `.claude/agents/adversary.md`
- `auditor`, whose contract is `.claude/agents/auditor.md`
- `release`, whose contract is `.claude/agents/release.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.claude/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The spec reviewer judges the diff against the claims it cites. The adversary
attacks what the release exposes. The auditor reads the repository at rest for
drift. The release engineer then versions, writes the changelog and stages the
rollout, with the rollback stated.

**Preconditions and halts.** A blocking finding from any pass halts the release.
A release is outward-facing and hard to undo: nothing publishes without a
stated rollback and the person's explicit confirmation.

Work on: $ARGUMENTS
