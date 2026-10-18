---
name: implement
description: "Implement claimed behavior behind failing tests, or carry out a behavior-preserving refactor using the recorded entry evidence, with bounded specialist contributions."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`), `deliver-change` (`.agents/skills/deliver-change/SKILL.md`).

The implementation lead integrates the change against the recorded design and
behavior-change or refactor evidence. Behavior changes fill service and
integration code behind the declared interface stubs. When the domain model is
affected, the business-rule owner builds assigned
entities and invariants; when persistence is affected, the storage specialist
supplies assigned queries, storage code, migrations or backfills and evidence.
The presentation owner builds the decided interaction when presentation is
affected. Each contribution stays bounded by its item and design, with suites
and regression checks run before handoff. Each contributor commits citing the
claim it satisfies or the constraints its refactor preserves.

Work on: the request that invoked this skill
