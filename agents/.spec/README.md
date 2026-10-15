---
title: "Specification: Role contract"
description: "What every role file must declare and must not say, so that one host-free file is enough for every host."
type: feature
id: "F2"
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
depends_on: []
blocks: []
spikes: []
created: "2026-10-14"
updated: "2026-10-14"
github_issue: null
synced_at: null
---

# Specification: Role contract

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-14 -->

A role file is a contract a consuming repository can adopt without editing: it
says what the role owns, reads, produces and refuses, and it names no product
technology, path or build tool. Without that contract a role either binds itself
to one project and is forked per consumer, or leaves its section ownership
implicit and two roles end up writing the same section.

## 2. User Needs

<!-- last written by: spec-author, 2026-10-14 -->

| #   | Persona              | Need                                                           | Pain Point Today                                            |
| --- | -------------------- | -------------------------------------------------------------- | ----------------------------------------------------------- |
| 1   | Consuming repository | Adopt a role as-is and point it at a companion skill           | A role naming another project's paths must be rewritten     |
| 2   | Projection           | Read a fixed set of keys from every role                       | N/A (internal dependency on [0001-F1](../../scripts/.spec/README.md)) |
| 3   | Role author          | Know which keys a role may carry and which belong to a host    | Host knobs creep into role files and bind them to one host  |

### Assumptions

| ID  | Assumption                                                                            |
| --- | ------------------------------------------------------------------------------------- |
| A-1 | The specification template's `##` headings are the authority on which sections exist. |
| A-2 | A consuming repository provides a companion skill that names its paths and commands.  |

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-14 -->

| ID    | Claim                                                                                                                                                                     | Source                                                                         | Status |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------ |
| B-001 | Given the role files, each declares `name`, a one-line `description`, a semver `version` and `owned_spec_sections`, and none carries a host key.                          | [ADR-0001](../../.spec/adr/0001-roles-are-host-free-and-hosts-are-overlays.md) | Active |
| B-002 | Given the roles and the unowned declarations in the roles index, every section of the specification template has exactly one owner.                                      | `sdd-conventions` § Section ownership                                          | Active |
| B-003 | Given a role body, it carries the headings Owns, Read first, Produce or Look for, and Refuse.                                                                              | Hooked role contracts                                                          | Active |
| B-004 | Given a role body, it names no product technology, library, repository path, package or build tool; the consuming repository's companion skill names those.               | Root README § Purpose                                                          | Active |
| B-005 | Given a role body carrying `## Look for`, it also carries `## Report`, so every reviewing role states the form its findings take. | OQ-1 | Active |
| B-006 | Given a role body linking `../skills/<name>/SKILL.md`, that skill exists in `skills/`; projection fails naming the role and the skill otherwise. | C-1 | Active |
| B-007 | Given a role declaring `persona`, it carries a one-line `name` and a one-line `voice`, and no other role declares the same persona name; the role name stays the file stem, the host agent name and every machine-read stamp. | Brief: personas | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-14 -->

| ID  | Constraint                                                                        | Rules Out                                                             |
| --- | --------------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| C-1 | A role names skills by relative link to `skills/<name>/SKILL.md` and nothing else. | A link into a host directory, an absolute path, a URL to another repo |
| C-2 | A role's `name` equals its file stem.                                              | A display name that differs from the file, an alias                   |
| C-3 | Ownership is declared on the role, never in a skill.                              | A hand-kept ownership table; a skill naming a role                    |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-14 -->

| #   | Item                                              | Exclusion Reason                                                        |
| --- | ------------------------------------------------- | ----------------------------------------------------------------------- |
| 1   | Rendering a role for a host                       | [0001-F1 Projection](../../scripts/.spec/README.md)                     |
| 2   | A role per technology or platform                 | A technology belongs in the consuming repository's companion skill (B-004) |
| 3   | Persona traits beyond a name and a voice          | A name and one line of voice are what a reader acts on; a trait list is restated prose |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-14 -->

| #   | Concern                                           | Classification |
| --- | ------------------------------------------------- | -------------- |
| 1   | Which frontmatter keys a role carries             | Business       |
| 2   | Which body headings a role carries                | Business       |
| 3   | Keeping product technology out of a role body     | Business       |
| 4   | Validating the keys                               | Technical      |
| 5   | The name a role signs its prose with              | Business       |

## 7. Technical Design

<!-- last written by: architect, 2026-10-14 -->

A role file is Markdown with YAML frontmatter. The keys and their checks are
declared by `loadRole` in [`Projection.fsx`](../../scripts/Projection.fsx);
that function is the authority, not this section.

| Key                   | Form                                                      |
| --------------------- | --------------------------------------------------------- |
| `name`                | equals the file stem                                      |
| `description`         | one line; the host's trigger text                         |
| `version`             | semver, bumped when the contract changes                  |
| `owned_spec_sections` | list of `"<n>"`, `"Tasks"`, `"Scoring"` or `"<n>/<Subsection>"`; empty is valid |
| `persona`             | optional; `name` and `voice`, one line each; the name unique across roles |

A persona is how a role signs the prose it writes: reports, findings, review
notes. Projection puts one generated line naming it ahead of the body and lists
every persona in [`PERSONAS.md`](../PERSONAS.md). A stamp a tool reads —
`last written by`, `author`, the § 12 Reviewer column — keeps the role name, so
no tool has to map a persona back to a role.

Sections no single role owns are declared once, in the frontmatter of
[`README.md`](../README.md) under `unowned_spec_sections`, and merged with the
roles' claims before the totality check.

The body is free Markdown under four headings in this order: `## Owns`,
`## Read first`, `## Produce` (or `## Look for` followed by `## Report` for a
reviewing role), and `## Refuse`. A role may add a heading, such as `## Escalate`. Links to skills are relative, `../skills/<name>/SKILL.md`, so that
the same bytes resolve from `agents/` and from a host's agents directory one
level below the root.

Readiness: **Ready for tests and implementation.**

## 8. Testing Strategy

<!-- last written by: test-writer, 2026-10-14 -->

B-001 to B-003 run against the real checkout inside
`dotnet fsi scripts/project.tests.fsx`, in the group prefixed `F2_`, so a role
edit that breaks the contract fails the same command CI runs. The scenarios in
[`roles.feature`](roles.feature) are executed by those tests.

- B-001: `loadRoles` over `agents/`; the loader itself rejects a missing key,
  a non-semver version, a multi-line description or a host key, so the test
  asserts the roster loads and that both host overlays map every role.
- B-002: `ownershipRows` over the real roles, index and template; one row per
  template section plus the subsection row.
- B-003: the `## ` headings of each loaded body.
- B-005: every loaded body carrying `## Look for` also carries `## Report`.
- B-006: `validateSkillLinks` over the real roles passes, and a synthetic role
  linking a missing skill makes `plan` fail before anything is written.
- B-007: `loadRole` rejects a persona missing a key or not a mapping and
  `validatePersonas` rejects a shared name; the test runs both on synthetic
  roles and runs `validatePersonas` over the real roster.
- B-004 is a review question, not a test: at review, grep the role bodies for
  the names of the consuming project's stack and tooling. The reviewer records
  the result in § 12. A lint over a word list was considered and rejected
  because the list would itself be technology-specific.

### Performance

Not applicable.

## 9. Traceability Matrix

<!-- last written by: test-writer, 2026-10-14 -->

| Claim ID | Scenario                                            | Test                                                     | Status  |
| -------- | --------------------------------------------------- | -------------------------------------------------------- | ------- |
| B-001    | Every role declares the contract keys               | `F2_B001_EveryRoleDeclaresTheContractKeysAndNoHostKey`   | Covered |
| B-002    | Every template section has exactly one owner        | `F2_B002_EveryTemplateSectionHasExactlyOneOwner`         | Covered |
| B-003    | Every role body carries the four headings           | `F2_B003_EveryRoleBodyCarriesTheFourSections`            | Covered |
| B-004    | A role body names no product technology             | Manual: review question in § 8                           | Partial |
| B-005    | A reviewing role states its report                  | `F2_B005_LookForRoleCarriesReport`                       | Covered |
| B-006    | A role links only skills that exist                 | `F2_B006_EverySkillLinkResolves`                         | Covered |
| B-007    | A persona carries a name and a voice and is unique  | `F2_B007_PersonaRequiresNameAndVoiceAndIsUnique`         | Covered |

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-14 -->

None.

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-14 -->

| ID   | Question                                                                                   | Blocks | Resolution |
| ---- | ------------------------------------------------------------------------------------------ | ------ | ---------- |
| OQ-1 | Should the contract require a `## Report` heading for reviewing roles, as `spec-reviewer` and `architect` already carry? | B-003  | Resolved 2026-10-14: yes. A body with `## Look for` also carries `## Report`; claimed as B-005. |

## 12. Sign-off

<!-- last written by: spec-reviewer, 2026-10-14 -->

| Section | Status | Reviewer      | Note                                                            |
| ------- | ------ | ------------- | --------------------------------------------------------------- |
| 1-12    | 🟡     | spec-reviewer | Draft. B-004 reviewed by grep at the scaffolding change and again for the second-wave roster (2026-10-14): no product technology in any role body. |

## Tasks

None cut. The first six roles landed with the scaffolding change and the second
wave of eighteen with the roster change; a new role or a contract change cuts
its own work item.

## Scoring

| Field | Value | Basis                                              |
| ----- | ----- | -------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
