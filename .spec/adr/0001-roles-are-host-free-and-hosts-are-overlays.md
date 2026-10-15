---
title: "ADR-0001: Roles are host-free and hosts are overlays"
description: "A role contract carries no model, effort or tool setting; each host supplies those in its own overlay file"
type: adr
---

# ADR-0001: Roles are host-free and hosts are overlays

## Status

accepted

## Context

A role must run on more than one AI host. Claude Code reads a Markdown file whose
frontmatter accepts `model`, `effort` and `tools`; Codex reads a TOML file whose
keys are `model` and `model_reasoning_effort`, with different model identifiers.
A role file that carries one host's keys cannot be projected to the other without
rewriting, and a model change then touches every role. See `scripts/.spec/README.md`
C-1 and `agents/.spec/README.md` B-001.

## Decision drivers

1. One source per role, read by every host.
2. A model or effort change for a host is one edit.
3. A role file is reviewable without host knowledge.

## Considered options

1. Host keys in each role's frontmatter, as the LTI roster does (`model: claude-opus-5`).
2. A `hosts:` block in each role's frontmatter carrying every host's keys.
3. A role file with no host keys, plus one overlay file per host under `hosts/`.

## Decision

Option 3. `agents/<role>.md` declares `name`, `description`, `version` and
`owned_spec_sections` only. `hosts/<host>.yaml` maps each role to that host's
model, effort and tool settings, with host-wide defaults. The projector merges the
two and refuses a role file that carries a host key.

## Consequences

- A new host is one overlay file and one renderer; no role file changes.
- A role's model is not visible in the role file; readers consult `hosts/`.
- Option 1 is rejected because the Claude model identifiers break Codex. Option 2
  is rejected because every host change edits every role.
