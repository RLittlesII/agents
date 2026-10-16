---
name: design
description: Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it.
roles: [architect, domain, test-writer, benchmarker, critic]
skills: [spec-and-traceability, coding-conventions, test-from-scenarios]
argument-hint: "<Feature specification path>"
---

The architect records the concern separation and the technical design, with
diagrams and a readiness verdict. When the claims carry business rules, the
domain modeller places each invariant where it cannot be bypassed. The test
writer reads the design for testability and records the verdict, before any
test depends on it. When the design states a cost, the benchmarker measures it
and records the number in the performance strategy. The critic then reads the
design and returns findings; the architect answers each in the design or
records why not, and the critic re-reads what changed.

Work on: $ARGUMENTS
