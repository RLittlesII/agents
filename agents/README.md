---
title: Role contracts
description: The six documented role contracts that own the specification chain, and the rule that each role trusts only the artifact of the role before it
type: reference
unowned_spec_sections:
  "11": whoever is blocked
  Scoring: derived — recompute, never hand-edit
---

# Role contracts

A specification-driven repository treats the specification as the artifact of
record; code is downstream of it. Six roles own that chain, and each is a
**documented contract** — for whoever takes the role, a person or an agent.

| Role                                  | Turns                                | Into                                                       |
| ------------------------------------- | ------------------------------------ | ---------------------------------------------------------- |
| [`spec-author`](spec-author.md)       | a decided need                       | the agreement, and the `.feature` file                     |
| [`architect`](architect.md)           | the authored agreement and scenarios | a technical design, diagrams and readiness findings        |
| [`test-writer`](test-writer.md)       | a claim                              | a failing scenario and failing tests                       |
| [`implementer`](implementer.md)       | a failing test and recorded design   | production code, and design feedback                       |
| [`spec-reviewer`](spec-reviewer.md)   | a diff                               | a sign-off, or findings                                    |
| [`benchmarker`](benchmarker.md)       | a stated cost concern                | a benchmark and a recorded finding                         |

## Sequential trust

**Each role trusts only the artifact the preceding role produced — never a chat
summary of it.** The architect reads the groomed agreement and its scenarios
before tests and implementation begin. The test writer works from the claim and
the recorded design. The implementer works from that design, the failing test
and the claim it cites, not from a description of what the test is getting at. A
conversational handoff leaves no artifact and does not survive to the next
session or the next agent.

The authoritative sequence is
[`sdd-conventions` § Workflow](../skills/sdd-conventions/SKILL.md#workflow).

The architect records whether the design is ready for tests and implementation.
Requirement gaps return to `spec-author`; design or testability gaps from tests,
implementation or measurement return to `architect`. The architect reconciles
the design and diagrams before work proceeds against the revised design. Final
sign-off remains with `spec-reviewer`.

The architect also takes the first pass at technical spikes and records
performance concerns in the design; the benchmarker owns their measurements.

This is also why no role fills another's sections. Writing into someone else's
section removes the only signal that the preceding stage is incomplete.
A role that finds its input missing or wrong **escalates to the owning role**; it
does not quietly supply the gap.

## Where ownership is written

Which role owns which section is derived from each role's `owned_spec_sections`
and written in exactly one place:
[`sdd-conventions` § Section ownership](../skills/sdd-conventions/SKILL.md#section-ownership).
The two sections no single role owns are declared in this file's frontmatter.
Ownership is not restated in the role files — a second copy is a second thing to
drift.

## Standing notes

- **Small changes can combine roles; the ordering is mandatory.** A new or
  amended specification passes architecture review before tests and
  implementation. A one-line fix does not need a separate agent for every role;
  it still may not reach code before the claim it satisfies exists.
- **Scenarios execute.** The project's acceptance tier runs the `.feature`
  files. Unit tests still cover the concerns — a passing acceptance scenario is
  not a substitute for them, and § 9 cites both.
- **Every role writes less than it wants to.** Enough code to turn the test
  green; no abstraction without a second caller that exists today; one statement
  per fact. The rule is in
  [`coding-conventions`](../skills/coding-conventions/SKILL.md) §§ "Design" and
  "Say it once, and briefly" - each role file names only what that means for the
  artifact it owns.
- **The contracts are also installable agents.** `hosts/<host>.yaml` maps each
  role to a model and effort for that host, and `dotnet fsi scripts/project.fsx`
  writes the host-native files. Consumers pull the committed outputs with
  `skillfile`; see the repository `README.md`.
