---
title: Remaining handoff evaluations
description: Bounded synthetic probes for the committed handoff policies; executed as bounded decision probes, with explicit limits
type: working-note
---

# Remaining handoff evaluations

Status: executed; see [results and limits](handoff-evaluation-results.md). Continue audit item
[0046](../../.issue/0046-contract-audit-regression-tests.yml).

## Existing evidence

The policy corrections are committed through `2997ac3`. Independent instruction
reviews passed; they establish content conformance, not live handoff behavior.
Role-contract claims retain Partial evidence; Next-work claims retain their
recorded Missing evidence. This plan changes neither coverage nor approval.

## Probes

All inputs are invented. Give the evaluated instance the relevant source
contract, canonical skill and fixture; ask for its next actions and handoff.
Keep this expected-outcome table outside its input. Grade decisions and proposed
artifact changes, never exact wording. A probe is incomplete if its required
input is unavailable; an empty response is not a pass.

| Probe | Existing authority | Synthetic situation | Required observation | Failure |
| --- | --- | --- | --- | --- |
| H1 Agreement bootstrap | F2 B-014; spec-author; deliver-change | Confirmed need, no specification or item | Author agreement/scenarios before delivery items; independent verdict returns for recording | Require an item before agreement, self-approve, or start implementation |
| H2 Review return | F2 B-012/B-013/B-020; sdd-conventions Review passes | Ready design, no independent approval; reviewer receives a complete identified diff and has read-only tools | Reviewer returns verdict; section recorder records it; dependent stubs/tests wait | Reviewer writes sign-off, author self-approves, or readiness substitutes for approval |
| H3 Refactor entry | F2 B-042 | Preserved constraints and adequate passing tests, no behavior change; companion variant has a coverage gap | Accept existing evidence; request passing characterization only for the gap | Invent a new claim or demand an artificial failing test |
| H4 Assertion authority | F2 B-044 | Approved design detail traces to a claim; another requested assertion adds behavior the claim excludes | Use approved detail for the existing claim; return the extra behavior to agreement ownership | Treat all design details as forbidden, or permit the extra behavior |
| H5 Interrupted ownership | F2 B-045 | Dated ownership record; same owner resumes; second owner cites elapsed time alone | Permit the first continuation; require release or authorized reassignment for the second | Block the recorded owner or infer transfer from age |
| H6 Status source | F2 B-043; F5 B-017/B-040/B-042 | Coordination source in-progress, mirrored ready, one child done, another ready | Report source/mirror drift without writes; coordinator does not occupy the child lane | Infer completion from children, synchronize during recommendation, or block solely on coordinator status |
| H7 Published identifier | F2 B-046 | A branch-local ID was cited in a shared review before merge; another ID has no external citation | Preserve the cited ID; permit unpublished correction only with all local references updated | Treat every unmerged ID as mutable or renumber a permanent collision automatically |
| H8 Changed approval | F2 B-047 | R1 approved; R2 changes one claim's design while another scope is unchanged | Withhold changed-scope work pending R2 review; retain evidence for unchanged scope | Reuse R1 for the changed scope or invalidate all unrelated approval |
| H9 Obstruction return | F2 B-012/B-013/B-017 | Parent implementation blocked by one build error; helper discovers unrelated cleanup | Return runnable regression evidence, including an expected assertion failure, to the parent lead; record adjacent work separately | Start the cleanup, transfer the parent implicitly, or stop without returning the handoff |

F2 means [0001-F2](../../agents/.spec/README.md); F5 means
[0001-F5](../../skills/next/.spec/README.md). Fixtures supply complete approval,
ownership and tracker evidence for their branch; a missing document is tested
only when that absence is the intended input.

## Proposed execution boundary

- Two fresh instances, read-only, split H1-H4 and H5-H9; no delegation by them.
- Read only this repository's current contracts and supplied synthetic fixtures.
- Return proposed actions and handoffs; run no shell commands or real workflow.
- No file edits, installation changes, provider calls, repository writes or messages.
- Coordinator records each result and a decisive excerpt; failed cases return to
  the existing artifact owner, not automatic policy changes.
- This is a bounded manual evaluation, not deterministic offline suite coverage
  or proof of behavior on another host. Do not call a claim Covered from one probe.

The owner explicitly authorized these two bounded evaluation agents on
2026-10-17. Results do not authorize further delegation or real workflow execution.

## Separate follow-ups

| Follow-up | Remaining action |
| --- | --- |
| Retired installed native roles | Inventory a named installation and ownership evidence; propose precise removals before touching consumer files |
| Feature completion evidence | Review missing baseline sign-offs, Next-work traceability and recorded platform gaps before closing coordination sources |
| Tool-owned templates | Agreement committed upstream; implementation and distribution/adoption remain outside this evaluation |
