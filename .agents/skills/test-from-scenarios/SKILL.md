---
name: test-from-scenarios
description: Turn a specification claim into a test that fails for the right reason — synthetic fixtures, an injected clock, no network, and an assertion that can actually fail. Use when writing or reviewing tests.
---

# Tests from scenarios

**Project rules.** This skill is portable. The method's shared facts — id
schemes, section ownership, spec layout — are in
[`sdd-conventions`](../sdd-conventions/SKILL.md). The consuming repository's
companion skill, `<project>-conventions` § "Testing", names where this rule
lands there. Read all three.

## A claim comes first

Encode only what the specification states. A test that asserts an inference is a
test that will be "fixed" by someone who reads the claim instead. If the claim is
incomplete, amend the claim — that is an escalation, not a detour.

A scenario documents; a scenario alone is not coverage. Coverage is a test that
runs and can fail.

## Write from the scenario, not the conversation

The conversation that produced the claim is gone by the next session. Whatever it
established that matters is in the claim, the constraint, or an open question —
and if it is in none of them, it is not yet agreed.

## Assert behaviour, not structure

Assert what the thing does, not how it is arranged. A test that names a private
field, a call order that nothing depends on, or a type's shape fails on every
harmless refactor and teaches the team to weaken tests.

## A test's name is a citation

§ 9 cites a test by name. Renaming it to suit an implementation breaks that
citation silently: the row still reads as covered and points at nothing. Keep the
name, or reword the claim and the row together.

## Explicit state and execution

Build fixtures and fakes from the inputs the scenario states, then call the same
entry point a caller uses. A helper may stage input; it must not hide stage
ordering, barrier handling or a setup branch the claim depends on. Prefer
data-driven cases to loops or conditionals in a test body: a branch in a test is
a second test that never reports on its own.

## Both tiers, deliberately

An acceptance scenario proves the claim end to end. It does **not** relieve the
concern beneath it of unit coverage: a decision, a derivation, a boundary check,
an ordering guarantee, a mapping — each earns its own test at the level it lives
at. A claim covered only by a green scenario has its mechanism untested.

## Time is injected, always

A clock is a dependency. Advance it deliberately. Never read ambient time in a
test, and never sleep — a delay is an admission that something unobservable is
being waited on, and it will be flaky on a loaded machine.

## No network, ever

No live provider, no real credential, no DNS. Use the transport library's own test
double and assert on the request that was built, not only on the response that
came back.

## Fixtures are synthetic

Invented data, committed beside the test, preserving the exact wire format that
matters — header casing, timestamp format, byte-for-byte body. Never real personal
data and never a captured payload from a real sender, scrubbed or not.

## What is worth testing

Where bugs actually live: differs and derivations, converters and mappers,
anything time-derived, boundaries and seams, redaction, deduplication, and the
translation between an external format and the domain.

## Never add

- A test that reaches the network or needs a credential.
- An ambient clock read, a `Sleep`, or a delay.
- An assertion weakened to make a run green.
- An assertion on structure where behaviour was claimed.
- A test renamed while a § 9 row still cites the old name.
- A helper that hides the ordering or setup branch a claim is about.
- Recorded real traffic as a fixture.
- A test that exists only to raise a coverage number.
- A green coverage signal for a test that does not exist.
