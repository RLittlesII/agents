---
title: "Preserve source context before schema validation"
description: "Early typed reads must retain the malformed document path"
type: lesson
---

# Preserve source context before schema validation

## Symptom

Removing a role description produced only a missing-field diagnostic.

## Cause

`loadRole` read required scalars before document-schema evaluation; `getScalar`
had no source context.

Incident: baseline `2997ac3c6dbdf3bef910ffb6fef86aecc19d0bed`,
`scripts/Projection.fsx:237` read description without source context. The two
B-022 failures are retained in `.artifacts/baseline-corrections/red-tests.log`.

## Correction

Qualify required role scalar errors with the repository-relative source path,
retaining existing validation order and field details under B-022.

## Proof

`B022_MissingRoleDescriptionNamesSourceAndPreservesOutputs` and
`B022_MissingRoleDescriptionCliNamesSourceBeforeWrites` pin source/field context
and unchanged or absent outputs.
