---
title: "Specification: Commands"
description: "A composable prompt written once under commands/ convenes roles and names skills, and lands as a Claude command and a Codex skill."
type: feature
id: "F3"
epic: "0001"
spec_status: draft
status: ready
priority: med
value: 0
risk: 0
rank: 0
scored_by: null
scored_on: null
domain: "Specification-driven development"
author: "spec-author"
milestone: null
children: []
depends_on: ["F1", "F2"]
blocks: []
spikes: []
created: "2026-10-14"
updated: "2026-10-14"
github_issue: null
synced_at: null
---

# Specification: Commands

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-14 -->

A person starts a recurring piece of work — review a diff, take an ask to a
specification, chase a bug — by typing one name on whichever host they use, and
the right roles are convened in the right order with the right skills. Without
commands, each person re-types the convening prompt from memory, names roles
that no longer exist, and two hosts drift into two versions of the same ritual.

## 2. User Needs

<!-- last written by: spec-author, 2026-10-14 -->

| #   | Persona              | Need                                                         | Pain Point Today                                               |
| --- | -------------------- | ------------------------------------------------------------ | -------------------------------------------------------------- |
| 1   | Person at a host     | Invoke a named piece of work with a request                  | The convening prompt is re-typed and varies per person          |
| 2   | Command author       | Write a command once and have both hosts read it             | One copy per host drifts                                        |
| 3   | Consuming repository | Pull commands by path that name the roles it also pulled     | A command naming a role path that only exists in this repository breaks there |
| 4   | Projection           | Know a command names only roles and skills that exist        | N/A (internal dependency on [0001-F1](../../scripts/.spec/README.md)) |

### Assumptions

| ID  | Assumption                                                                                                 |
| --- | ---------------------------------------------------------------------------------------------------------- |
| A-1 | Claude Code reads `.claude/commands/<name>.md` and replaces `$ARGUMENTS` with the text typed after `/<name>`. |
| A-2 | Codex reads no repository prompts and discovers skills at `.agents/skills/<name>/SKILL.md` ([ADR-0005](../../.spec/adr/0005-commands-project-to-claude-commands-and-codex-skills.md)). |

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-14 -->

| ID    | Claim                                                                                                                                                                                                                         | Source   | Status |
| ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ------ |
| B-001 | Given a command and a host whose `commands_output` is a command file, this Feature emits that file with the description and argument hint, then a generated preamble naming each role's contract and each skill at that host's paths, then the body verbatim. | A-1      | Active |
| B-002 | Given a command and a host whose `commands_output` ends in `/SKILL.md`, this Feature emits a skill with the name and description, the same preamble at that host's paths, and the body with `$ARGUMENTS` replaced by the host's `commands_arguments`. | ADR-0005 | Active |
| B-003 | Given a command whose `roles` names a role with no `agents/<role>.md`, this Feature fails naming the command and the role, and writes nothing.                                                                                  | Brief    | Active |
| B-004 | Given a command whose `skills` names a skill with no `skills/<name>/SKILL.md`, this Feature fails naming the command and the skill, and writes nothing.                                                                          | Brief    | Active |
| B-005 | Given a command whose `name` differs from its file name, whose description is missing or more than one line, whose `roles` is empty, or whose body links into `agents/` or `skills/`, this Feature fails naming the command. | C-2      | Active |
| B-006 | Given a host without `commands_output`, this Feature projects no command for it; given a file in a commands output directory that no command produces, `--check` reports it stale.                                          | 0001-F1 B-008 | Active |
| B-007 | Given a command with the name of a skill directory, this Feature fails naming both, because on a host that projects commands as skills the two would share one place.                                                        | ADR-0005 | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-14 -->

| ID  | Constraint                                                                                   | Rules Out                                                                 |
| --- | -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| C-1 | A command convenes roles and names skills; what each role does is in the role's contract.     | Transitions, gates or a next-state table in a command; restating a role's refusals |
| C-2 | A command names a role or a skill once, in frontmatter.                                       | A body link into `agents/` or `skills/`, which cannot resolve from both hosts |
| C-3 | A command names no product technology, path or build tool, as for a role (0001-F2 B-004).     | A command per technology; a command naming a consumer's paths             |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-14 -->

| #   | Item                                         | Exclusion Reason                                                                       |
| --- | -------------------------------------------- | -------------------------------------------------------------------------------------- |
| 1   | A per-role command that convenes one role and adds nothing | The role is already an agent each host can call by name, and its halts live in its contract |
| 2   | Command-to-command transitions               | A command is a prompt, not a state machine (C-1)                                         |
| 3   | Host-specific command keys such as allowed tools or a model | A command inherits the role's host mapping; added when a host needs one   |
| 4   | Grooming or design inside `spec`             | Items are cut after agreement (`groom`) and design follows it (`design`); convening either in `spec` would act before the agreement exists |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-14 -->

| #   | Concern                                                   | Classification |
| --- | --------------------------------------------------------- | -------------- |
| 1   | Which roles a piece of work convenes, in what order       | Business       |
| 2   | Where each host reads a command and how it takes the request | Technical   |
| 3   | Resolving role and skill names to each host's paths       | Technical      |
| 4   | Validating names against the roster and the skills        | Both           |

## 7. Technical Design

<!-- last written by: architect, 2026-10-14 -->

A command is `commands/<name>.md`: frontmatter `name`, `description`,
`roles`, optional `skills` and `argument-hint`; a body that says how the
convened roles divide the work and where it halts, ending with the request
placeholder `$ARGUMENTS`. `loadCommand`, `validateCommands`, `commandPreamble`
and `renderCommand` in [`Projection.fsx`](../../scripts/Projection.fsx) are the
authority; `plan` runs them after the roles and skills load, so a failure
writes nothing.

- **The preamble is generated per host.** A role's contract is the host's
  agent output for that role (`.claude/agents/<role>.md`,
  `.codex/agents/<role>.toml`); a skill is the host's skills output. These paths
  exist wherever the outputs were pulled, which `agents/<role>.md` does not.
  One role reads "Act as the `<role>` role"; several read "Convene these roles,
  in this order" with one line each.
- **The output form follows the path.** A `commands_output` ending in
  `/SKILL.md` renders a skill; any other renders a command file. The decision
  and the Codex finding are in
  [ADR-0005](../../.spec/adr/0005-commands-project-to-claude-commands-and-codex-skills.md).
- **The commands directory is an output root**, so a command that is removed
  leaves a stale file `--check` reports. On Codex the root is the skills
  directory, which is one already.
- **A command is not a skill.** `commands/` is a sibling of `skills/`; a lint
  over skills does not read it, even though Codex receives commands as skills.

Readiness: **Ready for tests and implementation.**

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-14 -->

The `F3_` group in `dotnet fsi scripts/project.tests.fsx`, on the synthetic
checkout of 0001-F1 with `commands_output` set on both hosts and one command,
`convene`, naming both roles and the companion skill. The scenarios in
[`commands.feature`](commands.feature) are executed by those tests.

- B-001, B-002: whole Claude command file and whole Codex skill compared, one
  multi-role and one single-role command.
- B-003, B-004, B-007: `plan` raises `ProjectionError` naming the offender and
  no output directory exists; B-003 also validates the real `commands/`.
- B-005: one case per malformation, each failing before anything is written.
- B-006: the base fixture, whose hosts lack `commands_output`, projects no
  command; a planted file in `.claude/commands/` is reported stale.
- C-1 and C-3 are review questions, read with each new command.

### Performance

Not applicable.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-14 -->

| Claim ID | Scenario                                         | Test                                                     | Status  |
| -------- | ------------------------------------------------ | -------------------------------------------------------- | ------- |
| B-001    | A command projects to a Claude command file      | `F3_B001_CommandProjectsToClaudeCommandFile`             | Covered |
| B-002    | A command projects to a Codex skill              | `F3_B002_CommandProjectsToCodexSkill`                    | Covered |
| B-003    | A command names a role with no file              | `F3_B003_CommandNamingMissingRole_FailsNamingBoth`       | Covered |
| B-004    | A command names a skill with no directory        | `F3_B004_CommandNamingMissingSkill_FailsNamingBoth`      | Covered |
| B-005    | A malformed command fails                        | `F3_B005_MalformedCommand_Fails`                         | Covered |
| B-006    | Commands follow the host and stale ones show     | `F3_B006_CommandsFollowTheHostAndStaleOnesAreReported`   | Covered |
| B-007    | A command named like a skill fails               | `F3_B007_CommandNamedLikeSkill_Fails`                    | Covered |

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-14 -->

None.

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-14 -->

None.

## 12. Sign-off

<!-- last written by: spec-reviewer, 2026-10-14 -->

| Section | Status | Reviewer      | Note                                                                 |
| ------- | ------ | ------------- | -------------------------------------------------------------------- |
| 1-12    | 🟡     | spec-reviewer | Draft. The eight commands read for C-1 and C-3: no transitions, no technology. |

## Tasks

None cut. The first eight commands land with this Feature; a new command is a
new file and needs no work item of its own.

## Scoring

| Field | Value | Basis                                                |
| ----- | ----- | ---------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
