---
name: ship
description: "Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation."
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.codex/agents/spec-reviewer.toml`
- `adversary`, whose contract is `.codex/agents/adversary.toml`
- `auditor`, whose contract is `.codex/agents/auditor.toml`
- `metric-nerd`, whose contract is `.codex/agents/metric-nerd.toml`
- `database`, whose contract is `.codex/agents/database.toml`
- `benchmarker`, whose contract is `.codex/agents/benchmarker.toml`
- `release`, whose contract is `.codex/agents/release.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The spec reviewer judges the diff against the claims it cites. The adversary
joins when the release touches a trust boundary, a credential, a permission or
input from outside; say which, or that it touches none. The auditor reads the
repository at rest for drift. When an operational decision needs evidence, the
measurement specialist
states signal requirements, the baseline and the comparison to read after
rollout. When persistence is affected, the database specialist supplies
compatibility and verified rollback or recovery evidence. The benchmarker joins
only for a written performance, cost or structural concern. The release engineer then
versions, writes the changelog and stages the rollout, with the rollback
stated.

Work on: the request that invoked this skill
