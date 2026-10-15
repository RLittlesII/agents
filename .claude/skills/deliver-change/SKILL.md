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
2. **Take exactly one item.** Signal that you have it in the tracker *before* the
   branch, before the worktree, and before the first edit. An item already
   signalled as taken is never picked up — that signal is the only thing stopping
   two people building the same thing.
3. **One item in progress at a time, per Feature.** A task that cannot be finished
   without a sibling finishing first is a decomposition problem, not a small task.
   Take it back to grooming rather than stacking branches.
4. **Do not start an item whose specification is unresolved.** Open questions that
   block the claims you are about to build are resolved first, or the exception is
   written down as a decision record naming what was assumed.
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
- A branch or an edit before the item is signalled as taken.
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
