---
title: Bounded handoff evaluation results
description: Nine synthetic routing probes in two fresh instances; no full workflow or host enforcement coverage
type: working-note
---

# Bounded handoff evaluation results

2026-10-17: owner authorized two fresh read-only agents for the
[predeclared probes](handoff-evaluation-plan.md). Runtime baseline: `2997ac3`.

| Probe | Authority | Result | Decisive response excerpt |
| --- | --- | --- | --- |
| H1 | F2 B-014 | Pass | “No work item is required.” |
| H2 | F2 B-012/B-013/B-020 | Pass | “Ready assessment alone does not open the gate.” |
| H3 | F2 B-042 | Pass | “No new claim or deliberately failing test is required.” |
| H4 | F2 B-044 | Pass | “The test writer must not add the automatic-deletion assertion.” |
| H5 | F2 B-045 | Pass | “Seven days of silence grants no takeover permission.” |
| H6 | F2 B-043; F5 B-017/B-040/B-042 | Pass | “The recommendation changes no artifacts.” |
| H7 | F2 B-046 | Pass | “it became permanent before merge.” |
| H8 | F2 B-047 | Pass | “Hold work dependent on that changed design.” |
| H9 | F2 B-012/B-013/B-017 | Pass after fixture clarification | “the parent bug remains.” |

## Observation and limits

Two fresh instances received source excerpts and synthetic fixtures without the
expected-outcome table. Neither used tools, wrote files, delegated, messaged or
called an external provider. The coordinator compared their proposed decisions
with the predeclared outcomes; no independent grading is claimed.

- `/root/handoff_probe_a`: H1–H4; all required routing decisions conformed.
- `/root/handoff_probe_b`: H5–H9; all required routing decisions conformed after
  the H9 fixture clarification. This second answer reused the same instance.

H9 initially called for a “successful original test invocation” while the parent
regression remained intentionally failing. Clarification supplied a successful
build and a discovered/executed test failing at its expected behavior assertion.
The helper correctly treated that as proof the parent work could resume and
returned only its configuration contribution and evidence. A passing regression
remained the parent implementation lead's responsibility. This corrected fixture
wording, not a production-contract finding.

Nine routing decisions conformed; no contract failure was observed in these
cases. These responses do not prove actual file-boundary enforcement, executed
handoff/resumption, full host instruction precedence, repeatability or platform
behavior. Source excerpts were supplied rather than whole projected host prompts;
multiple cases shared each fresh instance. H1 did not independently test grooming.

F2 means [0001-F2](../../agents/.spec/README.md); F5 means
[0001-F5](../../skills/next/.spec/README.md). Existing Partial/Missing traceability
statuses are unchanged. No Feature, audit item or installed-file follow-up is
closed by this evidence. The runtime source hashes remained unchanged throughout.

Machine-readable grading and source fingerprints are retained locally under
`.artifacts/handoff-evaluation/`; raw responses remain in the two named agent
transcripts of this chat. No new test framework, executable tests or runtime
contract edits were introduced.
