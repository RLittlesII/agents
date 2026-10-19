---
title: "Decision 0001: Projection metadata boundary"
description: "Exclude tracker entries consistently from runtime skill surfaces"
type: decision
---

# Decision 0001: Projection metadata boundary

**Date:** 2026-10-17
**Status:** accepted

The user chose "Exclude tracker metadata consistently" instead of a root-tracker
placement exception. Exact `.issue` entries are producer tracking metadata at
any depth, excluded from skill discovery, copied resources, role-name lint and
native/private installation. Other runtime names and bytes remain in scope.
0001-F1 B-025 records this part of the boundary; sibling projection, lint and
installation agreements own their respective surfaces.

A Skill-boundary-only placement exception was rejected because it would preserve
the discovery conflict for future tracker entries and leave nested records in
runtime bundles. Broad exclusion of hidden files was rejected because hidden
runtime support files are legitimate resources. Existing installation non-pruning
remains; this decision does not authorize deleting installed consumer files.
