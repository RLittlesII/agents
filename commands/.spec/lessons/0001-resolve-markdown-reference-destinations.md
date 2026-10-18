---
title: "Resolve Markdown reference destinations"
description: "An inline-link expression does not validate reference-link destinations"
type: lesson
---

# Resolve Markdown reference destinations

## Symptom

A Markdown reference link into source contracts passed B-005 validation and
projected a broken relative destination into the other host.

## Cause

The guard recognized inline link syntax only, not resolved reference links.

Incident: baseline `2997ac3c6dbdf3bef910ffb6fef86aecc19d0bed`,
`scripts/Projection.fsx:718` and `:738` used only the inline-link guard. The four
reference-link failures are retained in
`.artifacts/baseline-corrections/red-tests.log`.

## Correction

Retain existing inline rejection and inspect resolved link destinations with the
existing Markdown parser. Full, collapsed and shortcut references share the
source-path rule; fenced reference examples remain text.

## Proof

Four registered B-005 reference-link regressions cover both source namespaces
and the three reference forms through planning and the public CLI. The fenced
reference control preserves valid command text.
