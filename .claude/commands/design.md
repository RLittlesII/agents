---
description: "Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it."
argument-hint: "<Feature specification path>"
---

Convene these roles, in this order:

- `architect`, whose contract is `.claude/agents/architect.md`
- `domain`, whose contract is `.claude/agents/domain.md`
- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `benchmarker`, whose contract is `.claude/agents/benchmarker.md`
- `critic`, whose contract is `.claude/agents/critic.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`), `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.claude/skills/test-from-scenarios/SKILL.md`).

The architect records the concern separation and the technical design, with
diagrams and a readiness verdict. When the claims carry business rules, the
domain modeller places each invariant where it cannot be bypassed. The test
writer reads the design for testability and records the verdict, before any
test depends on it. When the design states a cost, the benchmarker measures it
and records the number in the performance strategy. The critic then reads the
design and returns findings; the architect answers each in the design or
records why not, and the critic re-reads what changed.

Work on: $ARGUMENTS
