---
title: Contract audit follow-ups
description: Verified follow-up evidence and decisions after the reviewed contract corrections
type: working-note
---

# Contract audit follow-ups

## Committed baseline

Reviewed item [0046](../../.issue/0046-contract-audit-regression-tests.yml)
initially spanned 20 local commits ending at `56c615d`; 23 reviewed follow-up
commits end at `2997ac3`. Its delivery remains in progress
until merge; its verification and review evidence stays in the item.

## Tracker reconciliation

The owner approved nine evidence-backed closures and the one-time published-ID
correction on 2026-10-17. Historical identities resolve through
[the alias index](../../.issue/README.md). The queue moves from 33 done,
10 in progress and one blocked to 42 done, one in progress and one blocked.
No Feature status or verification gap changes through these item closures.

| Items | Verified delivery |
| --- | --- |
| 0036 role handoffs | [PR #1](https://github.com/RLittlesII/agents/pull/1) |
| 0047 Next-work rulings, 0037 design | [PR #2](https://github.com/RLittlesII/agents/pull/2) |
| 0039 installer | [PR #3](https://github.com/RLittlesII/agents/pull/3) |
| 0040 collaboration | [PR #4](https://github.com/RLittlesII/agents/pull/4) |
| 0041 schemas | [PR #6](https://github.com/RLittlesII/agents/pull/6) |
| 0043 baseline, 0044 coverage | [PR #7](https://github.com/RLittlesII/agents/pull/7) |
| 0045 technology decisions | [PR #8](https://github.com/RLittlesII/agents/pull/8) |

GitHub merge metadata, local ancestry and each item's presence at its merge
were verified. Historical item evidence retains platform and live-agent limits.

## Follow-up status

| Follow-up | Verified gap | Next decision or evidence |
| --- | --- | --- |
| Feature coordination records | Seven unique sources created as 0048–0054 under the approved migration. | Five mirrors synchronized; remaining completion gaps stay explicit and no Feature is closed. |
| Tracker metadata placement | User selected consistent exclusion; source/installer implementation passes 71 projector and 30 installer tests. | Both independent reviews approved; coverage passes 49/50 (98%). Real coordination metadata stays outside the runtime plans. |
| Upstream templates | Agreement committed in specht as `cfde34d6`, targeting owner-confirmed unpublished `0.2.0`. | Implementation and distributed adoption remain outstanding; no downstream template edits. |
| Existing consumer installations | Installer does not prune obsolete files. | Outside this audit continuation by explicit user instruction; no consumer inspection or edits. |
| Actual agent behavior | Static instruction reviews do not establish live compliance. | [Nine routing probes passed](handoff-evaluation-results.md), including an H9 fixture clarification; real workflow and host enforcement remain unverified. |

## Upstream template evidence

[0007](../../.issue/0007-template-stamp-owner.yml) already tracks wrong section
owners. The local tool pin is `specht.tool` 0.1.90. Its
[`feature.md`](../../.specht/templates/feature.md) also embeds tracker and
framework assumptions:

| Template location | Finding | Proposed upstream scope |
| --- | --- | --- |
| Sections 6–7 | Stamps and guidance name implementation as design owner. | Neutral or consumer-configured section owners. |
| Section 12 | Recorder ownership is assigned to the reviewer. | Distinguish the section recorder from independent reviewers. |
| Lifecycle guidance and Tasks | GitHub labels and issues are assumed. | Obtain tracker and mirror mapping from consumer guidance. |
| Frontmatter guidance and Scoring | Priority is both authored and derived. | One unambiguous ownership rule for priority. |
| Section 8 | Reqnroll is assumed for acceptance tests. | Obtain the testing stack from consumer guidance. |
| Section 12 | Approval guidance omits changed-scope applicability and separate history. | Preserve verdict history while checking current applicable approvals. |

The sibling specht checkout still carries these assumptions in embedded 0.1.0
and 0.2.0 templates. Its AGENTS.md forbids changing published versions in place;
an upstream fix must follow its versioning and upgrade agreement. Existing
unrelated sibling edits were observed and left intact. GitHub issues are
disabled for `RLittlesII/specht`; no upstream request has been sent.

Potential automated checks include configurable-owner initialization, tracker
and framework neutrality, consistent priority guidance, preserved versioned
snapshots and pin/upgrade behavior. They require an upstream agreement before
implementation; these notes do not authorize a schema change.

## Legacy coordination migration

The user approved "Create the sources and synchronize mirrors" on 2026-10-17.
Seven explicit sources now start at the approved `in-progress` state. Source-first
synchronization replaces five legacy `ready` mirrors and preserves two existing
`in-progress` mirrors. Specification maturity is unchanged; no Feature completion
is inferred from delivered children.

| Feature | Authoritative coordination source | Existing delivery children | Remaining completion evidence |
| --- | --- | --- | --- |
| 0001-F1 Projection | [0048](../../scripts/.issue/0048-coordinate-projection.yml) | 0029, 0041 | Baseline review and unmerged audit amendments. |
| 0001-F2 Role contract | [0049](../../agents/.issue/0049-coordinate-role-contract.yml) | 0028, 0040 | Baseline review, 32 Partial instruction rows and unmerged audit amendments. |
| 0001-F3 Commands | [0050](../../commands/.issue/0050-coordinate-commands.yml) | 0025, 0026, 0027 | Baseline review and unmerged audit amendments. |
| 0001-F4 Skill boundary | [0051](../../skills/.issue/0051-coordinate-skill-boundary.yml) | None adopted | Unmerged metadata amendment and prerequisite F2 completion. |
| 0001-F5 Next work | [0052](../../skills/next/.issue/0052-coordinate-next-work.yml) | None adopted | Baseline review and 46 Missing traceability rows. |
| 0001-F6 Repository verification | [0053](../../.github/.issue/0053-coordinate-repository-verification.yml) | 0044 | Reconcile the recorded Linux CI execution gap before declaring Feature completion. |
| 0038-F1 Host installation | [0054](../../install/.issue/0054-coordinate-host-installation.yml) | 0039 | Unmerged runtime/metadata amendments and recorded platform evidence gaps. |

This legacy source-record migration authorizes no new child work against
unapproved agreement scope. Nine explicitly specification-linked typed tasks
retain their IDs, types, delivery states, criteria, prior decisions and evidence;
their parent links now point reciprocally to their coordinators. Root chores and
other root history retain their existing parents and scope. Feature task indexes
identify the source, children and relevant supporting history.

Each source records maintenance ownership and provisional source-record planning
estimates. Those estimates do not size unfinished claim work; remaining Feature
scope and closure costs still need grooming before a completion score is offered.
No derived score or gate is stored. Feature dependency edges map to the explicit
coordination IDs, preserving their original specification meaning.

The queue changes from 44 records (42 done, one in progress, one blocked) to 51
records (42 done, eight in progress, one blocked). Seven in-progress records are
coordinators; audit item 0046 remains the sole taken delivery assignment.

Verification uses YamlDotNet against all 51 records, checking unique sources,
136 active-claim identities, nine reciprocal child links, source/mirror equality,
resolved acyclic dependencies and preserved prior content, maturity and history.
All 71 projector and 30 installer tests pass with these records present. Projection
still has 80 current outputs; offline plans remain Claude 39 files and Codex 68,
with no tracker metadata in either runtime outputs or manager input. No executable
source or test changed during the migration; the prior 49/50 coverage result remains
applicable. Strict specifications and whitespace checks pass.

Independent migration review: `/root/contract_design_diff_review` approves the
diff and `/root/contract_surface_review` carries surface approval at subject
`9ebe99964d0fcca955bc3dba521dadbeb200fd0a682ecaa7946d8be6093f9e13`.
All 45 hashes and the semantic checks were independently verified. Provisional
source-maintenance estimates do not become Feature completion estimates or
permission to start a child.

## Return to handoff findings

The upstream agreement is committed; no upstream implementation is part of this
audit continuation. [The evaluation plan](handoff-evaluation-plan.md) records
nine executed routing probes against existing claims. No new contract policy,
installed-file removal, coverage upgrade or Feature closure is inferred.

## Baseline verdicts and bounded corrections

The user authorized recording the three returned baseline approvals and fixing
baseline findings in this repository. F2, F3 and F5 retrospective content
approvals at `2997ac3` are recorded in their §12 histories; revised Projection
agreement/design also received independent approval before dependent tests.
Consumer projects and specht remain outside this continuation.

The bounded correction covers reference-link validation, complete stale-output
inventory, source-qualified role diagnostics, native/linked output-path safety,
Projection contract/scenario alignment, and Next-work lane/reservation guidance.
Regression execution and final diff/surface verdicts are recorded in Projection
§8/§12 and the owning Features’ §12 rows. The original 71-test baseline passed;
11 of 83 expanded cases failed before correction. A planned-file control brought
the first passing full gate to 84 tests. Subsequent surface review exposed Unix
path aliasing; four further controls failed before its native-path correction.

These content reviews remove the four baseline-review gaps in the migration
table above. Completion still requires the audit amendments to merge, the 32
Partial F2 rows and 46 Missing F5 rows to gain adequate evidence, and each
Feature’s other recorded prerequisite/platform evidence. All seven coordinators
and Feature delivery mirrors remain in progress. No Feature is declared complete.

Final local gate passes 88 F# tests, six compiled CLI cases, 30 installer tests
and 36 coverage-gate tests; changed production coverage is 88/89 (98.88%).
Strict specifications and the 80-output projection check pass. Surface review
Approved the implementation at `6edf5897` with no remaining findings.
Final diff review also Approved at `6edf5897`, including inspected recording-only
corrections, with no remaining findings. Unchanged F1/F3 and F2/F5 approvals
remain applicable. These changes are local and uncommitted.
