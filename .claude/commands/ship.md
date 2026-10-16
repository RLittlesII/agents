---
description: "Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation."
argument-hint: "<item id, branch or version>"
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`
- `adversary`, whose contract is `.claude/agents/adversary.md`
- `auditor`, whose contract is `.claude/agents/auditor.md`
- `metric-nerd`, whose contract is `.claude/agents/metric-nerd.md`
- `release`, whose contract is `.claude/agents/release.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.claude/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The spec reviewer judges the diff against the claims it cites. The adversary
joins when the release touches a trust boundary, a credential, a permission or
input from outside; say which, or that it touches none. The auditor reads the
repository at rest for drift. The metric nerd states the baseline and what to
read after the rollout to tell whether it helped. The release engineer then
versions, writes the changelog and stages the rollout, with the rollback
stated.

Work on: $ARGUMENTS
