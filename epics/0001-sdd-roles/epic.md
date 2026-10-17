---
title: "Epic 0001: Specification-driven development roles"
description: "Host-free roles, method skills and commands, projected into the files each AI host reads, with the rules between them checked."
id: "0001"
type: epic
status: in-progress
priority: med
milestone: null
children: ["0001-F1", "0001-F2", "0001-F3", "0001-F4", "0001-F5", "0001-F6"]
created: "2026-10-14"
updated: "2026-10-16"
github_issue: null
---

# Epic 0001: Specification-driven development roles

## Summary

The roles that work a specification — from the first ask to the last review —
written once as host-free contracts, with the method skills they share and the
commands that convene them, projected into the native files Claude Code and
Codex read. The rules between those pieces are checks, not conventions: every
section has one owner, every unit of review has one reviewer, a role names only
skills that exist, a command names only roles that exist, and a skill names no
role.

## Business Value

A consuming repository adopts the method by pulling committed files at a tag.
It gets one roster, one set of rules and one way to convene the roles on
whichever host its people use, and it adds only a companion skill naming its
own paths and stack. Without this epic each repository writes its own roles per
host, and the copies drift from each other and from the method.

## Measure

The epic is delivered when all four hold:

1. Every role, skill and command projects to Claude Code and Codex, and
   `dotnet fsi scripts/project.fsx --check` passes in CI.
2. A consuming repository pulls its roles, skills and commands from this
   repository at a tag, and keeps no hand-edited copy of any of them.
3. Each Feature below is approved with no `Missing` row in its § 9.
4. Repository verification enforces its changed-line coverage policy locally and in CI.

## Features

| Feature | Name           | Specification                                                      |
| ------- | -------------- | ------------------------------------------------------------------ |
| F1      | Projection     | [`scripts/.spec/README.md`](../../scripts/.spec/README.md)         |
| F2      | Role contract  | [`agents/.spec/README.md`](../../agents/.spec/README.md)           |
| F3      | Commands       | [`commands/.spec/README.md`](../../commands/.spec/README.md)       |
| F4      | Skill boundary | [`skills/.spec/README.md`](../../skills/.spec/README.md)           |
| F5      | Next work      | [`skills/next/.spec/README.md`](../../skills/next/.spec/README.md) |
| F6      | Repository verification | [`.github/.spec/README.md`](../../.github/.spec/README.md) |

The user confirmed F6 under this epic on 2026-10-16. It verifies changes to the
shared method without adding product technology to portable roles or skills.

## Placement

Each Feature's specification sits beside the concern it describes, as the
manifest's `**/.spec/README.md` layout expects, and names this epic in its
`epic` key. This record lists them in `children`. The checker validates this
file's frontmatter but does not compare `children` with the Features that name
the epic; a new Feature is added to both by hand, and the reviewer of that
change checks the two agree.

## Out of this epic

- Technology-specific roles and skills: a language, framework, cloud, package
  or build tool belongs in the consuming repository's companion skill.
- A host other than Claude Code and Codex: each is a new overlay and renderer.
- Installing outputs into a consumer: a consumer pulls committed paths with its
  own tooling.
