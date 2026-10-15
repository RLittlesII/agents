---
name: ship
description: "Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation."
---

Convene these roles, in this order:

- `spec-reviewer`, whose contract is `.codex/agents/spec-reviewer.toml`
- `adversary`, whose contract is `.codex/agents/adversary.toml`
- `auditor`, whose contract is `.codex/agents/auditor.toml`
- `release`, whose contract is `.codex/agents/release.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The spec reviewer judges the diff against the claims it cites. The adversary
attacks what the release exposes. The auditor reads the repository at rest for
drift. The release engineer then versions, writes the changelog and stages the
rollout, with the rollback stated.

**Preconditions and halts.** A blocking finding from any pass halts the release.
A release is outward-facing and hard to undo: nothing publishes without a
stated rollback and the person's explicit confirmation.

Work on: the request that invoked this skill
