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

Follow the companion's identity, dependency, and ranking contracts. Count shared
prerequisites once within a Feature's remaining work and promote the work that
unlocks the selected Features. Do not substitute stored derived fields or invent
missing estimates, value assessments, claim assignments, or delivery state.

Evaluate each stage separately. Specification, decomposition, design,
investigation, and implementation can each be next when their own inputs permit
it. Say which step is actionable and which later steps remain blocked. A status
label alone does not supply missing agreement or design evidence.

Keep affected work unscored or withheld when evidence is incomplete, while
retaining independent verified choices. Explain cycles, missing links, decision
holds, and occupied work. A valid scope with no available start is an answer
with reasons, not an empty success claim.

## Use lanes to support the choice

Offer one ranked startable head per available lane. Check both dependencies and
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
the known child work is complete.

## Never add

- A taken item offered as a new start.
- An item-first answer when the request is to choose or finish Features.
- A fabricated score, estimate, approval, claim link, or delivery state.
- A lane split that ignores evidenced shared edits.
- A silent scope expansion or a claim that unread work does not exist.
- An issue edit, status synchronization, branch, worktree, or dispatch during
  recommendation.
