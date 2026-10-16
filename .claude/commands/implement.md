---
description: "Make failing tests green inside the boundary the recorded design drew, model first, then service, then presentation — nothing more."
argument-hint: "<item id or concern path>"
---

Convene these roles as the body directs:

- `domain`, whose contract is `.claude/agents/domain.md`
- `implementer`, whose contract is `.claude/agents/implementer.md`
- `ui`, whose contract is `.claude/agents/ui.md`
- `database`, whose contract is `.claude/agents/database.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.claude/skills/test-from-scenarios/SKILL.md`), `deliver-change` (`.claude/skills/deliver-change/SKILL.md`).

The implementation lead integrates the change against the recorded design and
failing tests, filling service and integration code behind the declared interface
stubs. When business rules change, the domain modeller builds assigned
entities and invariants; when persistence is affected, the database specialist
supplies assigned queries, storage code, migrations or backfills and evidence.
The presentation engineer builds the decided interaction when presentation is
affected. Each contribution stays bounded by its claims and design, with suites
and regression checks run before handoff. Each contributor commits citing the
claim it satisfies.

Work on: $ARGUMENTS
