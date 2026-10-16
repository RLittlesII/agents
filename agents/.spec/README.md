---
title: "Specification: Role contract"
description: "Host-free role contracts organized around accountable leads, conditional specialists and artifact handoffs."
type: feature
id: "F2"
epic: "0001"
spec_status: in-review
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
blocks: ["F3", "F4"]
spikes: []
created: "2026-10-14"
updated: "2026-10-16"
github_issue: null
synced_at: null
---

# Specification: Role contract

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-16 -->

A consuming repository can identify who leads its work and which expertise the
specification requires, using host-free contracts it can adopt without editing;
this removes a peer-to-peer handoff graph whose overlapping roles obscure
accountability and turn specialties into unnecessary workflow stops. — Ruth

## 2. User Needs

<!-- last written by: spec-author, 2026-10-16 -->

| #   | Persona              | Need                                                           | Pain Point Today                                            |
| --- | -------------------- | -------------------------------------------------------------- | ----------------------------------------------------------- |
| 1   | Consuming repository | Adopt a role as-is and point it at a companion skill           | A role naming another project's paths must be rewritten     |
| 2   | Projection           | Read a fixed set of keys from every role                       | N/A (internal dependency on [0001-F1](../../scripts/.spec/README.md)) |
| 3   | Role author          | Know which keys a role may carry and which belong to a host    | Host knobs creep into role files and bind them to one host  |
| 4   | Person starting work | Identify the lead and relevant specialists for the situation | A global handoff graph makes every specialty look mandatory |
| 5   | Work lead | Integrate bounded contributions against the same claims | Peer handoffs leave nobody accountable for the complete result |
| 6   | Persistence maintainer | Obtain database expertise across the affected lifecycle | Persistence duties are scattered across general contracts |

### Collaboration glossary

| Term | Meaning |
| --- | --- |
| State | The responsibility currently exercised; not an executable state machine or a required separate agent invocation. |
| Lead | The role accountable for the current state's complete artifact and integration of assigned contributions. |
| Specialist | A role assigned bounded work because affected claims, surfaces or evidence require its expertise. |
| Mode | A bounded way a retained role works, without creating a separate role or changing its permissions. |
| Artifact handoff | Passing the recorded agreement, design, failing test, change or finding that the receiving responsibility needs. |

### State leads

| State | Lead | Artifact or responsibility |
| --- | --- | --- |
| Specify | `spec-author` | Discovery followed by the decided agreement and scenarios |
| Design | `architect` | Technical design and readiness |
| Implement | `implementer` | Integration of production changes against the design and failing tests |
| Verify | `spec-reviewer` | Independent design or diff findings and verdicts, with relevant verification evidence |
| Release | `release` | Rollout, recovery and verification using the existing confirmation rules |
| Investigate | `bug-finder` | Reproduction, classification, evidence and proven cause |

### Specialist selection

| Specialist | Trigger | Bounded contribution |
| --- | --- | --- |
| `domain` | Business invariants, rules or domain concepts need modeling or change | Domain model and enforceable rules |
| `ux` | Interaction, accessibility, states or copy need deciding | Decided interaction before presentation implementation |
| `ui` | The decided interaction requires presentation changes | Presentation code under the implementation lead |
| `database` | Persistence design, implementation, failure investigation or rollout is affected | Stored data shape, keys, indexes, nullability, transactions, queries, migration/backfill, compatibility and recovery evidence |
| `test-writer` | Agreed behavior lacks a failing test or traceability needs amendment | Tests and coverage records within existing section ownership |
| `tester` | Changed behavior needs execution or exploration | Run evidence and reproductions |
| `adversary` | A boundary, credential, permission or external input is affected | Independent hostile surface findings |
| `benchmarker` | A recorded performance, cost or structural concern needs measurement | Benchmarks and measurements of that concern |
| `metric-nerd` | An operational decision needs instrumentation, baselines or telemetry comparison | Signal requirements and operational evidence |
| `build` | A gate, build configuration, tool or pipeline needs work | Reproducible build and gate changes |
| `organizer` | Approved claims need work items or the queue needs maintenance | Decomposition and ranked work; no mandatory delivery stop |
| `document-writer` | Substantial prose is missing or stale | Documentation checked against the relevant artifact |
| `auditor` | A repository audit is requested, scheduled or required before release | Repository findings; no mandatory stop on every change |

### Retired roles and retained responsibilities

| Retired role | Destination | Retained responsibility |
| --- | --- | --- |
| `analyst` | `spec-author`, discovery mode | Identify requester, failure state, settled answers and material unknowns before writing a confirmed need as claims |
| `bug-fixer` | `implementer`, correction mode | Accepted diagnosis and a regression test failing for the proven cause precede the bounded fix; preserve regression, systemic-site and lesson duties |
| `yak-shaver` | Current lead with the relevant specialist | Remove the named obstruction, prove original work can resume and record adjacent work separately |
| `pair-programmer` | Collaboration mode available to every retained role | Visible steps, explanation, a stop at decision forks and a recorded decision; retain the active role's write boundaries |
| `log-parser` | `bug-finder`, log-analysis capability | Isolate the evidence window and correlation, reconstruct sequence, mark inference, protect secrets and report gaps; large evidence may be delegated as a bounded task |
| `critic` | Independent `spec-reviewer`, design-review mode | Review the design against claims, constraints and decisions before implementation; report findings without rewriting |

### Scenario routing

| Situation | Route | Conditional involvement |
| --- | --- | --- |
| Bug report | Investigate the existing claim → accepted diagnosis and failing regression test → Implement correction → Verify → Release when shipping is required | `spec-author` for missing/disputed expected behavior; `architect` for a design gap; specialists for the affected concern |
| New feature | Specify → Design → failing tests → Implement → Verify → Release | Only specialists whose trigger applies; `organizer` cuts work after agreement |
| Technical obstacle | Current lead retains parent work → bounded investigation or obstruction removal → resume original work | `architect` for an unknown approach; `bug-finder` for unexpected behavior; `build` for tooling or a gate |
| Pitched idea | Specify in discovery mode → agreement only when accepted | Bounded `architect` feasibility work if required to decide the idea; no speculative production implementation |
| Production issue | `bug-finder` diagnoses while `release` coordinates any recovery under an existing procedure → correction route only if software must change | Log analysis when logs exist; `metric-nerd` for operational evidence; other specialists for the affected concern |
| Changed shipped requirement | Amend the existing agreement → assess design, dependencies and compatibility → revised failing tests → Implement → Verify → Release | `database` for existing data or migration; `ux`/`domain` for changed interaction/rules; `metric-nerd` for rollout evaluation |

The tables define responsibility and artifact dependencies; the method skills
retain their gates and commands only convene the applicable roles. — Ruth

### Assumptions

| ID  | Assumption                                                                            |
| --- | ------------------------------------------------------------------------------------- |
| A-1 | The specification template's `##` headings are the authority on which sections exist. |
| A-2 | A consuming repository provides a companion skill that names its paths and commands.  |

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-16 -->

| ID    | Claim                                                                                                                                                                     | Source                                                                         | Status |
| ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------ |
| B-001 | Given the role files, each declares `name`, a one-line `description`, a semver `version` and `owned_spec_sections`, and none carries a host key.                          | [ADR-0001](../../.spec/adr/0001-roles-are-host-free-and-hosts-are-overlays.md) | Active |
| B-002 | Given the roles and the unowned declarations in the roles index, every section of the specification template has exactly one owner.                                      | `sdd-conventions` § Section ownership                                          | Active |
| B-003 | Given a role body, it carries the headings Owns, Read first, Produce or Look for, and Refuse.                                                                              | Hooked role contracts                                                          | Active |
| B-004 | Given a role body, it names no product technology, library, repository path, package or build tool; the consuming repository's companion skill names those.               | Root README § Purpose                                                          | Active |
| B-005 | Given a role body carrying `## Look for`, it also carries `## Report`, so every reviewing role states the form its findings take. | OQ-1 | Active |
| B-006 | Given a role body linking `../skills/<name>/SKILL.md`, that skill exists in `skills/`; projection fails naming the role and the skill otherwise. | C-1 | Active |
| B-007 | Given a role declaring `persona`, it carries a one-line `name` and a one-line `voice`, and no other role declares the same persona name; the role name stays the file stem, the host agent name and every machine-read stamp. | Brief: personas | Active |
| B-008 | Given a role declaring `reviews`, each entry is a review unit and the body carries `## Look for` and `## Report`; projection fails naming the role otherwise. | `sdd-conventions` § Review passes | Active |
| B-009 | Given a review unit in the roles index, it declares what it reviews, when its pass runs and what the pass never does, and the registry shows all three; projection fails naming the unit and the missing key otherwise. | LTI review-pass registry | Active |
| B-010 | Given work exercising a state, this Feature assigns it the accountable lead in § 2 State leads. | User brief: lead-and-specialist grouping | Active |
| B-011 | Given a specialist assignment, this Feature requires a bounded contribution justified by an affected claim, surface or evidence under § 2 Specialist selection. | User brief: specialists do certain parts | Active |
| B-012 | Given a specialist contribution, this Feature keeps the current lead accountable for integrating it into the state's complete artifact. | User brief: implementer collaboration | Active |
| B-013 | Given an assignment by a lead, this Feature preserves the assigned role's existing artifact and specification-section write boundaries. | B-002; user brief: spec drives the software | Active |
| B-014 | Given work whose earlier required artifacts and approvals remain valid for its affected claims, this Feature starts at the earliest unmet responsibility. | User brief: small changes need no repeated discovery | Active |
| B-015 | Given an undecided need, this Feature assigns discovery to `spec-author` in the mode replacing `analyst` in § 2. | User brief: fold redundant roles; § 2 retirement mapping | Active |
| B-016 | Given a software correction, this Feature assigns it to `implementer` in the mode replacing `bug-fixer` in § 2. | User brief: fold redundant roles; § 2 retirement mapping | Active |
| B-017 | Given an incidental obstruction, this Feature assigns bounded removal to the current lead with relevant expertise in place of `yak-shaver`. | User brief: fold redundant roles; § 2 retirement mapping | Active |
| B-018 | Given a request for work alongside the person, this Feature offers the § 2 collaboration mode within the active role in place of `pair-programmer`. | User brief: fold redundant roles; § 2 retirement mapping | Active |
| B-019 | Given logs relevant to an investigation, this Feature assigns the § 2 log-analysis capability to `bug-finder` in place of `log-parser`. | User brief: fold redundant roles; § 2 retirement mapping | Active |
| B-020 | Given a design ready for review, this Feature assigns the design-review mode replacing `critic` to a `spec-reviewer` instance that did not author that design. | User brief: fold redundant roles; § 2 retirement mapping | Active |
| B-021 | Given a persistence concern, this Feature assigns `database` the bounded expertise in § 2 Specialist selection at the affected state. | User brief: add a database specialist | Active |
| B-022 | Given database work, this Feature constrains its storage decisions to the agreed domain invariants and architect-owned design. | User brief: database specialist; B-002 | Active |
| B-023 | Given a database migration or backfill, this Feature requires the database contribution to include compatibility and rollback or recovery verification. | User brief: database specialist; retained implementer/release migration duties | Active |
| B-024 | Given a measurement request, this Feature selects `benchmarker` or `metric-nerd` by their distinct § 2 triggers. | User brief: retain performance and observability distinction | Active |
| B-025 | Given required production instrumentation, this Feature assigns its code to the implementation lead or relevant implementation specialist. | User brief: specialists within implementation; existing benchmarker code boundary | Active |
| B-026 | Given a bug report, this Feature follows the § 2 bug route against existing claimed behavior. | User brief: bug comes in | Active |
| B-027 | Given an accepted new feature, this Feature follows the § 2 feature route with the stated artifact dependencies. | User brief: new feature starts | Active |
| B-028 | Given a technical obstacle, this Feature follows the § 2 obstacle route according to the kind of uncertainty. | User brief: technical problem | Active |
| B-029 | Given a pitched idea, this Feature follows the § 2 idea route without production implementation before acceptance. | User brief: new idea pitched | Active |
| B-030 | Given a production issue, this Feature follows the § 2 production route according to whether recovery or a software correction is required. | User brief: issue in production | Active |
| B-031 | Given unavailable incident telemetry, this Feature records the missing evidence as a gap without treating it as a proven cause. | User brief: production diagnosis depends on available observability | Active |
| B-032 | Given changed shipped requirements, this Feature follows the § 2 amendment route while retaining permanent claim anchors. | User brief: requirements change in production; spec-and-traceability | Active |
| B-033 | Given the resulting role roster, this Feature exposes exactly the six leads and thirteen specialists listed in § 2 as standalone roles. | User brief: consolidate six and add database | Active |
| B-034 | Given the consolidated roster, this Feature makes active role, command, host and projected references resolve to retained roles and their applicable modes. | User brief: functional roles; 0001-F1 B-008/B-009; 0001-F3 B-003 | Active |
| B-035 | Given historical records affected by role retirement, this Feature preserves their recorded identities and evidence. | spec-and-traceability § Claims and traceability / Records beside a specification | Active |
| B-036 | Given the roles index handoff guidance, this Feature presents bounded state, specialist and scenario guidance in place of the global all-role graph. | User brief: handoffs too complicated | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-16 -->

| ID  | Constraint                                                                        | Rules Out                                                             |
| --- | --------------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| C-1 | A role names skills by relative link to `skills/<name>/SKILL.md` and nothing else. | A link into a host directory, an absolute path, a URL to another repo |
| C-2 | A role's `name` equals its file stem.                                              | A display name that differs from the file, an alias                   |
| C-3 | Ownership is declared on the role, never in a skill.                              | A hand-kept ownership table; a skill naming a role                    |
| C-4 | Grouping preserves existing approval, test-first, traceability and independent-review requirements. | Skipping a required gate because one lead coordinates several specialties |
| C-5 | State, specialist and mode guidance introduces no required agent invocation per responsibility. | Treating every specialist as a mandatory workflow stop or separate process |
| C-6 | Commands convene roles within 0001-F3 C-1 and C-2. | Embedding transitions, gates or next-state tables in command bodies |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-16 -->

| #   | Item                                              | Exclusion Reason                                                        |
| --- | ------------------------------------------------- | ----------------------------------------------------------------------- |
| 1   | Rendering a role for a host                       | [0001-F1 Projection](../../scripts/.spec/README.md)                     |
| 2   | A role per technology or platform                 | A technology belongs in the consuming repository's companion skill (B-004) |
| 3   | Persona traits beyond a name and a voice          | A name and one line of voice are what a reader acts on; a trait list is restated prose |
| 4   | A role-level declaration of how scenarios execute, or an opt-out into documentation-only scenarios | How the runner executes scenarios is the companion's § "Testing"; a scenario alone is not coverage either way |
| 5   | Database engines, persistence frameworks and engine-specific tooling | Replaces the database-role exclusion on 2026-10-16: host-free database expertise is included by B-021; engine knowledge remains the companion's (B-004) |
| 6   | An executable workflow engine or new role frontmatter schema | Responsibility grouping uses existing contracts and review declarations; no runtime state machine is requested |
| 7   | Unrelated role renames or changes to section ownership | The accepted scope retires the six § 2 roles and adds `database`; existing artifact boundaries remain |

## 6. Concern Separation

<!-- last written by: architect, 2026-10-14 -->

| #   | Concern                                           | Classification |
| --- | ------------------------------------------------- | -------------- |
| 1   | Which frontmatter keys a role carries             | Business       |
| 2   | Which body headings a role carries                | Business       |
| 3   | Keeping product technology out of a role body     | Business       |
| 4   | Validating the keys                               | Technical      |
| 5   | The name a role signs its prose with              | Business       |
| 6   | Which unit of work a role reviews                 | Business       |

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
| `reviews`             | optional list of review units from the roles index; a reviewing body follows |

A persona is how a role signs the prose it writes: reports, findings, review
notes. Projection puts one generated line naming it ahead of the body and lists
every persona in [`PERSONAS.md`](../PERSONAS.md). A stamp a tool reads —
`last written by`, `author`, the § 12 Reviewer column — keeps the role name, so
no tool has to map a persona back to a role.

The review units — design, diff, surface, repository — are declared once, in
the same frontmatter under `review_units`, in the order their passes run, each
with what it reviews, when its pass runs and what the pass never does (B-009). A
role
declaring `reviews` carries the reviewing headings (B-008); which role reviews
which unit is generated into
[`sdd-conventions` § Review passes](../../skills/sdd-conventions/SKILL.md#review-passes).

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
- B-008: the real roster declares exactly the four passes; a synthetic role
  declaring a unit on a body without the reviewing headings fails `plan`.
- B-009: every real unit loads with all three keys; a synthetic unit missing
  `never`, or given as bare text, fails `plan` naming the unit.
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
| B-008    | A reviewing role carries the reviewing headings     | `F2_B008_ReviewingRoleCarriesLookForAndReport`           | Covered |
| B-009    | A review unit declares its trigger and refusal      | `F2_B009_ReviewUnitDeclaresTriggerAndRefusal`            | Covered |

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-16 -->

| Date | Trigger | Delta |
| --- | --- | --- |
| 2026-10-16 | User requested simpler roles and handoffs | Add B-010–B-036 and § 2 grouping/routing; retire six standalone roles by folding their duties into retained roles or modes; include `database`, reversing § 5 row 5's earlier exclusion; record the choice and rejected alternatives in [decision 0001](decisions/0001-lead-and-specialist-collaboration.md). — Ruth |

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-14 -->

| ID   | Question                                                                                   | Blocks | Resolution |
| ---- | ------------------------------------------------------------------------------------------ | ------ | ---------- |
| OQ-1 | Should the contract require a `## Report` heading for reviewing roles, as `spec-reviewer` and `architect` already carry? | B-003  | Resolved 2026-10-14: yes. A body with `## Look for` also carries `## Report`; claimed as B-005. |

## 12. Sign-off

<!-- last written by: spec-author, 2026-10-16 -->

| Section | Status | Reviewer      | Note                                                            |
| ------- | ------ | ------------- | --------------------------------------------------------------- |
| 1-12    | 🟡     | spec-reviewer | Baseline draft. Preserve B-004 review evidence: grep at scaffolding and second-wave roster (2026-10-14) found no product technology in role bodies. |
| 1-5, 7-10, 12, scenarios, decision 0001 | 🟢 | spec-reviewer | Agreement-only amendment reviewed 2026-10-16; § 7 scopes baseline readiness, §§ 8–9 record pending coverage; implementation remains pending. |

## Tasks

None cut. The first six roles landed with the scaffolding change and the second
wave of eighteen with the roster change; a new role or a contract change cuts
its own work item.

## Scoring

| Field | Value | Basis                                              |
| ----- | ----- | -------------------------------------------------- |
| value | 0     | Not scored: tooling agreement, not a product Feature |
| risk  | 0     | Not scored: tooling agreement, not a product Feature |
