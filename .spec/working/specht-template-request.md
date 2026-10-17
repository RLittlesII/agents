---
title: Draft request for consumer-neutral specht templates
description: Unsent upstream request grounded in verified template conflicts and versioning constraints
type: working-note
---

# Draft request for consumer-neutral specht templates

**Status:** local request draft, not sent. The user authorized developing the
upstream agreement in the sibling specht checkout after the reviewed local
changes are committed. No upstream files,
published versions, tool pins or consumer-owned templates have changed.

## Problem and evidence

This repository consumes `specht.tool` 0.1.90 with an implicit schema pin of
0.1.0. Its tool-owned feature template prescribes workflow details that differ
from the consuming repository's agreed method. The same assumptions remain in
the sibling checkout's embedded `src/tool/templates/v0.1.0/feature.md` and
`src/tool/templates/v0.2.0/feature.md`.

| Location | Conflict | Requested outcome |
| --- | --- | --- |
| Sections 6–7 | Implementation owns design in the template; this consumer assigns design separately. | Obtain section owners from consumer guidance or explicit placeholders. |
| Section 12 | The independent reviewer is also prescribed as the recorder. | Distinguish recording responsibility from the review verdict. |
| Lifecycle and Tasks | GitHub issues and labels are prescribed; this consumer uses local records. | Route tracker selection, source identity and synchronization to consumer guidance. |
| Priority guidance | Priority is described as authored and derived. | State one consistent consumer-selected scoring policy. |
| Section 8 | Reqnroll is prescribed; this consumer uses F# and Python tests. | Route test-stack selection to consumer guidance. |
| Section 12 | The template does not distinguish current applicable approvals from history. | Explain current-scope coverage and compatible preservation of historical verdicts. |

[Item 0007](../../.issue/0007-template-stamp-owner.yml) already tracks the wrong
design-owner stamps. Its local mitigation rejects incorrect stamps at projection
time; it does not repair the producer template. Consumer ADR-0004 keeps the
installed template tool-owned, so the upstream request remains the chosen route.

## Proposed upstream agreement scope

Make newly authored specifications obtain project policy from their consumer's
guidance. Neutral placeholders are the smaller candidate; configurable ownership
would add a public configuration contract and needs its own explicit agreement.
Do not impose this repository's particular role roster, tracker, scoring rule or
testing stack on every consumer.

Retain schema fields and checker behavior unless an upstream claim explicitly
authorizes changing them. A template correction must follow specht's published
version and upgrade policies; its AGENTS.md forbids changing a published version
in place. The upstream owner must settle the version and supported pins before
implementation. Existing unrelated edits in the sibling checkout remain outside
this work.

## Proposed regression evidence

- Author a fixture specification using a different section-owner mapping and
  show no contradictory owner is supplied by template guidance.
- Use a local tracker fixture and show no GitHub label or issue is required by
  the template's instructions.
- Use a non-Reqnroll testing fixture and show the template routes to its declared
  stack without introducing another framework.
- Check priority guidance for a single policy and explicit consumer ownership.
- Keep historical verdicts outside the current approval table and verify current
  applicability without destroying earlier evidence.
- Verify immutable published snapshots and the agreed new-version pin/upgrade
  behavior; a template-only correction changes no checker verdict by accident.

These are proposed upstream outcomes and tests, not new approved upstream claims.
They require the owning specification's agreement and design before dependent
tests or implementation. GitHub issues are disabled for `RLittlesII/specht`; no
external message has been sent.
