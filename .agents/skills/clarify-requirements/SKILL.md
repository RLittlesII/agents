---
name: clarify-requirements
description: Resolve genuinely material requirement ambiguity in a specification-driven repository. Use when two plausible readings would change user-visible behaviour, the data shape, a public surface, or a provider's terms.
---

# Clarify requirements

**Project rules.** This skill is portable. The method's shared facts — id
schemes, section ownership, spec layout — are in
[`sdd-conventions`](../sdd-conventions/SKILL.md). The consuming repository's
companion skill, `<project>-conventions` § "Questions", names where this rule
lands there. Read all three.

## First, look

The specification, the scenarios, the sibling Features' scope sections, the
decision records, the lessons, the code, and the tests. Most questions are already
answered there, and a question the project knows it has not decided is recorded as
an open question rather than re-asked.

## Decide or ask

For project technology choices, use
[`technology-decisions`](../technology-decisions/SKILL.md) for evaluation and
human authorization; this skill governs the questions and requirement answers.

- **Reversible, with no behavioural change?** Decide it, and document the
  assumption locally. Folder names, class structure, the order of two independent
  operations — those are yours.
- **Ask one concise question** when the answer cannot be found and the choice
  materially changes user-visible behaviour, the data shape, a public surface,
  credential handling, a provider's terms, or a guarantee the design makes.
  Include: the evidence you already have, two concrete readings, what each costs,
  and the smallest decision that unblocks you.
- **Ask what rule the answer follows**, not only which option wins. An undocumented
  ruling cannot be checked later, and a standing convention will eventually
  contradict it once code is built on both.
- **Never resolve material ambiguity by assumption** — not under a spike framing,
  not under a prototype framing, not because the session is nearly over.

Weigh how hard the action is to undo against how ambiguous it is:

|                  | Reversible            | Hard to reverse       | Irreversible |
| ---------------- | --------------------- | --------------------- | ------------ |
| Low ambiguity    | proceed, state it     | proceed, state it     | ask          |
| Medium ambiguity | proceed, state it     | ask                   | ask          |
| High ambiguity   | ask                   | ask                   | ask          |

An irreversible action always stops for an answer. A reversible one with little
ambiguity never does: asking there is indecision, and it teaches the person to
stop trusting the judgement. A standing instruction or an earlier authorization
already answers what it covers; within its scope, do not ask again.

When several questions are open, ask them together: at most three per pass, one
theme each, as concrete options grounded in what you found. When an answer
comes back, say whether it fully resolves the question before acting on it.

## When nobody can answer

A halt is a return, not a wait. Hand back each open question word for word,
with the work finished so far. Never pick a default, and never produce output
that assumes the answer.

## An exploratory comment is not a decision

"What if we…", "another thought…", a reference to how a previous project did it —
these are substantive enough to change direction and carry none of the structure
of a decision: no explicit choice and no rejected alternative. **Escalate
before allocating work against one.** "This sounds like it could be a direction
change — is it exploratory, or a decision to record?" is a cheap question that
prevents a day of wasted design.

A choice that moves a load-bearing boundary is asked, always, whatever the
table above says.

## A pullback scopes only what was withdrawn

The opposite error costs as much and is harder to catch. When someone pulls back
from a direction, they withdrew **what they named**, not everything adjacent to
it — and "we deferred that" reads as caution rather than as a claim needing
evidence, so nobody checks it.

**Before recording that something is undesigned, open the specification and read
the section.** When a design genuinely is superseded, the honest move is an open
question **on the specification that owns it** — never a contradicting note in a
third document that points at neither.

The incident behind both halves: a "what if we…" about a load-bearing boundary
was relayed as a decision and design started toward it; the pullback that
followed was then read as withdrawing more than it named.

## Write the answer back

In the same change, so the next session starts from the answer:

- The answer becomes a claim, a scenario, a specification edit, or an
  out-of-scope row. **The specification is where it lands first.**
- A decision record captures what was chosen **and what was rejected, with its
  cost** — the second half is what stops the rejected option being rediscovered as
  a gap.
- A design change updates the section it affects and the item's acceptance
  criteria.

An answer that exists only in conversation was never recorded.

## Never add

- An assumption standing in for a material decision.
- An irreversible action taken on an assumption.
- A question asked that a standing instruction already answers.
- A question asked that the specification already answers.
- A decision recorded without its rejected alternative.
- An exploratory comment treated as ratified.
- A note in a third document contradicting a specification it does not cite.
- A ruling that names the winning option but not the rule it follows.
- A default picked, or output produced, while a blocking question is unanswered.
