---
title: "Specification: Skill boundary"
description: "A skill never names a role: projection fails when a file under skills/ names one outside a generated block."
type: feature
id: "F4"
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
depends_on: ["F2"]
blocks: []
spikes: []
created: "2026-10-14"
updated: "2026-10-14"
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

<!-- last written by: spec-author, 2026-10-15 -->

| ID    | Claim                                                                                                                                                                                                                       | Source                     | Status |
| ----- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------- | ------ |
| B-001 | Given a file under `skills/` that, outside a generated block, names a role — its name in backticks, a hyphenated name as a whole word with its hyphens or with spaces, its persona name as a whole word, or a path into `agents/` — projection fails naming the file, the line and the role, and writes nothing. | 0001-F2 C-3 | Active |
| B-002 | Given a role name between a `BEGIN GENERATED` marker and its matching `END GENERATED` marker, the file passes, and lines after the block are reported at their own line numbers.                                             | A-1                        | Active |
| B-003 | Given this repository's `skills/`, the lint passes.                                                                                                                                                                         | Brief                      | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-14 -->

| ID  | Constraint                                                                                   | Rules Out                                                                    |
| --- | -------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| C-1 | A bare one-word role name is not linted.                                                      | Failing on "build", "release", "domain" or "design" used as ordinary words   |
| C-2 | The lint reads only `skills/`, never a host copy or `commands/`.                              | Linting generated output; treating a command projected as a Codex skill as a skill |

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

<!-- last written by: architect, 2026-10-14 -->

`lintSkills` in [`Projection.fsx`](../../scripts/Projection.fsx) is the
authority. `plan` calls it after the roles load and before any output is
built, so a violation writes nothing. It builds one pattern per form from the
loaded roles — so a new role or persona is linted without a word list — reads
every text file `filesUnder` returns for `skills/` (which excludes `.spec/`),
blanks each generated block while keeping its newlines, and reports every
finding in one failure.

A one-word role name is ordinary English in a skill ("the build", "a release",
"the domain"), so C-1 leaves it to review, as 0001-F2 leaves product technology
to review. The way to say who without a role name is the section owner ("the
owner of § 7") or the review pass ("the diff pass").

Readiness: **Ready for tests and implementation.**

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-14 -->

The `F4_` group in `dotnet fsi scripts/project.tests.fsx`, on the synthetic
checkout of 0001-F1 plus a role `log-reader` signing as Zed. The scenarios in
[`skills.feature`](skills.feature) are executed by those tests.

- B-001: one case per form, each failing `plan` with the file, line and role
  and writing nothing; a negative case of a bare name, a longer hyphenated word,
  a host directory and a lower-case word passes.
- B-002: a generated block naming three roles passes; a violation after it is
  reported at its own line.
- B-003: `lintSkills` over the real checkout.
- C-1 is a review question: at review, read new skill text for a bare one-word
  role name used as a name rather than a word.

### Performance

Not applicable.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-14 -->

| Claim ID | Scenario                                      | Test                                                    | Status  |
| -------- | --------------------------------------------- | ------------------------------------------------------- | ------- |
| B-001    | A skill names a role                          | `F4_B001_SkillNamingRole_FailsNamingFileLineAndRole`    | Covered |
| B-002    | A generated block names roles                 | `F4_B002_GeneratedBlockIsExempt`                        | Covered |
| B-003    | This repository's skills name no role         | `F4_B003_RepositorySkillsNameNoRole`                    | Covered |

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-14 -->

None.

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-14 -->

None.

## 12. Sign-off

<!-- last written by: spec-author, 2026-10-15 -->

| Section | Status | Reviewer      | Note                                                                                       |
| ------- | ------ | ------------- | ------------------------------------------------------------------------------------------ |
| 1-12    | 🟡     | spec-reviewer | Draft. The five existing violations in `sdd-conventions` were rephrased by section owner and review pass. |

## Tasks

None cut. The lint and the clean-up of the existing violations land together.

## Scoring

| Field | Value | Basis                                                |
| ----- | ----- | ---------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
