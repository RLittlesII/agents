---
title: "Decision 0022: Open lanes that do not overlap"
description: "Candidates start in parallel only when they overlap neither each other nor taken work, by a dependency or any shared file."
type: decision
---

# Decision 0022: Open lanes that do not overlap

**Date:** 2026-11-25
**Decided by:** the repository maintainer, rulings recorded on Hooked issue
#887 (comments 7137957300 and 7140080845) and on agents#17 (2026-11-25, "Add
no-overlap check").

## The call

Feature ownership no longer limits starts; overlap does. Two candidates overlap
when either depends on the other's result, directly or transitively through
`depends_on` or spikes, for the stage it is offered for, or when both will edit
any of the same files: the same specification file, the same work record, or
shared generated or configuration output. Sharing a prerequisite is not overlap
by itself, so design still proceeds while an implementation prerequisite is
unfinished (B-012).

Only candidates that overlap neither each other nor any taken work, in review
too, start in parallel. A candidate that overlaps taken work or a higher-ranked
head queues behind that lane and starts only once the work it overlaps has
merged. An active Feature or epic container is coordination and reserves
nothing by itself.

Overlap is read from evidence: the files each open branch or pull request
touches, diffed against its merge-base, and the files each candidate's claims
and records name. A candidate whose overlap cannot be determined, from its own
files or from an open branch, pull request or taken item, is treated as
overlapping, and the report says why. Unscored and standalone work skips the
ranking but not this test. The report names each overlap, its
evidence and the lane that holds it. A later rebase is never a reason to start
overlapping work.

This supersedes [decision 0021](0021-one-start-per-feature.md) and the
"offered at most one start" sentence of
[decision 0011](0011-lanes-inferred-from-shared-edits.md); in its place, a
Feature is offered every start that passes the test above. Decision 0011's
inference from dependencies and shared edits, its uncertainty reporting and
[decision 0017](0017-work-in-review-holds-its-lane.md)'s review reservation
still hold. [`deliver-change`](../../../deliver-change/SKILL.md) § "The shape of
it", rule 3, changes with it: any item may be in progress alongside other work
only when it does not overlap that work.

## Why

On Hooked issue #887 the maintainer said: "The point of next is to OPEN lanes.
Not restrict to a single path!" and "Update the rule to check for
interdependency." Applying a dependency check alone then let four pull requests
run in parallel that all edited one specification file, and they conflicted on
every merge. The maintainer's ruling on agents#17: "Maybe now we get why the 1
item per feature restraint was put in and what I meant by NO OVERLAP." File
overlap keeps the parallelism the first ruling asked for without the conflicts.

## Rejected

- Keeping one delivery child per Feature, which serializes work that touches no
  common file.
- A dependency check alone, treating a shared file that both edit in separate
  parts as a rebase risk; it produced the repeated conflicts above.
- Offering both candidates when overlap is unknown, which starts work that
  nothing shows is safe.

## Affects

Adds B-047 to B-052. Withdraws B-046. Carves B-034 by B-050, which overrides
its "subject to one start per Feature (B-046)", with B-034's text unchanged.
B-035's evidenced shared edit still joins two lanes; B-049 and B-051 extend it
to any shared file and to overlap that cannot be determined. Resolves OQ-6
anew; changes `deliver-change` rule 3 and the companion's `next` evidence. As a
consequence of rule 3, the `deliver-change` Never-add line "A second item in
progress alongside the first." becomes "A second item in progress for the same
owner.": parallel items are now allowed across owners when they do not overlap,
and one owner still holds one item at a time.

## Reversal

None.
