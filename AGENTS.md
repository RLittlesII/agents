---
title: agents
description: Entry point to the role contracts, method skills, specifications and generated files of this repository
type: instructions
---

# AGENTS.md

This repository defines specification-driven development roles and skills and
projects them into host-native files. [README.md](README.md) describes the
purpose; the links below route work to its owning guidance.

## Setup

- [Maintaining](README.md#maintaining) — the SDK, the checker and the projector.
- [Consuming](README.md#consuming) — how another repository pulls an output.

## Skills

Read [sdd-conventions](skills/sdd-conventions/SKILL.md) with the skill for the task.

| Task                                              | Owning skill                                                       |
| ------------------------------------------------- | ------------------------------------------------------------------ |
| Deliver a change, commit or open a pull request   | [deliver-change](skills/deliver-change/SKILL.md)                   |
| Choose the next work                              | [next](skills/next/SKILL.md)                                       |
| Write code or documentation; author a skill       | [coding-conventions](skills/coding-conventions/SKILL.md)           |
| Write or review tests                             | [test-from-scenarios](skills/test-from-scenarios/SKILL.md)         |
| Resolve requirement ambiguity                     | [clarify-requirements](skills/clarify-requirements/SKILL.md)       |
| Author or review specifications and traceability  | [spec-and-traceability](skills/spec-and-traceability/SKILL.md)     |

This repository has no companion skill of its own beyond `sdd-conventions`;
its paths and commands are in [README.md](README.md).

**One-way rule.** A role may name skills; a skill never names a role. Say who by
section owner or review pass instead. Projection fails on a role name under
`skills/` outside a generated block ([0001-F4](skills/.spec/README.md)).

## Roles

[Role contracts and handoffs](agents/README.md). A role file is host-free; the
model, effort and tools a host uses are in [`hosts/`](hosts/). The name each role
signs its prose with is in [`agents/PERSONAS.md`](agents/PERSONAS.md).

## Commands

[`commands/`](commands/) holds prompts that convene roles; the README lists
them. A command names roles and skills in frontmatter only.

## Specifications

- [Epic 0001](epics/0001-sdd-roles/epic.md) — the roles, skills and commands, and the Features below.
- [Projection](scripts/.spec/README.md) — `0001-F1`, the projector.
- [Role contract](agents/.spec/README.md) — `0001-F2`, what a role file declares.
- [Commands](commands/.spec/README.md) — `0001-F3`, prompts that convene roles.
- [Skill boundary](skills/.spec/README.md) — `0001-F4`, the one-way rule.
- [Decisions](.spec/adr/) — repository-wide ADRs.

## Specification-driven development

- [Specification model](skills/spec-and-traceability/SKILL.md).
- [Workflow](skills/sdd-conventions/SKILL.md#workflow).
- [Section ownership](skills/sdd-conventions/SKILL.md#section-ownership).
- [Review passes](skills/sdd-conventions/SKILL.md#review-passes).
- [ID schemes](skills/sdd-conventions/SKILL.md#id-schemes).
- [Spec layout and the checker](skills/sdd-conventions/SKILL.md#spec-layout).

## Generated files

Never hand-edit `.claude/`, `.codex/`, `.agents/`, or the blocks between the
generated markers in `skills/sdd-conventions/SKILL.md` and `agents/PERSONAS.md`.
Edit the source under `agents/`, `skills/`, `commands/` or `hosts/` and run
`dotnet fsi scripts/project.fsx`. CI runs the same command with `--check` and
fails on drift.

## Code conventions

| Concern                       | Owner                                                          |
| ----------------------------- | -------------------------------------------------------------- |
| SDK                           | [global.json](global.json)                                     |
| Local tools                   | [.config/dotnet-tools.json](.config/dotnet-tools.json)         |
| Package feeds                 | [nuget.config](nuget.config)                                   |
| Specification sections and ids | [.specht/manifest.json](.specht/manifest.json)                |
| Specification blanks          | [.specht/templates/](.specht/templates/)                       |
| Work items                    | [.spec/templates/item.yml](.spec/templates/item.yml)       |
| Projector design              | [Projection specification § 7](scripts/.spec/README.md#7-technical-design) |
| Continuous integration        | [.github/workflows/ci.yml](.github/workflows/ci.yml)           |

## SDLC

```yaml
sdlc:
  github_mode: false
```
