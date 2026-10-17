---
title: "Specification: Skill boundary"
description: "A skill never names a role: projection rejects role names outside generated blocks in source text within C-2 scope."
type: feature
id: "F4"
epic: "0001"
spec_status: approved
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
depends_on: ["F2"]
blocks: []
spikes: []
created: "2026-10-14"
updated: "2026-10-16"
github_issue: null
synced_at: null
---

# Specification: Skill boundary

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-14 -->

Roles depend on skills and never the reverse: a role names the skills it works
with, and a skill states a rule any role can follow. A skill that names a role
binds the rule to one roster, so renaming, splitting or dropping a role leaves
stale instructions in every skill that mentioned it, and a consuming repository
with a different roster inherits instructions addressed to nobody. This Feature
makes the rule a check instead of a convention.

## 2. User Needs

<!-- last written by: spec-author, 2026-10-14 -->

| #   | Persona              | Need                                                             | Pain Point Today                                          |
| --- | -------------------- | ---------------------------------------------------------------- | --------------------------------------------------------- |
| 1   | Role maintainer      | Rename, split or drop a role without editing any skill           | Skills mention roles by name and go stale silently        |
| 2   | Skill author         | Learn at projection time, with a line number, that a sentence names a role | The rule is found only at review, if at all       |
| 3   | Consuming repository | Adopt the skills with a different roster                         | A skill addresses a role the consumer does not have       |

### Assumptions

| ID  | Assumption                                                                                   |
| --- | -------------------------------------------------------------------------------------------- |
| A-1 | A generated block is written from role declarations and may name roles; nothing else may.    |
| A-2 | A section owner or a review pass is a stable way to say who, where a role name is not.       |

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-17 -->

| ID    | Claim                                                                                                                                                                                                                       | Source                     | Status |
| ----- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------- | ------ |
| B-001 | Given a file within the lint scope (C-2) that, outside a generated block, names a role — its name in backticks, a hyphenated name as a whole word with its hyphens or with spaces, its persona name as a whole word, or a path into `agents/` — projection fails naming the file, the line and the role, and writes nothing. | 0001-F2 C-3 | Active |
| B-002 | Given a role name between a `BEGIN GENERATED` marker and its matching `END GENERATED` marker, the file passes, and lines after the block are reported at their own line numbers.                                             | A-1                        | Active |
| B-003 | Given this repository's files within the lint scope (C-2), the lint passes.                                                                                                                                                                         | Brief                      | Active |
| B-004 | Given `.issue` tracker entries anywhere under `skills/`, role-name lint ignores those entries while continuing to reject violations in ordinary runtime skill files. | Decision 0002, 2026-10-17 | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-17 -->

| ID  | Constraint                                                                                   | Rules Out                                                                    |
| --- | -------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| C-1 | A bare one-word role name is not linted.                                                      | Failing on "build", "release", "domain" or "design" used as ordinary words   |
| C-2 | The lint reads text files under `skills/`, excluding `.spec/`, `.issue/` and files containing NUL bytes, and never host copies or `commands/`.                              | Linting specification records, binary assets, generated host output or commands |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-14 -->

| #   | Item                                         | Exclusion Reason                                                          |
| --- | -------------------------------------------- | ------------------------------------------------------------------------- |
| 1   | A role naming a skill                        | Allowed and checked for existence by [0001-F2](../../agents/.spec/README.md) B-006 |
| 2   | A command naming roles                       | A command sits above both; [0001-F3](../../commands/.spec/README.md)       |
| 3   | Role descriptions in prose, such as "the reviewer" | A description is not a name; renaming a role does not make it stale |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-14 -->

| #   | Concern                                         | Classification |
| --- | ----------------------------------------------- | -------------- |
| 1   | Which forms of text count as naming a role      | Business       |
| 2   | Exempting generated blocks                      | Technical      |
| 3   | Reporting file, line and role                   | Technical      |

## 7. Technical Design

<!-- last written by: architect, 2026-10-17 -->

`lintSkills` in [`Projection.fsx`](../../scripts/Projection.fsx) is the
authority. `plan` calls it after the roles load and before any output is
built, so a violation writes nothing. It builds one pattern per form from the
loaded roles and supplied persona names; unnamed personas add no name pattern.
It reads every text file the source traversal returns for `skills/` (excluding
`.spec/` and `.issue/` metadata),
blanks each generated block while keeping its newlines, and reports every
finding in one failure.

A one-word role name is ordinary English in a skill ("the build", "a release",
"the domain"), so C-1 leaves it to review, as 0001-F2 leaves product technology
to review. The way to say who without a role name is the section owner ("the
owner of § 7") or the review pass ("the diff pass").

Readiness: **Ready for tests and implementation.**

### Tracker metadata boundary

[Decision 0002](decisions/0002-exclude-tracker-metadata.md) extends C-2 and
adds B-004. `lintSkills` uses projection's source-only `runtimeFilesUnder`
traversal, specified by 0001-F1 B-025, rather than the output-file traversal.
Exact `.issue` entries are excluded before opening or descending; role names
in work-item records therefore do not become method instructions. Unrelated
hidden runtime resources and similarly named files remain linted.

Synthetic fixtures put role-bearing records at the skill root and nested
resource depths, then introduce a runtime violation to prove the guard remains
active. Readiness: Ready; independent agreement/design approval is recorded in § 12.

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-16 -->

The `F4_` group in `dotnet fsi scripts/project.tests.fsx`, on the synthetic
checkout of 0001-F1 plus a role `log-reader` signing as Zed. The scenarios in
[`skills.feature`](skills.feature) are executed by those tests.

- B-001: one case per form, each failing `plan` with the file, line and role
  and writing nothing; a negative case of a bare name, a longer hyphenated word,
  a host directory and a lower-case word passes. An unnamed persona adds no name
  pattern, while its role name remains linted.
- B-002: a generated block naming three roles passes; a violation after it is
  reported at its own line.
- B-003: `lintSkills` over the real checkout.
- C-1 is a review question: at review, read new skill text for a bare one-word
  role name used as a name rather than a word.

The [repository Testing policy](../../README.md#testing) establishes the shared
local/CI coverage gate and its 80% pooled changed executable production-line
floor under ADR-0007. Fresh repository verification run
`b9a269b572274089a3ad5b6e2b6561b2` passes 217/242 changed executable lines
(89.67%); its source-bound manifest validates independently. The four named
`F4_` tests all pass within the 62-test original-linked F# runner. This F4
change edits specification evidence; its lint production source is unchanged.
The synthetic filesystem and pure lint seam require no clock, network or live
provider. Linux CI execution of the shared gate remains pending; local evidence
does not substitute for that result.

### Performance

Not applicable.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-16 -->

| Claim ID | Scenario                                      | Test                                                    | Status  |
| -------- | --------------------------------------------- | ------------------------------------------------------- | ------- |
| B-001    | A skill names a role                          | `F4_B001_SkillNamingRole_FailsNamingFileLineAndRole`, `F4_B001_UnnamedPersona_HasNoNamePattern` | Covered |
| B-002    | A generated block names roles                 | `F4_B002_GeneratedBlockIsExempt`                        | Covered |
| B-003    | This repository's skills name no role         | `F4_B003_RepositorySkillsNameNoRole`                    | Covered |

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-17 -->

| Date | Trigger | Delta / evidence |
| --- | --- | --- |
| 2026-10-14 | Original implementation and source cleanup | `ecb996c` explicitly delivered B-001–B-003; `48a71f0` removed source violations and regenerated outputs for B-003. |
| 2026-10-15 | Spaced role titles name the same role | `7d5e84f` amended B-001; `8e1a60d` added its mechanism and regression case. |
| 2026-10-16 | Optional persona names | PR #5, `f5366b9`, preserved B-001 and added unnamed-persona verification; earlier §12 delta approvals remain recorded. |
| 2026-10-16 | Independent baseline review | Clarify C-2 and its claim references to match the original recorded scope; [decision 0001](decisions/0001-recorded-source-text-scope.md). |

These commits are ancestors of `origin/main` at `c890c82`. They establish
historical delivery; current tests corroborate it. No retrospective tracker
claim assignment or new runtime behavior is asserted.

2026-10-17: User selected consistent tracker metadata exclusion. Add B-004;
source traversal and installation omit reserved tracker entries, preserving
runtime bytes and existing ownership. See Decision 0002.

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-16 -->

| ID | Question | Blocks | Resolution |
| --- | --- | --- | --- |
| OQ-1 | Which changed-line coverage threshold and enforcement gate does this repository adopt under ADR-0007? | None | Resolved 2026-10-16: README Testing adopts the 80% pooled changed executable production-line floor and shared local/CI gate delivered under 0001-F6. Independent /root/coverage_diff_review approved §8 and this resolution. |

## 12. Sign-off

<!-- last written by: spec-author, 2026-10-17 -->

| Section | Status | Reviewer      | Note                                                                                       |
| ------- | ------ | ------------- | ------------------------------------------------------------------------------------------ |
| 1-7, 9-12 | 🟢 | spec-reviewer | Independent `/root/f4_baseline_review`, 2026-10-16: bounded baseline approved after original scope clarification and historical-delta recording. Original five-violation cleanup evidence is `48a71f0`; earlier amendment approvals remain intact. |
| 8 | 🟢 | spec-reviewer | Independent `/root/coverage_diff_review`, 2026-10-16: baseline approved against README Testing and the measured 0001-F6 gate; OQ-1 resolved. Earlier ADR-0007 blocker is cleared. Linux CI execution remains pending; prerequisite F2 still prevents declaring full F4 delivery. |
| 7-9 | 🟢 | spec-reviewer | Approved 2026-10-16: optional-name lint design and coverage reviewed; 0001-F4 B-001 remains satisfied. Baseline evidence retained. |
| 7-9, 12 | 🟢 | spec-reviewer | Rebase delta approved 2026-10-16 by independent `/root/persona_diff_review` against main `e60dea1`; no findings, earlier evidence retained. |
| Agreement: B-004 tracker metadata | 🟢 | spec-reviewer | Independent `/root/contract_design_diff_review`, 2026-10-17, approved subject `052e8bcc4eb813d0916cad49aa0ca791ca6be25aefff7861a97a4fd24054d363` before tests/implementation; scenarios and decisions included. |
| Design: § 7 tracker metadata | 🟢 | spec-reviewer | Independent `/root/contract_design_diff_review`, 2026-10-17, approved subject `052e8bcc4eb813d0916cad49aa0ca791ca6be25aefff7861a97a4fd24054d363` before tests/implementation; planned strategy is testable. |
| Diff: B-004 tracker metadata and executed tests | 🟢 | spec-reviewer | Independent `/root/contract_design_diff_review`, 2026-10-17, approved subject `daf48f71a5b4f352a6e3df1a39bb0825f496322bc46f6ab2b558d67c3343965d`; 71 projector and 30 installer tests pass, with test-first evidence. |
| Surface: tracker metadata boundary | 🟢 | adversary | Independent `/root/contract_surface_review`, 2026-10-17, approved subject `daf48f71a5b4f352a6e3df1a39bb0825f496322bc46f6ab2b558d67c3343965d`; native/private traversal and existing publication safeguards verified. |

## Tasks

<!-- last written by: organizer, 2026-10-16 -->

- [0043 — Review Skill boundary baseline and reconcile delivery evidence](../../.issue/0043-skill-boundary-evidence.yml).

## Scoring

| Field | Value | Basis                                                |
| ----- | ----- | ---------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
