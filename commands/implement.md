---
name: implement
description: Make failing tests green inside the boundary the recorded design drew, model first, then service, then presentation — nothing more.
roles: [domain, implementer, ui]
skills: [coding-conventions, test-from-scenarios, deliver-change]
argument-hint: "<item id or concern path>"
---

Work in the order the change needs and skip a role whose tier it does not
touch. The domain modeller builds the entities and invariants the tests pin.
The implementer fills the service and integration code behind the interface
stubs. The UI engineer builds the presentation the decided interaction calls
for. Each runs the suites and the regression check, then commits citing the
claim, before handing on.

Work on: $ARGUMENTS
