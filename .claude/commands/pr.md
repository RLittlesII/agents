---
description: "Take a finished change through the diff pass to an open pull request, and work each round of review feedback until the person merges it."
argument-hint: "<item id, branch or pull request>"
---

Convene these roles, in this order:

- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`
- `build`, whose contract is `.claude/agents/build.md`
- `implementer`, whose contract is `.claude/agents/implementer.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.claude/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

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
