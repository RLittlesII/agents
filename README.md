---
title: agents
description: Specification-driven development roles and method skills, defined once and projected into host-native files for Claude Code and Codex
type: overview
---

# agents

Six role contracts and seven method skills for specification-driven
development, written once, host-free, and projected into the files each AI host
reads. The repository carries no product technology and no tool-specific
knowledge: a consuming repository adds a companion skill that names its own
paths, commands and stack.

## The method

A Feature starts as a specification: twelve numbered sections plus Tasks and
Scoring, with a companion `.feature` file whose scenarios carry the ids of the
claims they document. Each section has exactly one owning role. Each role
trusts only the artifact the preceding role produced. The
[`specht`](https://github.com/RLittlesII/specht) tool checks the layout, the
ids and the traceability matrix in CI. The shared facts are in
[`sdd-conventions`](skills/sdd-conventions/SKILL.md).

## Roles

| Role                                         | Turns                                | Into                                                |
| -------------------------------------------- | ------------------------------------ | --------------------------------------------------- |
| [`spec-author`](agents/spec-author.md)       | a decided need                       | the agreement, and the `.feature` file              |
| [`architect`](agents/architect.md)           | the authored agreement and scenarios | a technical design, diagrams and readiness findings |
| [`test-writer`](agents/test-writer.md)       | a claim                              | a failing scenario and failing tests                |
| [`implementer`](agents/implementer.md)       | a failing test and recorded design   | production code, and design feedback                |
| [`spec-reviewer`](agents/spec-reviewer.md)   | a diff                               | a sign-off, or findings                             |
| [`benchmarker`](agents/benchmarker.md)       | a stated cost concern                | a benchmark and a recorded finding                  |

The handoff rules are in [`agents/README.md`](agents/README.md).

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

## Consuming

Outputs are committed, so a consumer pulls a finished file by path at a tagged
ref. With the [`skillfile`](https://github.com/eljulians/skillfile) CLI:

```sh
skillfile add github agent rlittlesii/agents .claude/agents/architect.md v0.1.0 --name architect
skillfile add github skill rlittlesii/agents skills/sdd-conventions/SKILL.md v0.1.0
```

Pull a Codex agent from `.codex/agents/<role>.toml` the same way. Then write
your repository's companion skill, `<project>-conventions`, covering the areas
listed in [`sdd-conventions` § Your companion](skills/sdd-conventions/SKILL.md#your-companion).
The role files name no path or command of yours; the companion does.

## Layout

| Kind       | Paths                                                                  | Rule                                        |
| ---------- | ---------------------------------------------------------------------- | ------------------------------------------- |
| Source     | `agents/*.md`, `skills/*/SKILL.md`, `hosts/*.yaml`, `scripts/*.fsx`     | edit here, in a pull request                |
| Generated  | `.claude/agents/`, `.claude/skills/`, `.codex/agents/`, `.agents/skills/`, the ownership table in `sdd-conventions` | never hand-edit; run the projector |
| Tool-owned | `.specht/`                                                              | written by `dotnet specht init`             |
| Records    | `.spec/adr/`, `scripts/.spec/`, `agents/.spec/`                         | the method applied to this repository       |

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
one host, edit that host's overlay only. The decisions behind this shape are
in [`.spec/adr/`](.spec/adr/).
