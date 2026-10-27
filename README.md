---
title: agents
description: Specification-driven development roles and method skills, defined once and projected into host-native files for Claude Code and Codex
type: overview
---

# agents

Nineteen role contracts, eight method skills and twelve commands for
specification-driven development, written once, host-free, and projected into
the files each AI host reads. The repository carries no product technology and
no tool-specific knowledge: a consuming repository adds a companion skill that
names its own paths, commands and stack.

## The method

A Feature starts as a specification: twelve numbered sections plus Tasks and
Scoring, with a companion `.feature` file whose scenarios carry the ids of the
claims they document. Each section has exactly one owning role. The current lead
integrates bounded specialist contributions against the recorded artifacts. The
[`specht`](https://github.com/RLittlesII/specht) tool checks the layout, the
ids and the traceability matrix in CI. The shared facts are in
[`sdd-conventions`](skills/sdd-conventions/SKILL.md).

## Roles

The [role index](agents/README.md) names the six lead responsibilities, thirteen
conditional specialists, and the routes for common work. A lead integrates the
complete result while specialists work within their own artifact boundaries.
Its [Handoffs section](agents/README.md#handoffs) answers who is involved and
when; [PERSONAS.md](agents/PERSONAS.md) lists each voice and optional signing name.

## Skills

| Skill                                                              | Covers                                                            |
| ------------------------------------------------------------------ | ----------------------------------------------------------------- |
| [`sdd-conventions`](skills/sdd-conventions/SKILL.md)               | the method's shared facts: workflow, section ownership, id schemes, spec layout, the companion contract |
| [`spec-and-traceability`](skills/spec-and-traceability/SKILL.md)   | the specification model, claims, records, blast radius            |
| [`clarify-requirements`](skills/clarify-requirements/SKILL.md)     | when to ask versus decide, and writing the answer back            |
| [`technology-decisions`](skills/technology-decisions/SKILL.md)   | stack, packages, architecture and UI agreed with the human before implementation |
| [`deliver-change`](skills/deliver-change/SKILL.md)                 | work item to branch to specification to pull request              |
| [`test-from-scenarios`](skills/test-from-scenarios/SKILL.md)       | a claim first, injected time, synthetic fixtures, every tier owed |
| [`coding-conventions`](skills/coding-conventions/SKILL.md)         | orient, keep the design direct, say it once                       |
| [`next`](skills/next/SKILL.md)                                     | Feature comparisons, completion paths, and next actionable work   |

## Commands

A command names its role and skill dependencies in source frontmatter. Native
command files bind the roles; generated skills follow repository artifact
ownership and review boundaries without agent references. The shared body states
the work sequence. Type `/<name>` in Claude Code or `$<name>` in Codex.

| Command                               | Convenes                                  | For                                                  |
| ------------------------------------- | ----------------------------------------- | ---------------------------------------------------- |
| [`spec`](commands/spec.md)            | spec-author, spec-reviewer                | a raw ask taken to a written agreement               |
| [`design`](commands/design.md)        | architect, domain, database, observability, test-writer, benchmarker, spec-reviewer, spec-author | a design, reviewed before anything is built on it |
| [`design-ui`](commands/design-ui.md)  | spec-author, ux                           | a screen or flow decided before it is built          |
| [`test`](commands/test.md)            | test-writer, tester                       | failing tests from claims, then a run report         |
| [`implement`](commands/implement.md)  | domain, implementer, ui, database         | behavior changes and refactors integrated against their entry evidence |
| [`review`](commands/review.md)        | spec-reviewer, adversary                  | a diff judged against its claims                     |
| [`pr`](commands/pr.md)                | spec-reviewer, conditional adversary, implementer, relevant artifact owners, build | required reviews, then a pull request opened and followed to merge |
| [`ship`](commands/ship.md)            | spec-reviewer, adversary, auditor, observability, database, benchmarker, release | the final gate, then a confirmed release |
| [`bug`](commands/bug.md)              | bug-finder, test-writer, implementer, database | a failure reproduced, pinned, fixed and learned from |
| [`groom`](commands/groom.md)          | organizer                                 | the queue ranked, an item filed, or its health       |
| [`document`](commands/document.md)    | document-writer                           | documents repaired against the repository            |
| [`cleanup`](commands/cleanup.md)      | auditor, organizer, document-writer       | a tidying pass, delivered as one change              |

## Consuming

Install the complete setup from a checkout at the version you want with
Python 3.11+ and either Skillfile or Microsoft apm on PATH:

```sh
python3 /path/to/agents/install/install.py --host claude --manager skillfile --target /path/to/project
python3 /path/to/agents/install/install.py --host codex --manager apm --target /path/to/project
```

Both managers work with both hosts. The [installation guide](install/README.md)
covers Windows, installed paths, conflicts and updates. The consumer needs no
.NET SDK; the installer uses committed projections.

For individual Claude artifacts, use the
[`skillfile`](https://github.com/eljulians/skillfile) CLI:

```sh
skillfile add github agent rlittlesii/agents .claude/agents/architect.md v0.1.0 --name architect
skillfile add github skill rlittlesii/agents skills/sdd-conventions/SKILL.md v0.1.0
```

Use the installer for Codex TOML agents and complete command dependencies. Write
your repository's companion skill, `<project>-conventions`, covering the areas
listed in [`sdd-conventions` § Your companion](skills/sdd-conventions/SKILL.md#your-companion).
The role files name no path or command of yours; the companion does.

## Layout

| Kind       | Paths                                                                  | Rule                                        |
| ---------- | ---------------------------------------------------------------------- | ------------------------------------------- |
| Source     | `agents/*.md`, `skills/*/SKILL.md`, `commands/*.md`, `hosts/*.yaml`, `scripts/*.fsx` | edit here, in a pull request                |
| Installation | `install/install.py`, `install/test_install.py`, `install/.spec/` | project-scoped consumer installer, offline tests and its agreement |
| Generated  | `.claude/agents/`, `.claude/skills/`, `.claude/commands/`, `.codex/agents/`, `.agents/skills/`, the ownership table and review-pass registry in `sdd-conventions`, the roster in `agents/PERSONAS.md` | never hand-edit; run the projector |
| Tool-owned | `.specht/`                                                              | written by `dotnet specht init`             |
| Work items | `.issue/`, `<area>/.issue/`                                          | one `<id>-<slug>.yml` per item; schema in `.spec/templates/item.yml` |
| Working notes | `.spec/working/` | topic-named discovery notes and confirmed needs before items; no duplicate specification |
| Scratch | `.artifacts/scratch/` | ignored, topic-scoped experiments and generated logs; retain portable evidence in owned records |
| Records    | `epics/`, `.spec/adr/`, `scripts/.spec/`, `agents/.spec/`, `commands/.spec/`, `skills/.spec/` | the method applied to this repository       |

## Maintaining

Projection and specification checks require the pinned .NET SDK in `global.json`;
installer tests also require Python 3.11+. CI restores Specht with its temporary
`GITHUB_TOKEN` and read-only package permission. Local restores need a GitHub
Packages credential, exported as the environment variable
the [specht setup guide](https://github.com/RLittlesII/specht/blob/main/docs/setup.md)
names.

```sh
dotnet tool restore
dotnet specht --strict                   # specification check
dotnet fsi scripts/project.fsx           # regenerate every host output
dotnet fsi scripts/project.fsx --check   # exit 1 on drift; what CI runs
dotnet fsi scripts/project.tests.fsx     # claim-named tests
python3 -m unittest discover -s install -p 'test_*.py' -v
```

### Testing

The repository adopts an **80% pooled changed executable production-line
coverage floor** under [ADR-0007](.spec/adr/0007-the-companion-declares-the-coverage-threshold.md).
[Repository verification](.github/.spec/README.md) defines the source inventory,
comparison and failure contract; its versioned `.github/coverage/` policy and
shared gate enforce that contract locally and in CI.

From an activated Python 3.11+ virtual environment, install the pinned tooling
and run the shared gate against the base branch:

```sh
python -m pip install -r .github/coverage/requirements.txt
python .github/coverage/run.py --base origin/main
```

The gate resolves the comparison merge base and measures current tracked edits
and non-ignored untracked sources. It collects F# coverage from test-only
assemblies linked to original scripts and Python coverage from offline suites;
production entrypoints stay unchanged. Missing, stale or mismapped evidence
fails before evaluation. A valid zero denominator reports N/A, never 100%.
Reports and build outputs stay under ignored `.artifacts/coverage/`.
The F# build adapter validates checkout, output and Python interpreter paths
before invoking its shell-based build target; [the gate](.github/coverage/run.py)
defines the supported path characters. Unsupported paths fail before collection.
Production text must use UTF-8 with LF or CRLF line endings so Git and collector
line identities agree; ambiguous separators fail before evaluation.

To add a role: write `agents/<name>.md` with the keys the
[role contract](agents/.spec/README.md) requires, add it to each host overlay
under `hosts/`, and run the projector. To change a role's model or effort on
one host, edit that host's overlay only. To add a command: write `commands/<name>.md`
with the keys the [commands specification](commands/.spec/README.md) requires
and run the projector. The decisions behind this shape are
in [`.spec/adr/`](.spec/adr/).

### Working records and release guidance

Before work items exist, record discovery notes and the confirmed need in a
topic-named Markdown file under `.spec/working/`, with the documentation
frontmatter. Once an agreement is written, link it from the note; claims and
material decisions live in their owning specification records. After an item
exists, its `decisions` log holds ongoing working and ownership evidence.

Bounded experiments and generated logs go under ignored `.artifacts/scratch/`
with a topic-specific directory. Retain portable conclusions in the owned
specification or item before removing that scratch directory when the experiment
ends. Scratch output is not a delivered artifact or a recorded approval.

This repository has no established release or recovery automation. Committed
projections and the chosen Git ref supply installation bytes. A release
assignment must first identify its version, changelog location, targets and
publish/recovery procedure; missing procedure is a named gap, not an invented
command. The existing explicit confirmation requirement still governs publishing.

### Delivery status

Work is tracked in local `.issue/` items, not GitHub issues. An item that
delivers a Feature sits in that Feature's `<area>/.issue/`; any other sits in
the root `.issue/`. Ids share one number space with `epics/`, and
`.issue/.sequence` holds the last id claimed: take the next and write it back in
the same commit. The fields, the status vocabulary and the rank definition are in
[`.spec/templates/item.yml`](.spec/templates/item.yml). Set `status: in-progress`
before the branch for delivery work; its `done` and `closed:` land when the
pull request that delivers the item merges. A Feature coordination record
closes after its required child delivery and active-claim completion are
verified; it is not an additional implementation item or occupied child lane.

Ownership is recorded as dated, append-only events in each item's `decisions`
log. A claim names the human or agent session (with a durable session reference),
branch and checkout; a release names its recorded owner; a reassignment names the
authorizing person or delegated authority and both owners. Each handoff records
completed work, remaining work and verification evidence or gaps. Apply
[`deliver-change` § Resume, release and reassignment](skills/deliver-change/SKILL.md#resume-release-and-reassignment)
before continuation. Existing taken items without that evidence need an explicit
ownership reconciliation; status alone identifies no owner. A release records the
resulting `ready` or `blocked` state with its reason; a reassignment retains the
occupied state. These edits do not set `done`.

The Feature coordination item is the unique `type: feature` record whose `spec`
names that specification's repository-relative path. Child task, test, bug and
refactor records never supply the Feature's mirrored `status`. Missing or
multiple coordination records are diagnostics; existing tasks are not silently
retyped or adopted as the source.

During tracker maintenance, update the coordination item's `status` first and
copy that exact value into its specification's `status` in the same change.
This is synchronization, not a separately authored delivery decision. Validate
the specification afterwards; `spec_status` retains its separate owner. Queue
recommendations perform no synchronization. The portable contract is in
[`sdd-conventions` § The spec / work-item split](skills/sdd-conventions/SKILL.md#the-spec--work-item-split).
