---
description: "Implement claimed behavior behind failing tests, or carry out a behavior-preserving refactor using the recorded entry evidence, with bounded specialist contributions."
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
behavior-change or refactor evidence. Behavior changes fill service and
integration code behind the declared interface stubs. When the domain model is
affected, the domain modeller builds assigned
entities and invariants; when persistence is affected, the database specialist
supplies assigned queries, storage code, migrations or backfills and evidence.
The presentation engineer builds the decided interaction when presentation is
affected. Each contribution stays bounded by its item and design, with suites
and regression checks run before handoff. Each contributor commits citing the
claim it satisfies or the constraints its refactor preserves.

Work on: $ARGUMENTS
