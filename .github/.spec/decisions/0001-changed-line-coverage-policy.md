---
title: "Decision 0001: Changed-line coverage policy"
description: "Adopt an exact pooled 80% floor with fresh source-bound evidence and one local/CI gate."
type: decision
---

# Decision 0001: Changed-line coverage policy

## Context

The user requested repository-wide coverage policy and enforcement while
finishing 0001-F4, and confirmed this Feature under epic 0001. ADR-0007
requires the repository to name its own threshold and gate.

## Decision

Use an 80% floor over distinct changed executable production lines pooled
across measured runtimes, including the gate's production code. Compare exact
integer counts; do not average file/runtime percentages or round before deciding.
Resolve the requested Git base and use its merge base with HEAD for comparison,
including current tracked edits and non-ignored untracked sources.

Collect fresh evidence on every invocation and bind it to source bytes,
inventory, policy and comparison identities. Missing, malformed or inconsistent
evidence fails before an empty changed-line intersection can become N/A.
Production FSI/Python entrypoints remain unchanged; compiled F# assemblies exist
only for instrumentation of linked original sources.

## Alternatives

- A fixed percentage in the portable method: rejected by ADR-0007; this number
  belongs to this repository.
- Whole-repository coverage as the gate: rejected because this request is a
  changed-line floor; historical misses are reported rather than hidden.
- Per-runtime or per-file floors: rejected in favor of one weighted changed-line
  decision; they introduce additional independent pass conditions.
- Rounded decisions: rejected because 79.9% would appear to meet 80%.
- Skip collection for documentation-only changes: rejected because missing
  executable-line evidence cannot establish that no applicable lines changed.

## Consequences

The policy and gate are shared locally and in CI. New production sources
without a collector mapping fail explicitly. Test, generated and metadata
lines cannot inflate the production denominator. This decision supplies the
repository policy required by ADR-0007; it does not amend that accepted ADR.
