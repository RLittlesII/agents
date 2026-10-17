---
name: implement
description: "Implement claimed behavior behind failing tests, or carry out a behavior-preserving refactor using the recorded entry evidence, with bounded specialist contributions."
---

Convene these roles as the body directs:

- `domain`, whose contract is `.codex/agents/domain.toml`
- `implementer`, whose contract is `.codex/agents/implementer.toml`
- `ui`, whose contract is `.codex/agents/ui.toml`
- `database`, whose contract is `.codex/agents/database.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

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

Work on: the request that invoked this skill
