---
title: "Specification: Commands"
description: "A composable prompt written once under commands/ convenes roles and names skills, and lands as a Claude command and a Codex skill."
type: feature
id: "F3"
epic: "0001"
spec_status: approved
status: in-progress
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
updated: "2026-10-17"
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

<!-- last written by: spec-author, 2026-10-17 -->

| ID    | Claim                                                                                                                                                                                                                         | Source   | Status |
| ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ------ |
| B-001 | Given a command and a host whose `commands_output` is a command file, this Feature emits that file with the description and argument hint, then a generated preamble naming each role's contract and each skill at that host's paths, then the body verbatim. | A-1      | Active |
| B-002 | Given a command and a host whose `commands_output` ends in `/SKILL.md`, this Feature emits a skill with the name and description, the same preamble at that host's paths, and the body with `$ARGUMENTS` replaced by the host's `commands_arguments`. | ADR-0005 | Withdrawn |
| B-003 | Given a command whose `roles` names a role with no `agents/<role>.md`, this Feature fails naming the command and the role, and writes nothing.                                                                                  | Brief    | Active |
| B-004 | Given a command whose `skills` names a skill with no `skills/<name>/SKILL.md`, this Feature fails naming the command and the skill, and writes nothing.                                                                          | Brief    | Active |
| B-005 | Given a command whose `name` differs from its file name, whose description is missing or more than one line, whose `roles` is empty, or whose body links into `agents/` or `skills/`, this Feature fails naming the command. | C-2      | Active |
| B-006 | Given a host without `commands_output`, this Feature projects no command for it; given a file in a commands output directory that no command produces, `--check` reports it stale.                                          | 0001-F1 B-008 | Active |
| B-007 | Given a command with the name of a skill directory, this Feature fails naming both, because on a host that projects commands as skills the two would share one place.                                                        | ADR-0005 | Active |
| B-008 | Given a command projected as a skill, the output names its skills and uses repository artifact ownership and independent review instructions instead of named-agent bindings or agent contract paths, and substitutes the invocation text for `$ARGUMENTS`. | PR #9 review 5560928256 | Active |

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
| 5   | Documentation inside `spec`                  | The specification is the agreement's only document; a README or guide changes with the code that makes it true, not before |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-14 -->

| #   | Concern                                                   | Classification |
| --- | --------------------------------------------------------- | -------------- |
| 1   | Which roles a piece of work convenes, in what order       | Business       |
| 2   | Where each host reads a command and how it takes the request | Technical   |
| 3   | Resolving role and skill names to each host's paths       | Technical      |
| 4   | Validating names against the roster and the skills        | Both           |

## 7. Technical Design

<!-- last written by: architect, 2026-10-15 -->

A command is `commands/<name>.md`: frontmatter `name`, `description`,
`roles`, optional `skills` and `argument-hint`; a body that says how the
convened roles divide the work — a halt is a role's own — ending with the request
placeholder `$ARGUMENTS`. `loadCommand`, `validateCommands`, `commandPreamble`
and `renderCommand` in [`Projection.fsx`](../../scripts/Projection.fsx) are the
authority; `plan` runs them after the roles and skills load, so a failure
writes nothing.

- **The preamble is generated per host.** A role's contract is the host's
  agent output for that role (`.claude/agents/<role>.md`,
  `.codex/agents/<role>.toml`); a skill is the host's skills output. These paths
  exist wherever the outputs were pulled, which `agents/<role>.md` does not.
  Command-file outputs retain the role-contract preamble. Skill outputs instead
  direct the reader to repository artifact ownership and review boundaries and
  list only skill dependencies; they never embed an agent roster or contract path.
  The command body uses section owners and review passes rather than role names.
- **The output form follows the path.** A `commands_output` ending in
  `/SKILL.md` renders a skill; any other renders a command file. The decision
  and the Codex finding are in
  [ADR-0005](../../.spec/adr/0005-commands-project-to-claude-commands-and-codex-skills.md).
- **The commands directory is an output root**, so a command that is removed
  leaves a stale file `--check` reports. On Codex the root is the skills
  directory, which is one already.
- **Source commands remain separate from method skills.** Role bindings in
  command frontmatter support native command files. Projected skill text follows
  the one-way boundary and contains no instructional agent binding or agent
  contract reference; B-005 fenced examples remain literal text without routing authority.

B-005 retains the existing inline-link guard and also parses the command body
with the existing Markdown dependency. Resolved reference-link destinations into
`../agents/` or `../skills/` fail with the command path before output writes.
Full, collapsed and shortcut references share this check; fenced reference
examples are not resolved links. Existing inline rejection and valid rendering
remain unchanged. Regression cases exercise planning and public CLI failure.

Readiness: **Ready for regression tests after independent correction-design approval.**

### Role-free command skills (PR #9)

B-008 replaces B-002. `renderCommand` already decides the output form from
`commands_output`; use that same predicate in `commandPreamble`. For a skill,
emit repository ownership/review guidance and skill dependencies only. For a
native command file, retain the existing generated role-contract preamble.
The body remains shared and verbatim apart from invocation substitution; source
bodies describe artifact owners and review passes without naming agent roles.
B-005's fenced examples remain literal text, not agent-routing instructions.
No new host key, role lookup, runtime configuration or dependency is needed.

Tests compare complete one-role/no-skill and multi-role/with-skill outputs, verify
that a changed agent output path cannot affect skill bytes, and check every real
projected command skill for roster names and agent paths. Existing command-file
expectations prove that native convening still works. Readiness: Ready for
independent agreement/design review before dependent tests and implementation.

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-17 -->

The `F3_` group in `dotnet fsi scripts/project.tests.fsx`, on the synthetic
checkout of 0001-F1 with `commands_output` set on both hosts and one command,
`convene`, naming both roles and the companion skill. The scenarios in
[`commands.feature`](commands.feature) are executed by those tests.

- B-001, B-008: whole Claude command file and whole Codex skill compared, one
  multi-role and one single-role command.
- B-003, B-004, B-007: `plan` raises `ProjectionError` naming the offender and
  no output directory exists; B-003 also validates the real `commands/`.
- B-005: one case per malformation, including resolved full/collapsed/shortcut
  reference destinations in both source namespaces. Planning and CLI cases reject
  before writes; a fenced-reference control preserves valid body text.
- B-006: the base fixture, whose hosts lack `commands_output`, projects no
  command; a planted file in `.claude/commands/` is reported stale.
- C-1 and C-3 are review questions, read with each new command.

### Performance

Not applicable.

PR #9 B-008 correction: the 88-test baseline passed. All three new B-008
controls failed against the prior renderer (90 tests, three expected failures).
After correction, the source-bound gate passed 90 F# tests, six CLI cases,
30 installer tests and 36 coverage-gate tests. Coverage run
`3db83d8ed9a44c81a5b5556ef7c304b3` measured 98/99 changed executable production
lines (98.99%; required 80%) against main `d4eac9c`; the existing unexecuted
source traversal fallback is unchanged. Two test compatibility corrections
narrowed a path check to agent directories and updated the earlier role-rename
skill assertion while retaining native command and agent-file migration checks.
These are local execution results, not reported GitHub CI or live-host evidence.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-17 -->

| Claim ID | Scenario                                         | Test                                                     | Status  |
| -------- | ------------------------------------------------ | -------------------------------------------------------- | ------- |
| B-001    | A command projects to a Claude command file      | `F3_B001_CommandProjectsToClaudeCommandFile`             | Covered |
| B-002 | A command projects to a Codex skill | Replaced by B-008 after PR #9 review | Withdrawn |
| B-003    | A command names a role with no file              | `F3_B003_CommandNamingMissingRole_FailsNamingBoth`       | Covered |
| B-004    | A command names a skill with no directory        | `F3_B004_CommandNamingMissingSkill_FailsNamingBoth`      | Covered |
| B-005    | A malformed command fails                        | `F3_B005_MalformedCommand_Fails`; `F3_B005_FullReferenceRoleLinkFails`; `F3_B005_FullReferenceSkillLinkFails`; `F3_B005_CollapsedReferenceRoleLinkFails`; `F3_B005_ShortcutReferenceRoleLinkFails`; `F3_B005_FencedReferenceExampleRemainsVerbatim` | Covered |
| B-006    | Commands follow the host and stale ones show     | `F3_B006_CommandsFollowTheHostAndStaleOnesAreReported`   | Covered |
| B-007    | A command named like a skill fails               | `F3_B007_CommandNamedLikeSkill_Fails`                    | Covered |
| B-008 | A command skill follows artifact ownership without agent references | `F3_B008_CommandSkillUsesOwnershipWithoutAgentBindings`; `F3_B008_AgentOutputPathCannotChangeCommandSkill`; `F3_B008_RepositoryCommandSkillsNameNoAgent` | Covered |

## 10. Lessons / Spec Deltas

2026-10-17: [lesson 0001](lessons/0001-resolve-markdown-reference-destinations.md)
records the inline-only guard failure and resolved-destination correction. Four
reference regressions failed before implementation and pass afterward; final
source-bound execution evidence remains linked from Projection §8.

2026-10-17: baseline content approved; B-005 reference-link rejection requires a
bounded implementation correction, now implemented and independently reviewed.
The existing claim and constraint are unchanged; regression evidence and the
returned approvals are recorded in Projection §8 and this Feature’s §12.

<!-- last written by: spec-author, 2026-10-17 -->

2026-10-17: Reconcile command division of work with existing artifact authority
under C-1. PR corrections return to their owners; cleanup applies documentation
corrections and tracks findings outside the convened owners' boundaries. Bug
investigation retains unattributed reports; design permits an unmeasured finding
when no code is available. Implementation selects the agreed refactor evidence
under 0001-F2 B-042. The spec command includes independent agreement review;
design includes the sign-off owner to record its returned verdict before the
design owner supplies stubs. Projection format and host mappings remain unchanged.

2026-10-17, PR #9 review 5560928256: retire B-002 in favor of B-008;
command skills follow artifact ownership without agent references. Native command
files retain their role bindings. The affected coordinator is 0050; audit 0046
continues the correction. ADR-0011 supersedes ADR-0005 while preserving its locations and invocation rules.

[Lesson 0002](lessons/0002-projected-skills-keep-one-way-boundary.md) records the
command-source exception incorrectly carried into a generated skill.

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-14 -->

None.

## 12. Sign-off

<!-- last written by: spec-author, 2026-10-17 -->

| Section | Status | Reviewer      | Note                                                                    |
| ------- | ------ | ------------- | ----------------------------------------------------------------------- |
| Diff: §§ 10, 12 and command corrections | 🟢 | spec-reviewer | Independent `/root/contract_design_diff_review`, 2026-10-17, approved subject `36ab103e778d070d3ca8291ab56c63a0b96991bf6c656fb5c176ffce63785d5b` after both retirement/history findings were resolved; no remaining findings. |
| Baseline: 1-12 and scenarios | 🟢 | spec-reviewer | Independent `/root/baseline_projection_commands`, 2026-10-17: retrospective baseline content Approved at `2997ac3c6dbdf3bef910ffb6fef86aecc19d0bed`. Content approval; B-005 reference-link validation remains an implementation finding. No historical pre-implementation approval is claimed. |
| Correction: §7 B-005 reference links and scenarios | 🟢 | spec-reviewer | Independent `/root/baseline_projection_commands`, 2026-10-17: Approved at `a2c1215ccec340282a9e75c4ba2e014946624db8f49c59513c03562bc247e7e3`; unchanged baseline approval remains applicable. |
| Diff: B-005 reference-link correction and evidence | 🟢 | spec-reviewer | Independent `/root/baseline_projection_commands`, 2026-10-17: Approved production, regression and recording scope at `a148bf47a5a726c87dbf4908547bcc27c6cfe56c70dfb7b7b95a2ca33ab629ed`; subsequent incident citations independently Approved, completed 84-test execution and full gate confirmed. |
| Agreement/design: B-008 and role-free command skills | 🟢 | spec-reviewer | Independent `/root/pr9_review`, 2026-10-17, Approved composite `bcf82372e7d69ff36d3158922f62ea033cf16d2a94f7a6730d18a009e8ac3564` before tests/implementation. Clarification of inert fenced examples independently Approved at `0a701167dfdfcc07dbe64d421eb2365dc3cbd16cf12d3144c6464668cdc66301` before the rerun; no production change. Earlier approvals retain unchanged scope. |
| Diff: PR #9 B-008 correction | 🟢 | spec-reviewer | Independent `/root/pr9_review`, 2026-10-17, Approved baseline `c7e3a9d` plus correction patch SHA-256 `e9a277dcb696e1f08e01210ab091a2e0c87c293fd0134a9eaa81eb8a729eb227`, manifest `1dbc35023731704b2e2c547b092ffdc732987bcb65aae9410259588f6208f111`. Renderer, source commands, projections, lesson and 90-test/98-of-99 coverage evidence approved. Overall PR remains blocked on F5 B-017/B-046 Missing evidence. |
| Surface: PR #9 instruction/projection boundary | 🟢 | adversary | Independent `/root/pr9_surface`, 2026-10-17, Approved static boundary scope; projector SHA-256 `93aad58741378bda9f09b48c8d268d1608c6f2eda9ab0cbb30a750dfcc2e9cc9`, installer `8b7d93f191bf041d2df7140d849c6ef5f35dbb9177185ca6604ab3139d668473`. No actionable findings; documented concurrency/manager exclusions remain. |

### Superseded amendment review history

Prior baseline record, superseded by the returned baseline content verdict:

> | 1-12    | 🟡     | spec-reviewer | Draft. The twelve commands read for C-1 and C-3: no transitions, no technology. |

These pending records predate the current independent verdicts. They retain
their original notes; current live-agent evidence limits remain in § 8.

- Command routing corrections, 2026-10-17 — 🟡 — spec-reviewer — Authored static conformance check only; independent review pending.

Current amendment verdicts apply to the identified reviewed snapshot. Earlier
pending rows are retained for their earlier subjects; current review approval
does not establish live-agent behavior or unperformed platform/manager runs.

## Tasks

<!-- last written by: organizer, 2026-10-17 -->

- [0050 — Coordinate Commands delivery](../.issue/0050-coordinate-commands.yml) — authoritative delivery-status source.
- [0025 — Add the implement, release and design-ui commands](../.issue/0025-implement-release-design-ui.yml).
- [0026 — Widen the roles that design, spec and bug convene](../.issue/0026-command-role-lists.yml).
- [0027 — Decide the pull-request, backlog and single-role commands](../.issue/0027-pr-backlog-commands.yml).

## Scoring

| Field | Value | Basis                                                |
| ----- | ----- | ---------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
