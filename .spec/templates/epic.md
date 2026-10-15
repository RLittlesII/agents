---
title: "Epic {{id}}: {{name}}"
description: "{{one sentence: the result the Features produce together}}"
id: "{{id}}"
type: epic
status: needs-decomposition
priority: med
milestone: null
children: []
created: "{{date}}"
updated: "{{date}}"
github_issue: null
---

<!--
The blank for an epic record: epics/<id>-<slug>/epic.md. The epic's owner
copies it and deletes this comment. An epic states an outcome and holds no
claims, acceptance criteria or scenarios; see spec-and-traceability
§ "An epic states an outcome". specht validates the frontmatter but ships no
epic blank, so it lives here and not in .specht/templates/ (ADR-0004).
`id` shares the repository-wide number space with work items.
-->

# Epic {{id}}: {{name}}

## Summary

<!-- What exists when this epic is delivered, in a paragraph. No claim list. -->

## Business Value

<!-- Who gains what, and what happens without it. -->

## Measure

<!--
Written before the first Feature. How the result is observed, and the value
that means it arrived. An outcome nobody can measure is an ambition.
-->

The epic is delivered when all of these hold:

1.

## Features

<!-- One row per Feature, added once the person confirms it belongs here. -->

| Feature | Name | Specification |
| ------- | ---- | ------------- |

## Out of this epic

<!-- Each exclusion names where that work goes instead, when it goes anywhere. -->

-
