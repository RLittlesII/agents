---
name: review
description: "Judge a diff against the claims and decisions it cites, and add a hostile pass when it touches a boundary; findings ranked most severe first."
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.codex/agents/spec-reviewer.toml`
- `adversary`, whose contract is `.codex/agents/adversary.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`), `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`).

The spec reviewer reads the diff against every claim it cites and every
accepted decision it touches. The adversary joins only when the diff changes a
trust boundary, a credential, an input from outside, or a permission; say which
of those it touches, or that it touches none.

Work on: the request that invoked this skill
