---
title: "ADR-0008: A role file declares its contract and no host setting"
description: "A role file carries name, description, version and owned_spec_sections, plus persona and reviews where the role has them; model, effort and tools stay in each host's overlay. Supersedes ADR-0001."
type: adr
---

# ADR-0008: A role file declares its contract and no host setting

## Status

accepted - 2026-10-15, by the repository owner (item 0034). Supersedes ADR-0001.

## Context

ADR-0001 decided that a role file is host-free and that `hosts/<host>.yaml`
carries each host's model, effort and tool settings. Its § Decision also fixed
the role file's keys: `name`, `description`, `version` and `owned_spec_sections`
"only". Two changes since then added keys without touching the record: ebecade
(2026-10-14) gave every role a `persona`, and ab40120 (2026-10-14) gave the four
reviewing roles a `reviews` list. `agents/.spec/README.md` § 7 lists six keys,
`loadRole` in `scripts/Projection.fsx` reads six, and every file in `agents/`
carries five or six.

The contract review (item 0033) found the sentence stale and filed it as out of
scope, because an accepted ADR is immutable: only its status changes, and a new
record supersedes it (`spec-and-traceability` § Records beside a
specification).
At stake: `agents/.spec/README.md` 0001-F2 B-001, B-007 and B-008, and
`scripts/.spec/README.md` 0001-F1 B-010.

## Decision drivers

1. One source per role, read by every host (carried from ADR-0001).
2. A model or effort change for a host is one edit (carried from ADR-0001).
3. A role file is reviewable without host knowledge (carried from ADR-0001).
4. A record that lists a key set names where that set is owned, so the next
   key does not strand the record as it stranded ADR-0001.

## Considered options

1. Amend ADR-0001 § Decision in place to list the current keys.
2. Leave ADR-0001 as it stands; `agents/.spec/README.md` § 7 is the current
   list.
3. Supersede ADR-0001 with a record that restates the host-free rule, names the
   owner of the key set, and records the keys as of this decision.

## Decision

Option 3. `agents/<role>.md` declares the role's contract and nothing a host
decides. The key set is declared by `loadRole` in `scripts/Projection.fsx` and
listed in `agents/.spec/README.md` § 7; as of this record it is:

| Key                   | Required | Declares                                                               |
| --------------------- | -------- | ---------------------------------------------------------------------- |
| `name`                | yes      | the role; equals the file stem (0001-F2 C-2)                           |
| `description`         | yes      | one line; the host's trigger text                                      |
| `version`             | yes      | semver, bumped when the contract changes                               |
| `owned_spec_sections` | yes      | the specification sections the role owns; an empty list is valid       |
| `persona`             | no       | `name` and `voice`, one line each; the name unique across roles (0001-F2 B-007) |
| `reviews`             | no       | the review units the role runs, declared in the roles index (0001-F2 B-008, B-009) |

`hosts/<host>.yaml` maps each role to that host's model, effort, tools and any
host-specific preload, with host-wide defaults. The projector merges the two and
refuses a role file carrying `model`, `effort`, `tools` or
`model_reasoning_effort` (0001-F1 B-010). A declaration that binds the roster
rather than one role — `unowned_spec_sections`, `review_units` — sits in the
frontmatter of the roles index, `agents/README.md`, not in a role file.

## Consequences

- ADR-0001's reasoning stays as history; its status points here. The Source
  cells in `agents/.spec/README.md` and `scripts/.spec/README.md` that cite
  ADR-0001 cite a rule this record carries unchanged; re-pointing them is their
  section owners' edit (item 0034 § out of scope).
- A new key is a 0001-F2 claim, a `loadRole` change and a § 7 row. It does not
  re-issue this record unless it moves the boundary between what a role
  declares and what a host decides.
- A new host is still one overlay file and one renderer; no role file changes.
  A role's model is still not visible in its file; readers consult `hosts/`.
- Option 1 is rejected because an accepted ADR is immutable. Option 2 is
  rejected because an accepted record then contradicts every file in `agents/`,
  and a reader of the ADRs believes it.
