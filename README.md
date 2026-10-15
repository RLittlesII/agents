---
title: agents
description: Specification-driven development roles and method skills, defined once and projected into host-native files for Claude Code and Codex
type: overview
---

# agents

Twenty-four role contracts, seven method skills and eight commands for
specification-driven development, written once, host-free, and projected into
the files each AI host reads. The repository carries no product technology and
no tool-specific knowledge: a consuming repository adds a companion skill that
names its own paths, commands and stack.

## The method

A Feature starts as a specification: twelve numbered sections plus Tasks and
Scoring, with a companion `.feature` file whose scenarios carry the ids of the
claims they document. Each section has exactly one owning role. Each role
trusts only the artifact the preceding role produced. The
[`specht`](https://github.com/RLittlesII/specht) tool checks the layout, the
ids and the traceability matrix in CI. The shared facts are in
[`sdd-conventions`](skills/sdd-conventions/SKILL.md).

## Roles

| Role                                           | Turns                                   | Into                                                      |
| ---------------------------------------------- | --------------------------------------- | --------------------------------------------------------- |
| [`analyst`](agents/analyst.md)                 | a raw ask                               | discovery notes and a decided need                        |
| [`spec-author`](agents/spec-author.md)         | a decided need                          | the agreement, and the `.feature` file                    |
| [`organizer`](agents/organizer.md)             | agreed claims                           | ranked work items and the Tasks index                     |
| [`architect`](agents/architect.md)             | the authored agreement and scenarios    | a technical design, diagrams and readiness findings       |
| [`domain`](agents/domain.md)                   | business rules in claims                | a domain model whose invariants cannot be bypassed        |
| [`ux`](agents/ux.md)                           | user needs and claims                   | a decided interaction: flows, states and copy             |
| [`critic`](agents/critic.md)                   | a design before it is built             | findings, never a redesign                                |
| [`test-writer`](agents/test-writer.md)         | a claim                                 | a failing scenario and failing tests                      |
| [`implementer`](agents/implementer.md)         | a failing test and recorded design      | production code, and design feedback                      |
| [`ui`](agents/ui.md)                           | a decided interaction and failing tests | presentation code                                         |
| [`pair-programmer`](agents/pair-programmer.md) | a task the person wants to steer        | small visible steps and a decision record                 |
| [`tester`](agents/tester.md)                   | a change to verify                      | a run report and reproductions                            |
| [`spec-reviewer`](agents/spec-reviewer.md)     | a diff                                  | a sign-off, or findings                                   |
| [`adversary`](agents/adversary.md)             | the surface a change exposes            | security findings                                         |
| [`benchmarker`](agents/benchmarker.md)         | a stated cost concern                   | a benchmark and a recorded finding                        |
| [`metric-nerd`](agents/metric-nerd.md)         | a decision that needs evidence          | a baseline and a comparison                               |
| [`bug-finder`](agents/bug-finder.md)           | a failure report                        | a reproduction and a root-cause record                    |
| [`bug-fixer`](agents/bug-fixer.md)             | a diagnosed cause and a failing test    | the fix and its lesson                                    |
| [`log-parser`](agents/log-parser.md)           | a long log                              | the sequence and the decisive lines                       |
| [`yak-shaver`](agents/yak-shaver.md)           | an incidental obstacle                  | the obstacle removed, and what was left alone             |
| [`build`](agents/build.md)                     | a broken or missing gate                | a reproducible build and gates that fail when they should |
| [`release`](agents/release.md)                 | an artifact that passed the gates       | a confirmed release with a rollback                       |
| [`document-writer`](agents/document-writer.md) | stale or missing prose                  | documents checked against what exists                     |
| [`auditor`](agents/auditor.md)                 | the repository at rest                  | drift findings, grouped by rule                           |

The handoff rules are in [`agents/README.md`](agents/README.md). Each role signs
the prose it writes with a persona; the roster is in
[`agents/PERSONAS.md`](agents/PERSONAS.md).

## Skills

| Skill                                                              | Covers                                                            |
| ------------------------------------------------------------------ | ----------------------------------------------------------------- |
| [`sdd-conventions`](skills/sdd-conventions/SKILL.md)               | the method's shared facts: workflow, section ownership, id schemes, spec layout, the companion contract |
| [`spec-and-traceability`](skills/spec-and-traceability/SKILL.md)   | the specification model, claims, records, blast radius            |
| [`clarify-requirements`](skills/clarify-requirements/SKILL.md)     | when to ask versus decide, and writing the answer back            |
| [`deliver-change`](skills/deliver-change/SKILL.md)                 | work item to branch to specification to pull request              |
| [`test-from-scenarios`](skills/test-from-scenarios/SKILL.md)       | a claim first, injected time, synthetic fixtures, both tiers      |
| [`coding-conventions`](skills/coding-conventions/SKILL.md)         | orient, keep the design direct, say it once                       |
| [`next`](skills/next/SKILL.md)                                     | Feature comparisons, completion paths, and next actionable work   |

## Commands

A command convenes roles in an order and names the skills they work with; the
roles' contracts decide what each does. Type `/<name>` in Claude Code or
`$<name>` in Codex.

| Command                               | Convenes                                  | For                                                  |
| ------------------------------------- | ----------------------------------------- | ---------------------------------------------------- |
| [`spec`](commands/spec.md)            | analyst, spec-author                      | a raw ask taken to a written agreement               |
| [`design`](commands/design.md)        | architect, critic                         | a design, critiqued before anything is built on it   |
| [`test`](commands/test.md)            | test-writer, tester                       | failing tests from claims, then a run report         |
| [`review`](commands/review.md)        | spec-reviewer, adversary                  | a diff judged against its claims                     |
| [`bug`](commands/bug.md)              | bug-finder, test-writer, bug-fixer        | a failure reproduced, pinned, fixed and learned from |
| [`groom`](commands/groom.md)          | organizer                                 | the queue re-scored and ranked                       |
| [`document`](commands/document.md)    | document-writer                           | documents repaired against the repository            |
| [`cleanup`](commands/cleanup.md)      | organizer, document-writer, architect     | a tidying pass, delivered as one change              |

## Consuming

Outputs are committed, so a consumer pulls a finished file by path at a tagged
ref. With the [`skillfile`](https://github.com/eljulians/skillfile) CLI:

```sh
skillfile add github agent rlittlesii/agents .claude/agents/architect.md v0.1.0 --name architect
skillfile add github skill rlittlesii/agents skills/sdd-conventions/SKILL.md v0.1.0
```

Pull a Codex agent from `.codex/agents/<role>.toml` the same way, and a command
from `.claude/commands/<name>.md` or `.agents/skills/<name>/SKILL.md`; a command
names its roles and skills at those same paths, so pull them with it. Then write
your repository's companion skill, `<project>-conventions`, covering the areas
listed in [`sdd-conventions` § Your companion](skills/sdd-conventions/SKILL.md#your-companion).
The role files name no path or command of yours; the companion does.

## Layout

| Kind       | Paths                                                                  | Rule                                        |
| ---------- | ---------------------------------------------------------------------- | ------------------------------------------- |
| Source     | `agents/*.md`, `skills/*/SKILL.md`, `commands/*.md`, `hosts/*.yaml`, `scripts/*.fsx` | edit here, in a pull request                |
| Generated  | `.claude/agents/`, `.claude/skills/`, `.claude/commands/`, `.codex/agents/`, `.agents/skills/`, the ownership table and review-pass registry in `sdd-conventions`, the roster in `agents/PERSONAS.md` | never hand-edit; run the projector |
| Tool-owned | `.specht/`                                                              | written by `dotnet specht init`             |
| Records    | `.spec/adr/`, `scripts/.spec/`, `agents/.spec/`, `commands/.spec/`    | the method applied to this repository       |

## Maintaining

The pinned .NET SDK in `global.json` is the only prerequisite. The `specht`
feed needs a GitHub Packages credential, exported as the environment variable
the [specht setup guide](https://github.com/RLittlesII/specht/blob/main/docs/setup.md)
names.

```sh
dotnet tool restore
dotnet specht --strict                   # specification check
dotnet fsi scripts/project.fsx           # regenerate every host output
dotnet fsi scripts/project.fsx --check   # exit 1 on drift; what CI runs
dotnet fsi scripts/project.tests.fsx     # claim-named tests
```

To add a role: write `agents/<name>.md` with the keys the
[role contract](agents/.spec/README.md) requires, add it to each host overlay
under `hosts/`, and run the projector. To change a role's model or effort on
one host, edit that host's overlay only. To add a command: write `commands/<name>.md`
with the keys the [commands specification](commands/.spec/README.md) requires
and run the projector. The decisions behind this shape are
in [`.spec/adr/`](.spec/adr/).
