---
name: design
description: Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it.
roles: [architect, critic]
skills: [spec-and-traceability, coding-conventions]
argument-hint: "<Feature specification path>"
---

The architect records the concern separation and the technical design, with
diagrams and a readiness verdict. The critic then reads that design and returns
findings, one per line, severity first; the architect answers each in the
design or records why not.

**Preconditions and halts.** The agreement sections exist and are settled. A
requirement gap halts the design and goes to the agreement owner; a decision
larger than the Feature goes to the person.

Work on: $ARGUMENTS
