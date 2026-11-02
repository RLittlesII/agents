---
name: technology-decisions
description: Decide a project's stack, packages, architecture and UI technology with the human before dependent design or implementation. Use for initial technology selection, disputed library exclusions, or changes to agreed technology; expose assumptions and unknowns and record the human's choices.
---

# Technology decisions

**Project rules.** Read
[`sdd-conventions`](../sdd-conventions/SKILL.md) and the consuming repository's
companion skill, `<project>-conventions`, for its existing stack, dependency
policy and decision records. The owner of § 7 leads this conversation with the
human requesting the work. A technical recommendation is not the human's choice.

## Start from what the human chose

Read the request, standing instructions, accepted ADRs, specification, companion
and existing dependencies before proposing technology. Carry forward explicit
choices within their stated scope; do not ask for approval again when the human
has already authorized that choice. Distinguish a requirement ("use this
library"), a preference ("I would like this library") and an exploratory example.
If the distinction changes the decision and is unclear, ask.

Keep a requested library in the evaluation. If it appears incompatible, show
the actual requirement, evidence and available ways to meet it, then ask the
human to resolve the conflict. Do not quietly omit, substitute or rule it out.
Do not claim an infeasible choice works; leave dependent work blocked until the
human chooses a supported path or changes the requirement.

## Build the decision picture

For each affected area, record what is already decided, what is proposed and
what remains open. Mark an area not applicable with a reason when appropriate;
an existing project needs only its affected choices revisited.

| Area | Establish |
| ---- | --------- |
| Stack | Language, runtime, framework, supported platforms and deployment constraints |
| Packages | Requested and incumbent libraries, their responsibilities, relevant versions and integration constraints |
| Architecture | Boundaries, dependency direction, state and persistence, communication and justified patterns |
| UI | UI framework, component system and styling technology, tied to the agreed interaction |
| Assumptions | Each unverified premise, its source, which choice depends on it and how it will be checked |
| Unknowns | Each unanswered question, its impact, who can answer it and the evidence or experiment needed |

Requirements come from the human or cited project artifacts. Label an inferred
need as a question or assumption until confirmed; do not turn it into an
exclusion criterion. Separate a project's need from a candidate's capability:
"the project requires X" and "this library cannot do X" each need their own
evidence.

Compare viable options against those drivers, including the incumbent and the
human's requested option. Use the smallest comparison that explains the choice:
fit, concrete tradeoffs, adoption or migration cost, and the recommendation.
Avoid an exhaustive package survey when the decision is already bounded.

## Verify reasons before rejecting an option

Cite primary documentation or source for capability and compatibility claims,
with the relevant version and limitation. Check current sources when the choice
depends on current support, licensing or maintenance. Repository code or a
bounded experiment can establish how a mechanism behaves; distinguish observed
results from estimates and incomplete investigation.

Missing evidence means unknown, not unsupported. A failed experiment establishes
only what that setup tested. When the human supplies a working example, examine
its mechanism and applicability, correct any contradicted rationale, and bring
the revised comparison back to the human. Do not invent a new requirement to
preserve the original recommendation.

Use a disposable spike for a material feasibility question when needed. State
the question, scope and completion evidence before experimenting; spike code
does not become production scaffolding.

## Decide together before dependent work

Present the unresolved choices, recommendation, evidence, tradeoffs and material
assumptions to the human using
[`clarify-requirements`](../clarify-requirements/SKILL.md); it owns question
format and writing requirement answers back. Make the requested decision clear
instead of burying a package rejection in design prose.

Record the choice and rationale in § 7, linking an ADR for a durable technical
rule. For project setup before a Feature exists, use the companion's project
design record and repo-wide ADRs; do not invent a Feature to host the decision.
Follow
[`spec-and-traceability`](../spec-and-traceability/SKILL.md) for record placement,
ownership and supersession. Include:

- Scope and selected technologies, with the human's confirmation or the exact
  earlier instruction that authorizes them, and its date.
- Decision drivers and their sources; alternatives rejected and why, with the
  evidence supporting each rejection.
- Assumptions and unknowns, identifying which block dependent work and how each
  will be resolved. Nonblocking uncertainty has an agreed bound and follow-up.

Give the human a concrete decision summary before asking for confirmation. An
approval covers its stated choices and scope; approval of a goal does not
approve an unseen technology substitution. Silence, an agent recommendation
and an independent design review do not establish human consent. An explicit
delegation to choose technology is authorization within its stated bounds;
record it and the resulting choices without reopening each one.

The design is not Ready while an affected technology choice lacks authorization
or a blocking unknown remains. Record the missing decision in § 11, or the
project design record before a Feature exists, and stop dependent design,
interface stubs, tests and implementation. Continue evidence
gathering and work independent of that choice. Existing authorized choices need
no new ceremony.

Before implementation, check the design and dependency declarations against the
recorded choices. A new package, substitution, removal or architecture change
outside the authorized scope returns to this conversation before code depends
on it. Evidence that invalidates a decision reopens the affected choice; it
does not authorize an automatic replacement.

## Never add

- A fabricated requirement used to eliminate a technology.
- A requested library excluded without evidence and human resolution.
- An unsupported capability claim presented as a verified limitation.
- A working counterexample dismissed without examining it.
- An assumption or unknown silently promoted to a requirement or a decision.
- A recommendation, silence or review verdict recorded as human approval.
- Dependent code written before its technology choices are authorized.
- A replacement hidden in implementation or an accepted ADR edited in place.
