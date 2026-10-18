---
title: "ADR-0005: Commands project to Claude commands and Codex skills"
description: "A command is written once under commands/ and lands as a Claude command file and as a Codex skill"
type: adr
---

# ADR-0005: Commands project to Claude commands and Codex skills

## Status

superseded by ADR-0011

## Context

A command is a composable prompt: it convenes roles in an order, names the
skills they work with, and carries the request the person typed. It is not a
state machine; the roles' own contracts decide what each does. See
`commands/.spec/README.md`.

Claude Code reads repository commands from `.claude/commands/<name>.md` and runs
one as `/<name>`, replacing `$ARGUMENTS` with the text typed after it. Commands
and skills have merged in Claude Code, but the plain command file is still read
and is the form a person invokes by name.

Codex read custom prompts only from `~/.codex/prompts/`, in the user's home and
never from a repository. Custom prompts were deprecated and then removed in
Codex 0.117.0 (openai/codex#15941, 2026-03-27), whose note says to convert them
to skills. Codex discovers repository skills at `.agents/skills/<name>/SKILL.md`
with `name` and `description` frontmatter, and a person invokes one as
`$<name>` or from `/skills`. A skill has no `$ARGUMENTS`; it applies to the
message that invoked it.

## Decision drivers

1. One source per command, as for roles (ADR-0001).
2. Each host reads the command from the repository, so a consumer pulls it by
   path like any other output (ADR-0002).
3. A person invokes it by name on both hosts.

## Considered options

1. Claude `.claude/commands/<name>.md`; Codex `.agents/skills/<name>/SKILL.md`.
2. Codex `.codex/prompts/<name>.md` beside the agents.
3. Claude `.claude/skills/<name>/SKILL.md` as well, one form on both hosts.

## Decision

Option 1. `hosts/<host>.yaml` declares `commands_output`; an output ending in
`/SKILL.md` is rendered as a skill (`name`, `description`), any other as a
command file (`description`, `argument-hint`). `commands_arguments` declares the
text that replaces `$ARGUMENTS` on a host without it; Codex sets it.

## Consequences

- On Codex a command and a skill share one directory and one namespace, so a
  command named like a skill is an error on every host (F3 B-007).
- The roles and skills a command names are written by projection as each
  host's own paths, so a command body links nothing.
- Option 2 is rejected because Codex never read prompts from a repository and no
  longer reads them at all. Option 3 is rejected because a command in
  `.claude/skills/` sits beside the method skills, loses the `$ARGUMENTS` form,
  and is offered to the model as a skill it may pick on its own.
