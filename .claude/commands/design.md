---
description: "Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it."
argument-hint: "<Feature specification path>"
---

Convene these roles as the body directs:

- `architect`, whose contract is `.claude/agents/architect.md`
- `domain`, whose contract is `.claude/agents/domain.md`
- `database`, whose contract is `.claude/agents/database.md`
- `observability`, whose contract is `.claude/agents/observability.md`
- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `benchmarker`, whose contract is `.claude/agents/benchmarker.md`
- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`), `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.claude/skills/test-from-scenarios/SKILL.md`), `technology-decisions` (`.claude/skills/technology-decisions/SKILL.md`).

The architect records the concern separation and the technical design, with
diagrams and a readiness verdict. When the claims carry business rules, the
domain modeller advises where each invariant belongs. When persistence is
affected, the database specialist supplies storage, transaction and migration
advice for the architect to record. When the optional observability subsection
exists, the signal owner authors its declarations; absence does not require
creating it or invoking a specialist. The test writer reads the design for
testability and records the verdict, before any test depends on it. When the design states a cost, the benchmarker measures it
and records the number in the performance strategy. The spec reviewer then
reads the design and returns findings; the architect answers each in the design
or records why not, and the reviewer re-reads what changed.

Work on: $ARGUMENTS
