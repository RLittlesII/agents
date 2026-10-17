---
title: Agent and skill schemas
description: Reusable frontmatter schemas and the Markdown document contracts enforced by projection
type: reference
---

# Agent and skill schemas

[agent.schema.json](agent.schema.json) and [skill.schema.json](skill.schema.json)
validate the YAML frontmatter mapping after parsing it into JSON-compatible
values. Both use JSON Schema Draft 2020-12. Reference the files at a pinned
repository tag or commit to use the same contract across projects.

Both require `name` and `description`. Descriptions may use folded (`>`) or
literal (`|`) YAML scalars. `metadata` supports the supplied author, technology,
complexity, version, domain, triggers, role, persona, scope, output-format and
related-skills fields, and permits other metadata extensions. Roles and personas
are optional; an agent's top-level `persona` mapping retains its required voice
and optional signing name. Host settings are optional agent fields.

The schemas' `$defs/document` subschemas validate an object containing the
frontmatter mapping and arrays of top-level Markdown title and section text.
The projector uses these document contracts to enforce required headings;
plain frontmatter validators can use the root schemas directly.

For this repository, source roles also require top-level `version` and
`owned_spec_sections`. Host overlays determine the settings emitted for each
host, even when optional host settings appear in source frontmatter. Schema
validation does not establish semantic correctness of the instructions.

Run `dotnet fsi scripts/project.fsx --check` for schema and output validation.
