---
title: "Output inventory is not source selection"
description: "Source filtering must not hide stale generated metadata or bypass output boundaries"
type: lesson
---

# Output inventory is not source selection

## Symptom

B-008 checking omitted stale `.spec` files inside generated output roots.

## Cause

The output walker reused a metadata exclusion appropriate to source selection.
Extending that walk also exposed linked-directory traversal before deletion.

Incident: baseline `2997ac3c6dbdf3bef910ffb6fef86aecc19d0bed`,
`scripts/Projection.fsx:178` excluded `.spec` during output inventory. The
`B008_StaleSpecificationOutputsAreReportedAndRemoved` failure is retained in
`.artifacts/baseline-corrections/red-tests.log`.

The same baseline’s `scripts/Projection.fsx:85` and `:822` rewrote Unix literal
backslashes. Surface review of subject `a148bf47a5a726c87dbf4908547bcc27c6cfe56c70dfb7b7b95a2ca33ab629ed`
found that normalization also invalidated linked-path checks. All four native-path
controls failed before correction in
`.artifacts/baseline-corrections/native-red-tests.log`.

## Correction

Keep source pruning in `runtimeFilesUnder`; inventory all ordinary output files.
Reject escaping or linked output paths before comparison/publication and recheck
before each effect. Split only platform separators and preserve native characters
through relative paths, output roots, traversal and effects. Concurrent path
replacement is outside that guarantee.

## Proof

B-008 regressions exercise library and CLI stale detection/removal, unchanged
source/out-of-root metadata, linked metadata/root/ancestor/file rejection and an
escaping output root. Four Unix controls cover linked paths and native stale
cleanup while preserving slash-named sentinels. Targets and earlier planned
outputs remain unchanged.
