---
description: "Judge a diff against the claims and decisions it cites, and add a hostile pass when it touches a boundary; findings ranked most severe first."
argument-hint: "<diff, branch or path>"
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`
- `adversary`, whose contract is `.claude/agents/adversary.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`), `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`).

The independent reviewer reads the identified diff against every claim it cites
and every accepted decision it touches. The surface pass follows when the diff
changes a trust boundary, a credential, an input from outside, or a permission;
say which of those it touches, or that it touches none. A diff finding never
delays or suppresses the surface pass.

Work on: $ARGUMENTS
