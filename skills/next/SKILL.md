---
name: next
description: Compare Features and their completion paths to recommend the next actionable work, with parallel lanes as supporting detail. Use when asked what is next, which Feature to focus on, how to finish selected Features, or what can run in parallel.
---

# Choose the next work

**Project rules.** This skill is portable. The method's shared facts — id
schemes, section ownership, spec layout — are in
[`sdd-conventions`](../sdd-conventions/SKILL.md). The consuming repository's
companion skill, `<project>-conventions` § "Next work", names where this rule
lands there. Read all three.

## Start with the Feature choice

Compare the Features the person can drive to completion. Name each Feature and
its stable ID, importance, remaining work, blockers, completion path, and next
available step. Keep importance, proximity to completion, and blocker counts
distinct; a ranking score does not answer all three questions.

Respect the requested Feature or Feature set. Include its required prerequisites
even when another Feature owns them, explaining what they advance. Keep unrelated
work outside that focus. An unknown requested Feature is a scope error, not a
reason to widen the answer.

Read the complete relevant queue before comparing Features. Include external
reservations that can affect the chosen scope. Use one set of observed inputs
for the comparison, scores, paths, and lanes; identify when those inputs were read.

## Derive an evidenced next step

Follow the companion's identity and dependency contracts, the stage gates and
the ranking model below. Count shared
prerequisites once within a Feature's remaining work and promote the work that
unlocks the selected Features. Do not substitute stored derived fields or invent
missing estimates, value assessments, claim assignments, or delivery state.
Read delivery state from the tracker only. A mirrored status that disagrees with
it is a diagnostic, and an unreadable tracker leaves the affected work with no
start. Two records claiming one stable id are named as a duplicate; choose
neither, and withhold the work that depends on either.

Evaluate each stage separately. Specification, decomposition, design,
investigation, and implementation can each be next when their own inputs permit
it. Say which step is actionable and which later steps remain blocked. A status
label alone does not supply missing agreement or design evidence.

Keep affected work unscored or withheld when evidence is incomplete, while
retaining independent verified choices. Explain cycles, missing links, decision
holds, and occupied work. A valid scope with no available start is an answer
with reasons, not an empty success claim.

## Stage gates

Read approval from the § 12 rows by section, and open questions from § 11 by
the claims they block. `spec_status` alone is not approval evidence, and an
unrelated question blocks nothing. An open question whose row names no claim leaves its
Feature's design and implementation readiness unverified: withhold both and name
the gap. A row reading `None` names that it blocks nothing.

| Stage            | Next step, and the evidence it needs                                      |
| ---------------- | ------------------------------------------------------------------------- |
| Intake or decomposition | author or split the agreement; do not demand the approval it will produce |
| Design           | §§ 1-5 approved for the linked claims, no open question blocking them    |
| Implementation   | §§ 1-7 approved and the design Ready for the linked claims, no question blocking them, and every prerequisite delivered |
| Spike            | a bounded question and a time box; the status alone authorizes nothing more |
| Taken            | occupied, in review too: show it and reserve its lane until delivered; offer no new start there |

Delivered means finished as completed. Work in review, closed as not planned or
reopened does not satisfy a prerequisite. Design can proceed while
implementation prerequisites are unfinished.

Waiting is derived. An item whose `depends_on` or spikes are unfinished is
waiting, whatever its status says; never set a status to record it. A container
with unfinished children is never offered itself: its children are the
candidates. A dependency cycle is reported with its members, which stay
unscored, and routed to grooming as a re-cut
([`spec-and-traceability`](../spec-and-traceability/SKILL.md) § "Decomposition");
the rest of the queue is still ranked.

## The ranking model

One model, so two repositories rank the same queue the same way. The companion
names where each input is authored; it does not change the arithmetic.

1. **Priority tier first.** Order by the Feature's priority tier, highest first.
2. **Then the completion score:** the Feature's value divided by the summed
   cost of its unfinished closure — its open children and every open item they
   wait on, in any Feature. Each item counts once per closure, at full cost
   until it is delivered, even in progress or in review.
3. **A prerequisite inherits** the strongest tier of a selected Feature it
   advances, then the best score among those in that tier. An unselected
   Feature never changes a focused recommendation.
4. **Ties** break on the number of open items waiting on the item, most first,
   then its stable id, lowest first. Compare exact values; round only for
   display.

Missing value, missing cost, unknown decomposition or a cycle leaves a score
uncomputed — never zero, never infinite. An active claim with neither planned
work nor verified delivery is unknown scope: name the gap and leave the Feature
unscored. No derived tier, score or gate is written back to a record.

## Use lanes to support the choice

Offer one ranked startable head per available lane. Offer at most one start per
Feature, since one item is in progress per Feature: its other startable items
wait behind the head, and a Feature with taken work offers none. Check both dependencies and
shared edits: independent tasks can still collide in a specification or generated
output. Apply existing reservations before narrowing the displayed scope.

Explain each inferred conflict and its uncertainty. Separate actionable unscored
work from ranked lane heads; lack of ranking inputs is not evidence of blockage.
Keep standalone work visible as the companion directs.

## Report without taking work

Lead with Feature comparisons and completion paths, then supporting lanes,
blockers, occupied work, unscored work, and diagnostics. Identify the target
Feature and the owner of each recommended prerequisite. Expand detail on request
without recalculating different choices from the same inputs.

The person chooses from the recommendations. Asking what is next does not claim
an item or authorize starting it. Inspect both the agreement and delivery record
before calling a Feature finished; a completed child list alone proves only that
the known child work is complete. A Feature with no unfinished closure and every active
claim delivered is finished: report it so and leave it out of ranking.

## Never add

- A taken item offered as a new start.
- An item-first answer when the request is to choose or finish Features.
- A fabricated score, estimate, approval, claim link, or delivery state.
- A lane split that ignores evidenced shared edits.
- A silent scope expansion or a claim that unread work does not exist.
- An issue edit, status synchronization, branch, worktree, or dispatch during
  recommendation.
