---
name: implement
description: "Make failing tests green inside the boundary the recorded design drew, model first, then service, then presentation — nothing more."
---

Convene these roles as the body directs:

- `domain`, whose contract is `.codex/agents/domain.toml`
- `implementer`, whose contract is `.codex/agents/implementer.toml`
- `ui`, whose contract is `.codex/agents/ui.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

Work in the order the change needs and skip a role whose tier it does not
touch. The domain modeller builds the entities and invariants the tests pin.
The implementer fills the service and integration code behind the interface
stubs. The UI engineer builds the presentation the decided interaction calls
for. Each runs the suites and the regression check, then commits citing the
claim, before handing on.

Work on: the request that invoked this skill
