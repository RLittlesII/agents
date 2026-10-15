---
title: Role contracts
description: The documented role contracts that own the specification chain, and the rule that each role trusts only the artifact of the role before it
type: reference
unowned_spec_sections:
  "11": whoever is blocked
review_units:
  design: a design, before implementation exists
  diff: a diff, against the claims and decisions it cites
  surface: what a change exposes to a hostile caller
  repository: the repository at rest, on a cadence or before a release
---

# Role contracts

A specification-driven repository treats the specification as the artifact of
record; code is downstream of it. Twenty-four roles work that chain, and each
is a **documented contract** — for whoever takes the role, a person or an agent.

| Role                                    | Turns                                   | Into                                                      |
| --------------------------------------- | --------------------------------------- | --------------------------------------------------------- |
| [`analyst`](analyst.md)                 | a raw ask                               | discovery notes and a decided need                        |
| [`spec-author`](spec-author.md)         | a decided need                          | the agreement, and the `.feature` file                    |
| [`organizer`](organizer.md)             | agreed claims                           | ranked work items and the Tasks index                     |
| [`architect`](architect.md)             | the authored agreement and scenarios    | a technical design, diagrams and readiness findings       |
| [`domain`](domain.md)                   | business rules in claims                | a domain model whose invariants cannot be bypassed        |
| [`ux`](ux.md)                           | user needs and claims                   | a decided interaction: flows, states and copy             |
| [`critic`](critic.md)                   | a design before it is built             | findings, never a redesign                                |
| [`test-writer`](test-writer.md)         | a claim                                 | a failing scenario and failing tests                      |
| [`implementer`](implementer.md)         | a failing test and recorded design      | production code, and design feedback                      |
| [`ui`](ui.md)                           | a decided interaction and failing tests | presentation code                                         |
| [`pair-programmer`](pair-programmer.md) | a task the person wants to steer        | small visible steps and a decision record                 |
| [`tester`](tester.md)                   | a change to verify                      | a run report and reproductions                            |
| [`spec-reviewer`](spec-reviewer.md)     | a diff                                  | a sign-off, or findings                                   |
| [`adversary`](adversary.md)             | the surface a change exposes            | security findings                                         |
| [`benchmarker`](benchmarker.md)         | a stated cost concern                   | a benchmark and a recorded finding                        |
| [`metric-nerd`](metric-nerd.md)         | a decision that needs evidence          | a baseline and a comparison                               |
| [`bug-finder`](bug-finder.md)           | a failure report                        | a reproduction and a root-cause record                    |
| [`bug-fixer`](bug-fixer.md)             | a diagnosed cause and a failing test    | the fix and its lesson                                    |
| [`log-parser`](log-parser.md)           | a long log                              | the sequence and the decisive lines                       |
| [`yak-shaver`](yak-shaver.md)           | an incidental obstacle                  | the obstacle removed, and what was left alone             |
| [`build`](build.md)                     | a broken or missing gate                | a reproducible build and gates that fail when they should |
| [`release`](release.md)                 | an artifact that passed the gates       | a confirmed release with a rollback                       |
| [`document-writer`](document-writer.md) | stale or missing prose                  | documents checked against what exists                     |
| [`auditor`](auditor.md)                 | the repository at rest                  | drift findings, grouped by rule                           |

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

Most roles own no section. The analyst works before the agreement exists; the
critic, the adversary and the auditor review; the tester, the bug finder, the log
parser and the metric nerd report. Each hands its artifact to the owner of the
section it concerns, by the same rule.

## Where ownership is written

Which role owns which section is derived from each role's `owned_spec_sections`
and written in exactly one place:
[`sdd-conventions` § Section ownership](../skills/sdd-conventions/SKILL.md#section-ownership).
The two sections no single role owns are declared in this file's frontmatter.
Ownership is not restated in the role files — a second copy is a second thing to
drift.

The critic, the spec reviewer, the adversary and the auditor each review one
unit of work, declared under `review_units` in this file's frontmatter and
claimed by the role's `reviews`. Which pass reviews what, and in what order, is in
[`sdd-conventions` § Review passes](../skills/sdd-conventions/SKILL.md#review-passes).

Each role also declares the persona it signs its prose with; the roster is
generated into [`PERSONAS.md`](PERSONAS.md).

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
