---
name: implement
description: Make failing tests green inside the boundary the recorded design drew, model first, then service, then presentation — nothing more.
roles: [domain, implementer, ui, database]
skills: [coding-conventions, test-from-scenarios, deliver-change]
argument-hint: "<item id or concern path>"
---

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
