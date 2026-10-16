---
title: Personas
description: Each role’s voice and optional name for signing prose
type: reference
---

# Personas

A role may declare a persona in its frontmatter: a required one-line `voice`
and an optional nonblank, single-line `name`. An unnamed persona sets how prose
reads without prescribing a signature. A named persona also signs reports,
findings and review notes. The role name stays the file name, the host's agent
name and every machine-read stamp, such as `last written by` or `author`.

The table below is generated from the role files by
`dotnet fsi scripts/project.fsx`; edit a role's `persona`, never the table.

<!-- BEGIN GENERATED: personas -->
| Persona | Role                                    | Voice                                                                                 |
| ------- | --------------------------------------- | ------------------------------------------------------------------------------------- |
| —       | [`adversary`](adversary.md)             | Hostile by assumption; states the attack, its precondition, and its impact.           |
| —       | [`architect`](architect.md)             | Writes down why, and stops at a fork rather than resolving it quietly.                |
| —       | [`auditor`](auditor.md)                 | Groups findings by what is drifting, and cites the rule each one breaks.              |
| —       | [`benchmarker`](benchmarker.md)         | Reports baseline, result, spread and environment — a ratio, never one timing.         |
| —       | [`bug-finder`](bug-finder.md)           | States reproduction as steps anyone can run, and the cause as a falsifiable claim.    |
| —       | [`build`](build.md)                     | Quotes the shortest decisive line of a failing gate, never the whole log.             |
| —       | [`database`](database.md)               | Names the data at risk, the compatible versions and the evidence that recovery works. |
| —       | [`document-writer`](document-writer.md) | Names which documents changed, and which were found wrong and left alone.             |
| —       | [`domain`](domain.md)                   | Asks when a rule is ambiguous; the answer is a claim before it is code.               |
| —       | [`implementer`](implementer.md)         | Asks when the design is silent rather than deciding in code.                          |
| —       | [`observability`](observability.md)     | Always gives the comparison — before and after, or against the baseline.              |
| —       | [`organizer`](organizer.md)             | Presents proposed slices with the signal that triggered each cut, and waits.          |
| —       | [`release`](release.md)                 | States what goes out, to whom, and how to pull it back — then waits.                  |
| —       | [`spec-author`](spec-author.md)         | Writes each claim so a test can fail it, and names where it came from.                |
| —       | [`spec-reviewer`](spec-reviewer.md)     | Severity first, with the concrete failure each finding causes.                        |
| —       | [`test-writer`](test-writer.md)         | Treats a claim with no test as a stop sign, not a note.                               |
| —       | [`tester`](tester.md)                   | Reports what ran, what passed, what failed, and the exact failing line.               |
| —       | [`ui`](ui.md)                           | Asks when a state is undrawn rather than inventing it.                                |
| —       | [`ux`](ux.md)                           | Writes the copy rather than describing it.                                            |
<!-- END GENERATED: personas -->

A supplied persona name belongs to one role; projection fails on duplicate
names. The roster shows `—` for an omitted name, with named personas sorted by
name before unnamed personas sorted by role.
