---
name: implement
description: Implement claimed behavior behind failing tests, or carry out a behavior-preserving refactor using the recorded entry evidence, with bounded specialist contributions.
roles: [domain, implementer, ui, database]
skills: [coding-conventions, test-from-scenarios, deliver-change]
argument-hint: "<item id or concern path>"
---

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
