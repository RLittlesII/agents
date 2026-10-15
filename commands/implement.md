---
name: implement
description: Make failing tests green inside the boundary the recorded design drew, model first, then service, then presentation — nothing more.
roles: [domain, implementer, ui]
skills: [coding-conventions, test-from-scenarios]
argument-hint: "<item id or concern path>"
---

Work in the order the change needs and skip a role whose tier it does not
touch. The domain modeller builds the entities and invariants the tests pin.
The implementer fills the service and integration code behind the interface
stubs. The UI engineer builds the presentation the decided interaction calls
for. Each runs the suites and the regression check before handing on.

**Preconditions and halts.** A recorded design marked Ready, its interface
stubs, and failing tests for the cited claims. Halt if any is missing: a
missing design or stub goes to the design owner, a missing test to the test
owner. A needed signature change halts and goes back to the design owner.

Work on: $ARGUMENTS
