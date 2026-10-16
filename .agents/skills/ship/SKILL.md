---
name: ship
description: "Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation."
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.codex/agents/spec-reviewer.toml`
- `adversary`, whose contract is `.codex/agents/adversary.toml`
- `auditor`, whose contract is `.codex/agents/auditor.toml`
- `benchmarker`, whose contract is `.codex/agents/benchmarker.toml`
- `release`, whose contract is `.codex/agents/release.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The spec reviewer judges the diff against the claims it cites. The adversary
joins when the release touches a trust boundary, a credential, a permission or
input from outside; say which, or that it touches none. The auditor reads the
repository at rest for drift. The benchmarker states the baseline and what to
read after the rollout to tell whether it helped. The release engineer then
versions, writes the changelog and stages the rollout, with the rollback
stated.

Work on: the request that invoked this skill
