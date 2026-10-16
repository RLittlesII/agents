---
title: "Decision 0001: Lead and specialist collaboration"
description: "Replace peer handoff complexity with accountable state leads, conditional specialties and retained capabilities."
type: decision
author: spec-author
---

# Decision 0001: Lead and specialist collaboration

## Context

The user identified `agents/README.md` Handoffs as too complicated and requested
a specification for lead-and-specialist grouping, redundant-role consolidation
and a database specialist on 2026-10-16. — Ruth

## Decision

Adopt the state leads, specialist triggers, retirement mappings and six
situation routes in [0001-F2 § 2](../README.md#2-user-needs), claimed by
B-010–B-036; these are responsibility groupings and artifact dependencies,
without an executable workflow engine or a separate agent per responsibility.

The lead integrates the result while existing section owners, independent
review, accepted diagnosis, failing-test and release-confirmation requirements
remain authoritative. Commands convene applicable roles within
[0001-F3 C-1 and C-2](../../../commands/.spec/README.md#4-constraints);
they acquire no state-transition tables.

Retire six standalone roles without discarding their duties; add the
host-free `database` specialist without assigning it specification ownership.
This reverses [0001-F2 § 5 row 5](../README.md#5-out-of-scope)'s database-role
exclusion: storage follows domain invariants and architect-owned design,
while persistence implementation, investigation and recovery receive explicit
expertise. Engine and framework knowledge stays with the companion.

Performance/cost benchmarking and operational observability keep distinct
triggers; production instrumentation belongs to the responsible implementation
assignment. Active contracts, commands and host references migrate together,
with host outputs regenerated and historical identities preserved. — Ruth

## Alternatives and costs

| Alternative | Disposition and cost |
| --- | --- |
| Keep the peer-to-peer graph and all current roles | Rejected: the reader must reconstruct accountability from many edges, and overlapping capabilities remain separate workflow identities. |
| Keep database work scattered across general contracts | Rejected: lifecycle persistence expertise remains implicit and migration, compatibility and recovery questions lack a named specialist. |
| Put every specialty into one general implementation role | Rejected: persistence, business rules, presentation and evidence lose bounded expertise and review independence. |
| Require a separate agent or mandatory stop for every specialty | Rejected: invocation count grows regardless of affected claims, preserving the complexity the user asked to remove. |
| Replace command prompts with an executable state machine | Rejected: no runtime orchestration is requested and command transitions conflict with 0001-F3 C-1. |

The chosen shape costs coordinated edits to retained contracts, the roles index,
commands, host overlays and projected outputs; the agreement and scenarios are
authored first, and no such implementation is delivered by this amendment.
No new frontmatter key is required to describe a retained role with several
review units or modes. — Ruth

## Baseline reconciliation — 2026-10-16

Main commit `5924065` landed while this branch was being prepared: it already
folds correction into implementation, design review into specification review,
and log analysis into investigation, and also folds operational measurement
into benchmarking. Its twenty-role roster retains discovery, pairing and
obstruction roles.

The user's later agreement in this record remains the target: preserve the
three completed duty transfers, fold the remaining discovery, pairing and
obstruction identities into the agreed roles or modes, restore operational
observability as `metric-nerd` separate from `benchmarker`, and add `database`.
The result remains the nineteen roles in § 2; this changes the migration
baseline, not the agreed capability set.

This agreement supersedes the earlier choice to retain the pairing/obstruction
roles and combine operational measurement with benchmarking. Preserve that
commit and its decision evidence as history. Restore the operational contract
from source history and adjust its boundaries; regenerate host files rather
than restoring generated copies. — Ruth
