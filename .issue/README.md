---
title: Historical work-item identities
description: Explicit resolution of published work-item ID collisions
type: reference
---

# Historical work-item identities

## One-time correction, 2026-10-17

The repository owner authorized correcting the later published `0036`, retaining
the earlier identity and recording historical aliases. This exception applies
only to this collision; the permanent-ID rule in
[`sdd-conventions`](../skills/sdd-conventions/SKILL.md#id-schemes) still governs
other allocations and collisions.

| Historical identity | Current record | Evidence |
| --- | --- | --- |
| `0036`, `.issue/0036-groom-role-handoffs.yml` | [0036 role handoffs](0036-groom-role-handoffs.yml) | [PR #1](https://github.com/RLittlesII/agents/pull/1), merged `5924065` |
| `0036`, `.issue/0036-resolve-f5-open-questions.yml` | [0047 Next-work rulings](0047-resolve-f5-open-questions.yml) | [PR #2](https://github.com/RLittlesII/agents/pull/2), merged `785de62` |

Current bare `0036` resolves to role handoffs; current bare `0047` resolves to
Next-work rulings. A historical bare `0036` requires its path, PR or surrounding
scope to distinguish the two. An ambiguous historical citation remains a
diagnostic; this table does not guess its target. Git history and published PR
citations remain intact.

[0037](0037-align-f5-design-with-rulings.yml) now depends on `0047`, preserving
its original Next-work prerequisite. Allocation checked current main `d4eac9c`,
GitHub PR reservations, local branches and all local worktrees; `0047` was free.
