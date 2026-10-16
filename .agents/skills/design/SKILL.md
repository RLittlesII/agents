---
name: design
description: "Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it."
---

Convene these roles as the body directs:

- `architect`, whose contract is `.codex/agents/architect.toml`
- `domain`, whose contract is `.codex/agents/domain.toml`
- `database`, whose contract is `.codex/agents/database.toml`
- `observability`, whose contract is `.codex/agents/observability.toml`
- `test-writer`, whose contract is `.codex/agents/test-writer.toml`
- `benchmarker`, whose contract is `.codex/agents/benchmarker.toml`
- `spec-reviewer`, whose contract is `.codex/agents/spec-reviewer.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`), `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`).

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

Work on: the request that invoked this skill
