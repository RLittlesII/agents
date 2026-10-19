---
title: "ADR-0011: Command skills follow artifact ownership"
description: "Command skills omit agent bindings while native command files retain host-specific convening"
type: adr
---

# ADR-0011: Command skills follow artifact ownership

## Status

accepted

## Context

PR #9 review 5560928256 identifies agent references in generated command
skills. ADR-0005 projects commands as Codex skills but embeds host agent paths
in their preamble, contradicting the one-way skill boundary. The same source
body already describes the order and artifact responsibilities.

## Considered options

1. Keep named agents and replace host paths with portable contract references.
2. Omit agent bindings from skill outputs and follow repository artifact ownership.
3. Restore repository-local Codex prompts, rejected by ADR-0005.

## Decision

Option 2, as requested by the review's skill boundary. This ADR supersedes
ADR-0005 entirely and carries forward its unchanged projection decisions:

- Write commands once under `commands/` with role and skill dependencies in
  frontmatter; project native Claude commands and Codex skills.
- `hosts/<host>.yaml` declares `commands_output`. An output ending in
  `/SKILL.md` has `name` and `description`; native command files have
  `description` and `argument-hint`.
- `commands_arguments` supplies invocation text on hosts without `$ARGUMENTS`.
- Preserve `.claude/commands/<name>.md` and `.agents/skills/<name>/SKILL.md`;
  command names cannot collide with method skill names.
- Native command files retain their host agent and skill dependency paths.
  Skill outputs list skill dependencies and repository ownership/review guidance,
  without instructional agent bindings or agent contract paths. Literal fenced
  examples remain text under F3 B-005 and supply no routing authority. Shared bodies use artifact owners
  and review passes; projection preserves them apart from invocation substitution.

Method skills remain unchanged. Agent contracts continue to be installed and
available through host registration and repository guidance.

## Consequences

Skills no longer assume a host's agent installation paths or roster. Shared
command bodies use artifact owners and review passes. Repository guidance must
supply ownership; a skill does not choose a named agent. Native commands keep
explicit convening. Portable agent paths would still make skills name agents,
so option 1 does not satisfy the review. No host capability or package changes.
