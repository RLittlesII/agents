---
title: "Decision 0022: Open lanes by interdependency"
description: "Sibling work runs in parallel unless one item waits on the other or their edits collide."
type: decision
---

# Decision 0022: Open lanes by interdependency

**Date:** 2026-11-24
**Decided by:** the repository maintainer, ruling recorded on Hooked issue
#887 (comments 7137957300 and 7140080845).

## The call

Feature ownership no longer limits starts. Two startable children of one
Feature are both offered, and both run, when neither waits on the other,
directly or transitively through `depends_on` or spikes, and their edits do
not collide. Sharing a prerequisite does not make two items wait on each
other. An item waits on another only when the stage it is offered for needs
it, so design still proceeds while an implementation prerequisite is
unfinished (B-012). A taken child blocks only the children that wait on it or
collide with its edits; the rest of its Feature stays startable. An active
Feature or epic container is coordination and reserves nothing by itself.

A shared file collides only when both items rewrite the same part of it. Two
designs that each add a new subsection or row to one specification section do
not collide, though they can still conflict on rebase; two items rewriting the
same rows or the same code do collide. A shared file that does not collide is
reported as a rebase risk. A collision joins only the two items that collide,
never a chain. When nothing shows whether two edits touch the same part, both
are offered and the possible rebase conflict is named; the lower-ranked one is
not held.

This supersedes [decision 0021](0021-one-start-per-feature.md) and the
"offered at most one start" sentence of
[decision 0011](0011-lanes-inferred-from-shared-edits.md); in its place, a
Feature is offered every start that passes the test above. Decision 0011's
inference from dependencies and shared edits, narrowed to same-part edits, its
uncertainty reporting and
[decision 0017](0017-work-in-review-holds-its-lane.md)'s review reservation
still hold. [`deliver-change`](../../../deliver-change/SKILL.md) § "The shape of
it", rule 3, changes with it: siblings may be in progress together under the
same test.

## Why

The maintainer's words: "The point of next is to OPEN lanes. Not restrict to a
single path!" and "Update the rule to check for interdependency. yes. if two
tasks won't step on each other ... they can be done in parallel." On the
uncertain case: "Offer both". One Feature had sat waiting on one task while five
more were ready.

## Rejected

Keeping one delivery child per Feature, which serialized independent work.
Treating every shared file as a collision, which blocks additive edits that
do not collide. Holding the lower-ranked item when the evidence cannot show a
same-part edit, which serializes work nothing shows will collide.

## Affects

Adds B-047 to B-051. Withdraws B-046. Carves, with their text unchanged: B-034
by B-050, which overrides its "subject to one start per Feature (B-046)"; and
B-035 by B-051, which narrows its shared edit to evidence that both rewrite the
same part. Resolves OQ-6 anew; changes `deliver-change` rule 3.

## Reversal

None.
