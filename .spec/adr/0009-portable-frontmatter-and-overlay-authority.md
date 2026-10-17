---
title: "ADR-0009: Portable frontmatter and overlay authority"
description: "Reusable schemas accept portable metadata and host annotations; host overlays determine rendered settings. Supersedes ADR-0008."
type: adr
---

# ADR-0009: Portable frontmatter and overlay authority

## Status

accepted - 2026-10-16, from the repository owner's supplied agent and skill
frontmatter examples and instruction to fix schema enforcement. Supersedes ADR-0008.

## Context

The owner supplied valid skill frontmatter with folded descriptions and nested
metadata, and valid agent frontmatter with model, effort and metadata.version.
The old source-only format rejected host fields and multiline descriptions,
preventing its schema from serving other projects.

## Decision

The root schemas validate portable frontmatter directly. Their document
subschemas additionally describe required Markdown structure. Both permit
extensible metadata; the agent permits optional host annotations and persona.
This repository's source role loader additionally requires top-level version
and owned_spec_sections. Host overlays remain authoritative for rendered
settings; source annotations never override them. Descriptions retain their
value through both host encodings, including literal newlines.

## Considered options

- Keep the old restrictions: rejected because they exclude the supplied examples.
- Move host settings into role sources: rejected because it removes per-host authority.
- Accept portable fields while retaining overlay authority: selected.

## Consequences

Consumers can pin the same schemas while applying their own host configuration.
Existing source roles and generated outputs remain valid. Optional annotations
are accepted without changing configured host profiles. Semantic instruction
quality remains a review concern; schema validation covers data and structure.
