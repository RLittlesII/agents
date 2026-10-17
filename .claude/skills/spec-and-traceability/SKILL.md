---
name: spec-and-traceability
description: The specification model — a specification starts before its work items, one record holds content and another holds delivery state, and every claim traces through a scenario to a test. Use when authoring, amending, or reviewing a specification, a claim, or a work item.
---

# Specification and traceability

**Project rules.** This skill is portable. The method's shared facts — id
schemes, section ownership, spec layout — are in
[`sdd-conventions`](../sdd-conventions/SKILL.md). The consuming repository's
companion skill, `<project>-conventions` § "Specs", names where this rule
lands there. Read all three.

## Where a specification starts

A Feature begins with its **specification**, authored with no work item in
existence. The specification is the agreement; items are cut from its claims
afterwards, once a stakeholder has agreed to it. That ordering is what lets a
specification be argued with before anyone has spent a day building from it.

A bug, a spike or a chore begins at the item instead, and may produce a
specification delta afterwards.

## An epic states an outcome

An epic scopes a set of Features and names the result they produce. It holds no
claims, no acceptance criteria and no scenarios; those are each Feature's. An
epic that grows a claim list is an oversized Feature.

- **The measure is written before the first Feature.** How the result is
  observed, and what value would mean it arrived. An outcome nobody can measure
  is an ambition, and nothing downstream can tell whether it was delivered. The
  epic record starts from the blank the companion names.
- **The boundary is an exclusion list.** Each exclusion names where that work
  goes instead, when it goes anywhere.
- **A Feature is filed under an epic once the person confirms it belongs.**
  The epic names its Features; a Feature names its epic in one field and never
  restates its outcome.

## Feature naming

A Feature name joining nouns or gerunds with "&" or another conjunction is a
decomposition smell, not an automatic split. At naming, grooming and migration,
check whether it joins independent capabilities or intentionally names one.
Keep or split the Feature according to that boundary, not the wording alone.

## Decomposition

Three tiers, and only the last is handed on to be built:

| Tier     | Grain                                                        |
| -------- | ------------------------------------------------------------ |
| Epic     | a subsystem or theme; a coordination record                  |
| Feature  | one capability dimension of it, deliverable on its own; a coordination record |
| Child    | one responsibility that can be implemented, reviewed and merged |

A unit is oversized when any of these trips, read from the raw ask for an epic
and from § 3 for a Feature:

1. More than three acceptance criteria with no shared dependency. Criteria
   resting on one change count once.
2. Two or more deployment or ownership boundaries crossed. Layers inside one
   boundary are one.
3. Five or more steps that could each merge on their own without leaving the
   system broken.
4. Unstarted across two grooming horizons. This is staleness, not size: if
   none of 1 to 3 trips, rescore it rather than cut it.

Cut one tier down, never two. An epic is cut along capability dimensions, not
layers, phases or single changes: a Feature delivers value with its siblings
unbuilt, fills its own criteria, constraints and exclusions, and survives a
sibling leaving scope. A Feature whose scope reads as one verb phrase ("add X")
is a child. A Feature is cut into typed children:

| Type    | Cut out when                                                   |
| ------- | -------------------------------------------------------------- |
| `task`  | a criterion needs implementation                               |
| `test`  | verification is big enough to land apart from the code it covers |
| `bug`   | a defect surfaced in the work that is not new scope            |
| `spike` | a design question blocks estimating or building the rest       |

A cut that yields a single child is a smell: the parent was already one
responsibility, or the cut missed a dimension.

A dependency cycle means two units were cut across a boundary that does not
hold. No edge is dropped to break it; it is re-cut, trying these in order:

1. **Merge.** The members were one unit of work; they become one child.
2. **Extract.** The shared part becomes a new unit both depend on.
3. **Narrow the edge to the child grain.** When the cycle sits between Features
   and the split is sound, move the dependency off the Feature and onto only the
   children that need it.

The re-cut is its own commit, and the cycle check runs again before any order is
trusted.

## Two records, one authority each

| Record            | Authority over                                        |
| ----------------- | ----------------------------------------------------- |
| The specification | content — goal, needs, claims, constraints, scope, design, coverage |
| The work item     | delivery — who has it, what state it is in, when it closed |

Neither restates the other. The item names its specification; **the
specification never names an item** beyond the Tasks list it cuts, because a
specification outlives any number of items.

Where one field is genuinely needed in both places, exactly one of them is the
source and the other is **mirrored** — copied from that source, never
independently authored on the mirror side.
[`sdd-conventions`](../sdd-conventions/SKILL.md) § "The spec / work-item split"
says which fields those are; the companion names where they land.

## Filling the blank

Guidance lives in the template. A copy carries content only: delete the
instruction comments, and never leave a `{{placeholder}}` or an empty table row
behind. A placeholder row in a coverage table is worse than an absent one — it
reads as a fact.

## One answer, one section

A derived answer is stored once. If § 9 can be read off § 3 and the test files,
then § 9 is where it lives and no other section re-summarises it. Two copies of a
derived fact drift silently, and the reader cannot tell which is stale.

## A declaration belongs to the file that compiles

Before code exists, a specification may state a signature to agree on it. Once
the code exists, **the compiler is the authority** — the specification references
the declaring file instead of restating the signature. A restated signature is
guaranteed to go stale and will be believed anyway.

## Claims and traceability

- A **claim** is one falsifiable statement, numbered, scoped to its Feature.
- Claim IDs follow [`sdd-conventions`](../sdd-conventions/SKILL.md) § "ID schemes":
  publication makes them permanent. A private draft is not proof that an ID has
  never been cited outside its authoring branch.
- A withdrawn claim is **marked** withdrawn, not deleted. Its number stays
  retired.
- Each scenario carries the tag of the claim it documents. The tag is the anchor;
  a scenario title is a human-reading courtesy and a stale one is a cleanup, not
  a gate failure.
- The **traceability matrix is the gate**: every claim appears there exactly once.
  A row whose test is missing blocks the merge that delivers its claim and the
  item reaching done. It does not block the specification being agreed — those
  are different questions.
- § 9 Status is one of five values:

  | Status      | Means                                                          |
  | ----------- | -------------------------------------------------------------- |
  | `Covered`   | a scenario and a test pin the claim and its mechanism          |
  | `Partial`   | a scenario covers it; the mechanism beneath has no unit test   |
  | `Missing`   | no test yet                                                    |
  | `Withdrawn` | the claim was withdrawn; the row stays as history              |
  | `Waived`    | deliberately unverified, with a § 11 row naming who approved shipping it and when |

  An undocumented `Waived` is a `Missing`. A withdrawn claim's scenario tag is
  removed in the same edit.
- Cite a row, a claim or a section by its id, never by its position.

## Amending an approved specification

An approved specification is accountable, not frozen. A change follows a path
rather than an edit:

1. Record the reason in § 10: the date, the trigger and what changed.
2. A changed behaviour is a claim with a **new** id; the old one is withdrawn.
   A behaviour that is not a claim cannot be traced, tested or signed off.
3. Add its § 9 row as `Missing`.
4. Return `spec_status` to an unapproved review state accepted by the project's
   specification schema. Add pending § 12 coverage for the changed scope and
   revision under § "Approval applicability", preserving earlier verdicts.
5. A reworded claim that items already cite changes their brief: name those
   items in the change.

A Feature re-cut rather than amended is superseded: its specification names the
replacement in § 10 and stays in place, never deleted.

Rename a claim's subject, a type or a section only once a search for the old
name finds nothing left to update, and update every diagram that shows it in the
same edit.

## An imported rule is a decision, not a constraint

An external rule the project chose to adopt — a vendor's requirement, a
platform's convention, an upstream skill's pattern — is a **decision record**
naming what it costs and what was accepted, not a constraint row. Constraints are
what the problem imposes; decisions are what the project chose. Filing a choice as
a constraint hides that it could be revisited.

## Numbers from independent sequences collide

Several id schemes coexist, each with its own sequence. Always carry the scheme
prefix in prose: a bare "12" is unresolvable when claims, constraints, open
questions and tasks all have a twelfth member.

## Section ownership

Each section has exactly one owning role. A role that needs another's section
changed **escalates**; it does not write there. Filling someone else's section
removes the only signal that the preceding stage is incomplete. The ownership map
is written in one place — [`sdd-conventions`](../sdd-conventions/SKILL.md)
§ "Section ownership" — and nowhere else.

### Approval applicability

A verdict identifies its review unit, scope and subject revision: a commit or an
identified working snapshot sufficient to recover what was reviewed. The owner
of § 12 records the returned verdict and evidence without supplying an approval
of its own.

A substantive agreement or design edit requires review of the changed scope
against its new revision. Include dependencies whose change affects what the
verdict established; unchanged text alone does not prove unchanged scope.
Earlier approval cannot authorize a changed subject while awaiting revocation.
Unchanged scope retains its applicable baseline approval. A non-substantive edit
may carry approval forward when the record explains why the reviewed meaning
and dependencies remain unchanged.

A bounded verdict adds approval coverage for its identified subject; preserve
baseline scope and evidence rather than replacing them with a delta-only row.
Record which current scope the new verdict covers and which earlier evidence
still applies. The approval table carries current coverage; preserve superseded
verdicts in a separately labeled history block or linked record, not as current
approval rows. Earlier verdicts remain historical evidence; explicit revocations
retain their reason and affected scope. The current required scope must have
applicable approval before its gate is met. Historical pending or blocked rows
for superseded subjects do not veto a subsequently approved current subject.

When reconciling sign-off, inspect its history before declaring approval absent
or applicable. Missing subject evidence, unclear scope or conflicting applicable
verdicts returns as an approval gap for clarification, not a favorable row chosen
by the recorder. Distinguish specification review, human consent and executable
coverage; none substitutes for another.

## Records beside a specification

Three kinds of record sit beside a specification, and the distinction is what they
are about:

| Record     | About                                            |
| ---------- | ------------------------------------------------ |
| decision   | the product — what will and will not be built     |
| ADR        | the code's structure — a durable technical rule    |
| lesson     | an incident and what it changed                    |

Location is chosen by **blast radius**, not by who made the call: a record binding
one Feature sits beside that Feature's specification; a record binding the whole
repository sits at the repository root, numbered repo-wide.

An accepted ADR is immutable. Only its status changes; a new ADR supersedes it
entirely. A decision that is later reversed keeps its original reasoning and gains
an appended reversal — including **what was considered and not chosen**, so a
later reader cannot rediscover the rejected option as a gap to fill.

## Identifiers are claimed after rebasing

Allocate against current trunk and shared reservations, not your branch alone.
Apply [`sdd-conventions`](../sdd-conventions/SKILL.md) § "ID schemes" to any
collision: only unpublished new IDs can change, with all local references updated.
Check publication evidence before choosing that path; a collision involving
permanent IDs is not resolved by calling one branch newer.

## Never add

- Template guidance, or a `{{placeholder}}`, left in a copy.
- A placeholder row in a coverage or traceability table.
- A reused or renumbered permanent ID, or an unpublished ID changed without all local references.
- A second copy of a derived answer, or a section mirroring another.
- A signature restated in prose after the code exists.
- A green coverage signal for a test that does not exist.
- A hand-kept index of specifications, decisions or lessons.
- An item that a specification names as its owner.
- A `Rule:` inserted mid-file in a `.feature`. It adopts every scenario after it;
  append it instead.
