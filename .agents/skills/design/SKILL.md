---
name: design
description: "Design the technical solution for an agreed Feature, then have the design critiqued before any test or code is written against it."
---

Convene these roles, in this order:

- `architect`, whose contract is `.codex/agents/architect.toml`
- `domain`, whose contract is `.codex/agents/domain.toml`
- `test-writer`, whose contract is `.codex/agents/test-writer.toml`
- `critic`, whose contract is `.codex/agents/critic.toml`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`), `coding-conventions` (`.agents/skills/coding-conventions/SKILL.md`), `test-from-scenarios` (`.agents/skills/test-from-scenarios/SKILL.md`).

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

Work on: the request that invoked this skill
