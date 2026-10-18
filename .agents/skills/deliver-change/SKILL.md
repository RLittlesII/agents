---
name: deliver-change
description: Deliver a change through a work item, a branch, the specification, the build, and a pull request. Use when picking up work, filing an item, committing, or opening and watching a pull request.
---

# Deliver a change

**Project rules.** This skill is portable. The method's shared facts — id
schemes, section ownership, spec layout — are in
[`sdd-conventions`](../sdd-conventions/SKILL.md). The consuming repository's
companion skill, `<project>-conventions` § "Delivery", names where this rule
lands there. Read all three.

## The shape of it

1. **A Feature starts at its specification.** Items are cut from its claims after
   agreement, not before. A bug, spike or chore starts at the item instead.
2. **Take exactly one item for tracked delivery.** Initial agreement authoring
   follows [`spec-and-traceability`](../spec-and-traceability/SKILL.md)
   § "Where a specification starts"; this item-taking guard does not apply
   before its items exist. Signal that you have an item in the tracker *before* the
   branch, before the worktree, and before the first edit. A taken item is not
   new work; continuation follows § "Resume, release and reassignment" below.
3. **One delivery child in progress at a time, per Feature.** A Feature's
   coordination record does not consume that delivery slot. Sequential prerequisites
   follow [`next`](../next/SKILL.md) § "Stage gates". A task that requires a
   sibling to be started alongside it, or an unmerged sibling branch to be
   stacked, returns to grooming for a cut that can finish on its own.
4. **Resolve questions blocking the assigned claims before dependent work.**
   Follow [`clarify-requirements`](../clarify-requirements/SKILL.md) for the answer
   and its record; a decision record cannot turn an unanswered material question
   into permission to proceed.
5. **The specification delta is part of the change.** Scenario and claim edits ship
   in the same pull request as the code, not after it. A correction that lives only
   in conversation does not survive to the next session.
6. **Cite the claim where the citation survives the merge.** Know which merge
   strategies the project uses before deciding where a citation goes: a squash or
   rebase merge can discard a pull-request body. The commit body carries
   `Satisfies <epic>-F<n> B-00n.` for each claim the commit delivers.
7. **Verify before you call it done.** Run the narrow check for what you touched,
   then the specification checker
   ([`sdd-conventions`](../sdd-conventions/SKILL.md) § "Spec layout"), then the full
   build. A green build proves only that the targets it declares
   ran — read what they actually cover. Each claim the change delivers has a
   § 9 row naming a test, not `Missing`.
8. **Open the pull request, then watch it.** A failing check is yours to fix, not
   the reviewer's to discover. Merging is the person's call: never merge without
   their explicit approval, however green the checks.
9. **Review feedback is a loop, not a conversation.** For each round: make the
   change, commit, push, then resolve every thread the commit addressed with a
   reply naming only that commit. Nothing else goes in the thread - the diff is
   the answer, and a reviewer who wants more reopens it. A comment you disagree
   with is answered in the thread before any resolve, and stays open.

## Resume, release and reassignment

A taken item retains its recorded owner across interruptions. Before resuming,
verify the latest ownership record, scope, checkout and branch, completed work,
remaining work and applicable stage evidence. The same owner may continue within
that assignment without requesting permission again. Continuation occupies the
existing delivery slot, not a second one.

Another owner may take it only after the recorded owner's explicit release or
an authorized reassignment. Record who released or authorized the change, the
old and new owner where applicable, and the handoff evidence before editing.
A release relinquishes ownership; reassignment replaces it. Neither completes
the item or satisfies its dependents. Preserve the prior events and unfinished
work; follow the companion's ownership record and maintenance operation.

Age, silence, an inactive session or a missing owner is not a release. Missing,
conflicting or unverifiable ownership returns for authoritative evidence or
reassignment; do not infer a takeover. Read the latest record again before
continuing if ownership may have changed. Transfer grants no new artifact
permissions and does not bypass agreement, design or testing gates.

## Exemptions

A behaviour change that touches no scenario is suspect. An exemption is claimed by
**citing the constraints the change preserves** — a closed category (refactor,
rename, formatting, dependency bump), a real reason, and the existing constraint
ids that still hold. Reaching for an exemption because writing a scenario feels
slow is the one use of it that is forbidden outright. Nothing automates this
check; review enforces it.

The format is fixed, in the commit body:

```text
No .feature change needed: <category> - <what changed and what still holds>.
Constraints preserved: <epic>-F<n> C-<n>, <epic>-F<n> C-<n>
```

## Behavior-preserving refactors

A refactor changes structure while preserving the existing behavior and public
contract. Its tracked item bounds the affected code and cites the constraints
it preserves. Use the exemption format above rather than claiming delivery of
new behavior. Existing design and approval requirements still apply.

Before production changes, inspect the affected contract and run the applicable
tests. Record why those tests adequately cover the behavior being preserved.
Adequate passing tests are the entry evidence; a new claim or deliberately
failing test is not required for unchanged behavior. Test gaps return to the
owner of § 9 for characterization of existing documented behavior, observed
passing before the refactor begins.

The implementation lead integrates bounded contributions using that evidence;
each contributor retains its existing artifact boundaries. Run the applicable
tests and regression check again before handoff. A failing baseline returns to
investigation. A change to behavior or an externally consumed contract leaves
this path and follows the normal agreement, design and failing-test gates.

## When a bug fix reveals a gap

The fix ships with a lesson in the same pull request: symptom, root cause, the
specification delta, and the claim that now proves it. A fix that reveals nothing
writes no lesson. A **process** lesson also updates the skill that would have
prevented it — the skill holds the rule, the lesson keeps the incident.
A **product** lesson changes the claim and scenario, not a skill.

### Before a lesson is written

A lesson that teaches a fabricated incident or misquotes the code it is about is
worse than none: later reviews cite it with the record's authority. Clear four
checks, and name the evidence in the lesson.

1. **The incident happened.** Find the string or symbol it turns on in history
   and cite the commit or line. A string that appears only in the commit adding
   the lesson is a fabrication. An incident outside the repository says so.
2. **Quoted code is copied, not recalled.** Every identifier, path and signature
   is copied from the current source.
3. **The lesson does not commit the mistake it warns against.** Read it once as
   its own reviewer.
4. **Every claim about another document is checked against it**, and the line
   cited. The specification delta names a claim that exists.

A forwarded finding shows that something happened, not what happened: its
framing is a claim to check like any other.

Search the recorded lessons for the topic first: a lesson already recorded gets a
correction, not a duplicate. A correction is appended, dated, below the original;
the original is never edited, and the correction re-runs all four checks on the
whole lesson. A repeated mistake records why the existing lesson did not prevent
it: missing, misfiled, or not consulted. A lesson no correction can save, such as
one teaching a fabricated incident, is marked withdrawn where it stands and
superseded by a lesson about a real one.

## Never add

- A second item in progress alongside the first.
- A takeover inferred from elapsed time, silence or missing ownership evidence.
- A branch or an edit for tracked delivery before the item is signalled as taken.
- A claim, a status, or a rule duplicated into the item from the specification.
- A scenario edit deferred to a follow-up pull request.
- A skipped phase because the change "is small". Small changes skip roles, not
  the ordering.
- A green build read as proof of coverage.
- A review thread resolved without a pushed commit, or with prose beyond the
  commit that addressed it.
- A merge without the person's explicit approval.
- A commit labelled by the tool that touched it rather than what it changes.
- A commit spanning two sections' owners or two Features. Split it, and split a
  commit that has outgrown its label.
- A force-push. A rejected push is investigated first.
- A branch from anything but the trunk the companion names.
- A lesson edited in place, or filed twice.
