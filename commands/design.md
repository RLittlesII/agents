---
name: design
description: Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it.
roles: [architect, domain, database, observability, test-writer, benchmarker, spec-reviewer, spec-author]
skills: [spec-and-traceability, coding-conventions, test-from-scenarios, technology-decisions]
argument-hint: "<Feature specification path>"
---

The architect records the concern separation and the technical design, with
diagrams and a readiness verdict. When the claims carry business rules, the
domain modeller advises where each invariant belongs. When persistence is
affected, the database specialist supplies storage, transaction and migration
advice for the architect to record. When the optional observability subsection
exists, the signal owner authors its declarations; absence does not require
creating it or invoking a specialist. The test writer reads the design for
testability and records the verdict, before any test depends on it. When the
design states a cost, the benchmarker measures the available code or records
why it is not yet measurable in the performance strategy. The spec reviewer then
reads the design and returns findings; the architect answers each in the design
or records why not, and the reviewer re-reads what changed. The specification
author records the returned verdict in sign-off; interface scaffolding stays
with the architect.

Work on: $ARGUMENTS
