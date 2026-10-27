---
title: "Specification: Skill boundary"
description: "A skill never names a role: projection rejects role names outside generated blocks in source text within C-2 scope."
type: feature
id: "F4"
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
depends_on: ["F2"]
blocks: []
spikes: []
created: "2026-10-14"
updated: "2026-10-27"
github_issue: null
synced_at: null
---

# Specification: Skill boundary

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-27 -->

A consuming repository can adopt skills independently of its role roster.
Projection rejects role references in runtime skills so roster changes leave
no stale instructions.

## 2. User Needs

<!-- last written by: spec-author, 2026-10-27 -->

| # | Persona | Need | Pain Point Today |
| --- | -------------------- | ---------------------------------------------------------------- | --------------------------------------------------------- |
| 1 | Role maintainer | Rename, split or drop a role without editing any skill | Skills mention roles by name and go stale silently |
| 2 | Skill author | Learn at projection time, with a line number, that a sentence names a role | The rule is found only at review, if at all |
| 3 | Consuming repository | Adopt the skills with a different roster | A skill addresses a role the consumer does not have |

### Assumptions

| ID | Assumption |
| --- | -------------------------------------------------------------------------------------------- |
| A-1 | A generated block is written from role declarations and may name roles; nothing else may. |
| A-2 | A section owner or a review pass is a stable way to say who, where a role name is not. |

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-27 -->

| ID | Claim | Source | Status |
| ----- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------- | ------ |
| B-001 | Given a file within the lint scope (C-2) that, outside a generated block, names a role — its name in backticks, a hyphenated name as a whole word with its hyphens or with spaces, its persona name as a whole word, or a path into `agents/` — projection fails naming the file, the line and the role, and writes nothing. | 0001-F2 C-3 | Active |
| B-002 | Given a role name between a `BEGIN GENERATED` marker and its matching `END GENERATED` marker, the file passes, and lines after the block are reported at their own line numbers. | A-1 | Active |
| B-003 | Given this repository's files within the lint scope (C-2), the lint passes. | Brief | Active |
| B-004 | Given `.issue` tracker entries anywhere under `skills/`, role-name lint ignores those entries while continuing to reject violations in ordinary runtime skill files. | Decision 0002, 2026-10-17 | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-27 -->

| ID | Constraint | Rules Out |
| --- | -------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| C-1 | A bare one-word role name is not linted. | Failing on "build", "release", "domain" or "design" used as ordinary words |
| C-2 | The lint reads text files under `skills/`, excluding `.spec/`, `.issue/` and files containing NUL bytes, and never host copies or `commands/`. | Linting specification records, binary assets, generated host output or commands |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-27 -->

| # | Item | Exclusion Reason |
| --- | -------------------------------------------- | ------------------------------------------------------------------------- |
| 1 | A role naming a skill | Allowed and checked for existence by 0001-F2 B-006 |
| 2 | A command naming roles | A command sits above both; 0001-F3 |
| 3 | Role descriptions in prose, such as "the reviewer" | A description is not a name; renaming a role does not make it stale |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-27 -->

| # | Concern | Classification |
| --- | --- | --- |
| 1 | Which text names a role | Business |
| 2 | Generated-block exemptions | Technical |
| 3 | File, line and role diagnostics | Technical |

## 7. Technical Design

<!-- last written by: architect, 2026-10-27 -->

Lint runtime skill text after loading roles and before planning outputs. Build
patterns from role and supplied persona names; unnamed personas add no pattern.
Blank generated blocks without removing newlines, then report all violations
with file, line and role. A failure publishes nothing.

Use the source traversal owned by 0001-F1 B-025: exclude exact `.spec` and
`.issue` entries before opening or descending at every depth. Other hidden
resources and similarly named files remain linted (B-004; [decision
0002](decisions/0002-exclude-tracker-metadata.md)).

C-1 leaves ordinary English uses of one-word role names to review. Refer to a
section owner or review pass when assigning responsibility.

Readiness: Ready for tests and implementation; approval evidence remains in § 12.

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-27 -->

Use synthetic skill trees to reject prohibited role/persona forms with file and
line diagnostics before writes. Accept ordinary words, longer hyphenated words,
host directories and generated blocks; report violations after an exempt block
at their actual lines. Check the real skill inventory and distinguish tracker
metadata from runtime resources. Review ambiguous bare one-word role names under
C-1 rather than inventing a technology-specific word list.

Testability: pure linting and synthetic filesystem roots need no clock, network,
credential or live provider. Require the repository's 80% pooled changed
executable production-line floor under ADR-0007; local evidence does not assert
Linux CI execution.

### Performance

Not applicable.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-27 -->

| Claim ID | Scenario                                      | Test                                                    | Status  |
| -------- | --------------------------------------------- | ------------------------------------------------------- | ------- |
| B-001    | A skill names a role                          | `F4_B001_SkillNamingRole_FailsNamingFileLineAndRole`, `F4_B001_UnnamedPersona_HasNoNamePattern` | Covered |
| B-002    | A generated block names roles                 | `F4_B002_GeneratedBlockIsExempt`                        | Covered |
| B-003    | This repository's skills name no role         | `F4_B003_RepositorySkillsNameNoRole`                    | Covered |
| B-004 | Tracker metadata scenarios in the companion `.feature` | `F4_B004_TrackerRecordsAreIgnoredButRuntimeStillFailsLint` | Covered |

Executable test names resolve in [`project.tests.fsx`](../../scripts/project.tests.fsx).

### Recorded verification

Historical run `b9a269b572274089a3ad5b6e2b6561b2` measured 217/242 changed
executable lines (89.67%; floor 80%); the four original lint checks passed in
the 62-test linked runner. Tracker exclusion later passed 71 projector tests,
with its new control observed red before implementation. Source-bound coverage
against `d4eac9c` measured 49/50 (98%). These local runs establish no additional
platform, real-manager or live-agent execution.

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-27 -->

| Date | Trigger | Change | Affected IDs |
| --- | --- | --- | --- |
| 2026-10-14 | Initial delivery | `ecb996c` implements lint; `48a71f0` removes source violations. | B-001–B-003 |
| 2026-10-15 | Spaced role titles | `7d5e84f` amends scope; `8e1a60d` adds mechanism and regression. | B-001 |
| 2026-10-16 | Optional persona names | PR #5 / `f5366b9` preserves lint semantics for unnamed personas. | B-001 |
| 2026-10-16 | Baseline review | Clarify recorded text scope; decision 0001. | C-2 |
| 2026-10-17 | Tracker metadata exclusion | Exclude reserved tracker entries without changing runtime bytes. | B-004; decision 0002 |
| 2026-10-27 | Confirmed concise-specification plan | Compress in place; qualify sibling references; separate strategy, locators and history. | §§ 1–12; IDs unchanged |

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-27 -->

| ID | Question | Blocks | Resolution |
| --- | --- | --- | --- |
| OQ-1 | Which changed-line coverage threshold and enforcement gate does this repository adopt under ADR-0007? | None | Resolved 2026-10-16: README Testing adopts the 80% pooled changed executable production-line floor and shared local/CI gate delivered under 0001-F6. Independent /root/coverage_diff_review approved §8 and this resolution. |

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
| 1-7, 9-12 | 🟢 | spec-reviewer | 2026-10-16; `/root/f4_baseline_review`; `48a71f0` |
| 8 | 🟢 | spec-reviewer | 2026-10-16; `/root/coverage_diff_review`; Linux CI execution remains pending |
| 7-9 | 🟢 | spec-reviewer | 2026-10-16 |
| 7-9, 12 | 🟢 | spec-reviewer | 2026-10-16; `/root/persona_diff_review`; `e60dea1` |
| Agreement: B-004 tracker metadata | 🟢 | spec-reviewer | 2026-10-17; `/root/contract_design_diff_review`; `052e8bcc4eb813d0916cad49aa0ca791ca6be25aefff7861a97a4fd24054d363` |
| Design: § 7 tracker metadata | 🟢 | spec-reviewer | 2026-10-17; `/root/contract_design_diff_review`; `052e8bcc4eb813d0916cad49aa0ca791ca6be25aefff7861a97a4fd24054d363` |
| Diff: B-004 tracker metadata and executed tests | 🟢 | spec-reviewer | 2026-10-17; `/root/contract_design_diff_review`; `daf48f71a5b4f352a6e3df1a39bb0825f496322bc46f6ab2b558d67c3343965d` |
| Surface: tracker metadata boundary | 🟢 | adversary | 2026-10-17; `/root/contract_surface_review`; `daf48f71a5b4f352a6e3df1a39bb0825f496322bc46f6ab2b558d67c3343965d` |

## Tasks

<!-- last written by: organizer, 2026-10-17 -->

- [0051 — Coordinate Skill boundary delivery](../.issue/0051-coordinate-skill-boundary.yml) — authoritative delivery-status source.

Supporting maintenance history: [0043 — Review Skill boundary baseline and reconcile delivery evidence](../../.issue/0043-skill-boundary-evidence.yml). It retains its root scope.

## Scoring

| Field | Value | Basis                                                |
| ----- | ----- | ---------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
