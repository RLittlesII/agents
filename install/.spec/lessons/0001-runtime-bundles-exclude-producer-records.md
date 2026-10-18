---
title: "Lesson 0001: Runtime bundles exclude producer records"
description: "Copying maintainer source trees installed broken producer references into private runtime contracts."
type: lesson
kind: product
---

# Lesson 0001: Runtime bundles exclude producer records

**Date:** 2026-10-17
**Kind:** product

## Symptom

The 2026-10-17 checkout's Codex plan contained 23 private specification files
and 12 unresolved relative file references. The producer role index linked
`.spec/README.md` and `../README.md`, which the private plan did not supply.
Four new executable tests failed against that selection.

## Root cause

Commit `3fd59aa` selected every flat `agents/*.md` document and called
`read_tree(source, method)` without a metadata exclusion. That copied the
producer index and nested maintainer specifications. Projection already omits
`.spec` directories, so native skills and private source skills had different
boundaries. Top-level hidden-directory filtering did not prune nested metadata.

The bounded fix in [`build_plan`](../../install.py) derives a consumer index
from the selected native-role inventory and excludes `.spec` during private
method traversal. Runtime bytes and source files remain unchanged; unselected
producer notes do not become contracts.

## Spec delta

User decision 0002 adds 0038-F1 B-007 to state the runtime packaging boundary;
B-006 retains installed dependency closure. Full producer closure was not chosen.
Old installed files require explicit reconciliation under the existing
non-pruning policy; a new plan does not delete them.

## Claim

0038-F1 B-007: `test_B007_consumer_index_uses_selected_runtime_inventory_with_both_managers`,
`test_B007_private_specs_are_excluded_at_every_depth_and_runtime_bytes_survive`,
`test_B007_native_role_cannot_overwrite_the_consumer_index` and
`test_B007_checkout_runtime_bundle_is_closed_over_private_file_links`.
