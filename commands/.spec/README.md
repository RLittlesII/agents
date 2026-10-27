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
updated: "2026-10-27"
github_issue: null
synced_at: null
---

# Specification: Commands

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-27 -->

A person invokes recurring work by one name on either host, convening the
required roles and skills from one prompt instead of divergent copies.

## 2. User Needs

<!-- last written by: spec-author, 2026-10-27 -->

| # | Persona | Need | Pain Point Today |
| --- | -------------------- | ------------------------------------------------------------ | -------------------------------------------------------------- |
| 1 | Person at a host | Invoke a named piece of work with a request | The convening prompt is re-typed and varies per person |
| 2 | Command author | Write a command once and have both hosts read it | One copy per host drifts |
| 3 | Consuming repository | Pull commands by path that name the roles it also pulled | A command naming a role path that only exists in this repository breaks there |
| 4 | Projection | Know a command names only roles and skills that exist | N/A (internal dependency on 0001-F1) |

### Assumptions

| ID | Assumption |
| --- | ---------------------------------------------------------------------------------------------------------- |
| A-1 | Claude Code reads `.claude/commands/<name>.md` and replaces `$ARGUMENTS` with the text typed after `/<name>`. |
| A-2 | Codex reads no repository prompts and discovers skills at `.agents/skills/<name>/SKILL.md` ([ADR-0005](../../.spec/adr/0005-commands-project-to-claude-commands-and-codex-skills.md)). |

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-27 -->

| ID | Claim | Source | Status |
| ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ------ |
| B-001 | Given a command and a host whose `commands_output` is a command file, this Feature emits that file with the description and argument hint, then a generated preamble naming each role's contract and each skill at that host's paths, then the body verbatim. | A-1 | Active |
| B-002 | Given a command and a host whose `commands_output` ends in `/SKILL.md`, this Feature emits a skill with the name and description, the same preamble at that host's paths, and the body with `$ARGUMENTS` replaced by the host's `commands_arguments`. | ADR-0005 | Withdrawn |
| B-003 | Given a command whose `roles` names a role with no `agents/<role>.md`, this Feature fails naming the command and the role, and writes nothing. | Brief | Active |
| B-004 | Given a command whose `skills` names a skill with no `skills/<name>/SKILL.md`, this Feature fails naming the command and the skill, and writes nothing. | Brief | Active |
| B-005 | Given a command whose `name` differs from its file name, whose description is missing or more than one line, whose `roles` is empty, or whose body links into `agents/` or `skills/`, this Feature fails naming the command. | C-2 | Active |
| B-006 | Given a host without `commands_output`, this Feature projects no command for it; given a file in a commands output directory that no command produces, `--check` reports it stale. | 0001-F1 B-008 | Active |
| B-007 | Given a command with the name of a skill directory, this Feature fails naming both, because on a host that projects commands as skills the two would share one place. | ADR-0005 | Active |
| B-008 | Given a command projected as a skill, the output names its skills and uses repository artifact ownership and independent review instructions instead of named-agent bindings or agent contract paths, and substitutes the invocation text for `$ARGUMENTS`. | PR #9 review 5560928256 | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-27 -->

| ID | Constraint | Rules Out |
| --- | -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| C-1 | A command convenes roles and names skills; what each role does is in the role's contract. | Transitions, gates or a next-state table in a command; restating a role's refusals |
| C-2 | A command names a role or a skill once, in frontmatter. | A body link into `agents/` or `skills/`, which cannot resolve from both hosts |
| C-3 | A command names no product technology, path or build tool, as for a role (0001-F2 B-004). | A command per technology; a command naming a consumer's paths |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-27 -->

| # | Item | Exclusion Reason |
| --- | -------------------------------------------- | -------------------------------------------------------------------------------------- |
| 1 | A per-role command that convenes one role and adds nothing | The role is already an agent each host can call by name, and its halts live in its contract |
| 2 | Command-to-command transitions | A command is a prompt, not a state machine (C-1) |
| 3 | Host-specific command keys such as allowed tools or a model | A command inherits the role's host mapping; added when a host needs one |
| 4 | Grooming or design inside `spec` | Items are cut after agreement (`groom`) and design follows it (`design`); convening either in `spec` would act before the agreement exists |
| 5 | Documentation inside `spec` | The specification is the agreement's only document; a README or guide changes with the code that makes it true, not before |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-27 -->

| # | Concern | Classification |
| --- | --- | --- |
| 1 | Work sequence and role selection | Business |
| 2 | Host paths and request syntax | Technical |
| 3 | Role and skill resolution | Technical |
| 4 | Dependency validation | Both |

## 7. Technical Design

<!-- last written by: architect, 2026-10-27 -->

Command frontmatter declares `name`, `description`, `roles`, optional `skills`
and `argument-hint`; the body divides work by artifact owner or review pass and
ends with `$ARGUMENTS`. Load dependencies before planning outputs; invalid
commands publish nothing.

The configured output path selects the form (ADR-0005): `/SKILL.md` produces a
skill with repository ownership/review guidance and skill dependencies only;
other paths produce native commands with generated host-agent bindings. Both
share the source body with invocation substitution. Agent output paths cannot
change skill bytes (B-008); source commands remain separate from method skills.

Reject inline and resolved full, collapsed or shortcut reference links into
source agent or skill directories before writing, naming the command path.
Fenced examples remain literal text without routing authority (B-005).

Command output directories participate in stale-file reconciliation under
0001-F1 B-008. No additional host key, dependency or routing mechanism is needed.

Readiness: Ready for tests and implementation; applicable independent verdicts
remain in § 12.

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-27 -->

Use synthetic checkouts with both host command outputs, single/multiple role
bindings and dependent skills. Compare complete outputs; reject missing bindings,
malformed metadata and resolved Markdown role/skill links before writes. Preserve
fenced examples, detect stale outputs and verify hosts without command output.
Review C-1 and C-3 for each new command.

Testability: explicit filesystem roots isolate library and CLI checks without
network, credentials or time dependencies. Require the repository's 80% pooled
changed executable production-line floor under ADR-0007. Local execution does
not establish hosted CI or live-host instruction compliance.

### Performance

Not applicable.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-27 -->

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

Executable test names resolve in [`project.tests.fsx`](../../scripts/project.tests.fsx).

### Recorded verification

Historical PR #9 correction: 88-test baseline passed; three new controls failed
in a 90-test run, then the source-bound gate passed 90 F# tests, six CLI cases,
30 installer tests and 36 gate tests. Coverage run
`3db83d8ed9a44c81a5b5556ef7c304b3` measured 98/99 changed executable lines
(98.99%; floor 80%) against `d4eac9c`. These are local results; hosted CI and
live-host execution are unclaimed.

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-27 -->

| Date | Trigger | Change | Affected IDs |
| --- | --- | --- | --- |
| 2026-10-17 | Resolved Markdown destinations | Correct reference-link rejection; [lesson 0001](lessons/0001-resolve-markdown-reference-destinations.md). | B-005 |
| 2026-10-17 | Artifact-authority reconciliation | Align correction, investigation, refactor and reviewed-stub handoffs. | C-1; 0001-F2 B-042 |
| 2026-10-17 | PR #9 review 5560928256 | Withdraw B-002; require role-free command skills; ADR-0011 supersedes ADR-0005; [lesson 0002](lessons/0002-projected-skills-keep-one-way-boundary.md). | B-002, B-008 |
| 2026-10-19 | PR review routing and PR #11 correction | Include conditional surface review; shorten shared command prose. | C-1; § 7 |
| 2026-10-27 | Confirmed concise-specification plan | Compress in place; qualify sibling references; separate strategy, locators and history. | §§ 1–12; IDs unchanged |

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-27 -->

None.

## 12. Sign-off

<!-- last written by: spec-author, 2026-10-27 -->

| Section | Status | Reviewer | Note |
| --- | --- | --- | --- |
| Concision migration: §§ 1–12 | 🟢 | spec-reviewer | Independent `/root/review`, 2026-10-27: Approved snapshot SHA-256 `f27fd76c55d76a7023b3e96f55f010b453564c682325dd738b0b557381841b38`; no findings. Unchanged baseline scope retains its prior evidence; no new behavior or coverage is approved. |

### Historical verdicts

These verdicts concern earlier subjects; they do not approve this migration.
Full notes, limitations and superseded pending rows are recoverable from § 12
of this specification at Git revision `57cd1200b089d2872d4538c91b5d085cf069307d`.

| Scope | Verdict | Reviewer | Subject / evidence |
| --- | --- | --- | --- |
| Diff: PR #11 concise PR command | 🟢 | spec-reviewer | 2026-10-19; `/root/pr_diff_review`; `ff488e86e217f03b5e2658e1ccba490136f1e333`; `92d1cccff69eba0b13cf8e96fa770136a552af2a261e3fa3329f183fdf945c9e` |
| Diff: §§ 10, 12 and command corrections | 🟢 | spec-reviewer | 2026-10-17; `/root/contract_design_diff_review`; `36ab103e778d070d3ca8291ab56c63a0b96991bf6c656fb5c176ffce63785d5b` |
| Baseline: 1-12 and scenarios | 🟢 | spec-reviewer | 2026-10-17; `/root/baseline_projection_commands`; `2997ac3c6dbdf3bef910ffb6fef86aecc19d0bed`; retrospective; No historical pre-implementation approval |
| Correction: §7 B-005 reference links and scenarios | 🟢 | spec-reviewer | 2026-10-17; `/root/baseline_projection_commands`; `a2c1215ccec340282a9e75c4ba2e014946624db8f49c59513c03562bc247e7e3` |
| Diff: B-005 reference-link correction and evidence | 🟢 | spec-reviewer | 2026-10-17; `/root/baseline_projection_commands`; `a148bf47a5a726c87dbf4908547bcc27c6cfe56c70dfb7b7b95a2ca33ab629ed` |
| Agreement/design: B-008 and role-free command skills | 🟢 | spec-reviewer | 2026-10-17; `/root/pr9_review`; `bcf82372e7d69ff36d3158922f62ea033cf16d2a94f7a6730d18a009e8ac3564`; `0a701167dfdfcc07dbe64d421eb2365dc3cbd16cf12d3144c6464668cdc66301` |
| Diff: PR #9 B-008 correction | 🟢 | spec-reviewer | 2026-10-17; `/root/pr9_review`; `c7e3a9d`; `e9a277dcb696e1f08e01210ab091a2e0c87c293fd0134a9eaa81eb8a729eb227`; `1dbc35023731704b2e2c547b092ffdc732987bcb65aae9410259588f6208f111`; Overall PR remains blocked; Missing |
| Surface: PR #9 instruction/projection boundary | 🟢 | adversary | 2026-10-17; `/root/pr9_surface`; `93aad58741378bda9f09b48c8d268d1608c6f2eda9ab0cbb30a750dfcc2e9cc9`; `8b7d93f191bf041d2df7140d849c6ef5f35dbb9177185ca6604ab3139d668473` |

## Tasks

<!-- last written by: organizer, 2026-10-19 -->

- [0050 — Coordinate Commands delivery](../.issue/0050-coordinate-commands.yml) — authoritative delivery-status source.
- [0025 — Add the implement, release and design-ui commands](../.issue/0025-implement-release-design-ui.yml).
- [0026 — Widen the roles that design, spec and bug convene](../.issue/0026-command-role-lists.yml).
- [0027 — Decide the pull-request, backlog and single-role commands](../.issue/0027-pr-backlog-commands.yml).
- [0056 — Route the PR command through required review passes](../.issue/0056-route-pr-through-required-reviews.yml).

## Scoring

| Field | Value | Basis                                                |
| ----- | ----- | ---------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
