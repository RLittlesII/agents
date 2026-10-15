---
name: design
description: Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it.
roles: [architect, domain, test-writer, critic]
skills: [spec-and-traceability, coding-conventions, test-from-scenarios]
argument-hint: "<Feature specification path>"
---

The architect records the concern separation and the technical design, with
diagrams and a readiness verdict. When the claims carry business rules, the
domain modeller places each invariant where it cannot be bypassed. The test
writer reads the design for testability and records the verdict, before any
test depends on it. The critic then reads the design and returns findings, one
per line, severity first; the architect answers each in the design or records
why not.

**Preconditions and halts.** The agreement sections exist and are settled. A
requirement gap halts the design and goes to the agreement owner; a decision
larger than the Feature goes to the person. A design the test writer finds
untestable goes back to the architect before the critic reads it.

Work on: $ARGUMENTS
