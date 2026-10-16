---
title: "Specification: Host installation"
description: "Install this repository's roles, method skills and commands into a consumer project through Skillfile or Microsoft apm."
type: feature
id: "F1"
epic: "0038"
spec_status: draft
status: in-progress
priority: med
value: 0
risk: 0
rank: 0
scored_by: null
scored_on: null
domain: "Specification-driven development"
author: "spec-author"
milestone: null
children: []
depends_on: []
blocks: []
spikes: []
created: "2026-10-16"
updated: "2026-10-16"
github_issue: null
synced_at: null
---

# Specification: Host installation

## 1. Business Goal

<!-- last written by: spec-author, 2026-10-16 -->

A consumer installs the complete method for its chosen AI host with one
cross-platform script and its preferred package manager, without choosing
individual files or receiving another host's formats.

## 2. User Needs

<!-- last written by: spec-author, 2026-10-16 -->

| # | Persona | Need | Pain Point Today |
| --- | --- | --- | --- |
| 1 | Consumer | Install every role, method skill and command together | Individual pulls omit dependencies |
| 2 | Claude Code user | Install Claude-native artifacts | Host-free sources are not the finished output |
| 3 | Codex user | Install Codex-native artifacts with usable source references | Markdown agent installation does not supply the projected TOML contracts |
| 4 | Team maintainer | Choose Skillfile or Microsoft apm in a consumer project | The README offers individual Skillfile additions only |

### Assumptions

None. Stakeholder-approved defaults are recorded in § 11.

## 3. Acceptance Criteria

<!-- last written by: spec-author, 2026-10-16 -->

| ID | Claim | Source | Status |
| --- | --- | --- | --- |
| B-001 | Given a consumer project and the Claude host selection, this Feature installs all committed Claude roles, method skills and commands into that project's Claude-native directories. | User request, 2026-10-16 | Active |
| B-002 | Given a consumer project and the Codex host selection, this Feature installs all committed Codex roles and method and command skills into that project's Codex-native directories. | User request, 2026-10-16 | Active |
| B-003 | Given either supported host and Skillfile as the selected manager, this Feature invokes Skillfile as part of installing the complete selected host. | User clarification, 2026-10-16 | Active |
| B-004 | Given either supported host and Microsoft apm as the selected manager, this Feature invokes Microsoft apm as part of installing the complete selected host. | User clarification, 2026-10-16 | Active |
| B-005 | Given the documented prerequisites on Windows, macOS or Linux, this Feature accepts the same script arguments and produces the selected host's installation. | User request for a cross-platform scripting language | Active |
| B-006 | Given an installed role or command, this Feature supplies the repository-owned contracts and skills that its instructions reference at their installed locations. | Complete installation in B-001 and B-002 | Active |

## 4. Constraints

<!-- last written by: spec-author, 2026-10-16 -->

| ID | Constraint | Rules Out |
| --- | --- | --- |
| C-1 | Installation is scoped to the selected consumer project. | User-wide host installation |
| C-2 | The installer consumes committed host projections. | Requiring consumers to run the F# projector or install the .NET SDK |
| C-3 | Installation preserves unrelated consumer instructions, configuration and artifacts. | Replacing entire host folders, root AGENTS.md or CLAUDE.md |
| C-4 | A source or destination link cannot redirect installation outside its declared boundary. | Following escaping symlinks or junctions |
| C-5 | Manager or validation failure produces an unsuccessful script exit. | Reporting a complete installation after a failed subprocess |

## 5. Out of Scope

<!-- last written by: spec-author, 2026-10-16 -->

| # | Item | Exclusion Reason |
| --- | --- | --- |
| 1 | Direct installation of the whole repository with only a package-manager command | The user selected a script that invokes the manager |
| 2 | User-wide installation | The user selected project folders |
| 3 | Hosts other than Claude Code and Codex | The request names these two hosts |
| 4 | Changing role contracts, host model mappings or command behavior | Projection remains owned by 0001-F1 and commands by 0001-F3 |
| 5 | Installing the host applications or package-manager executables | Consumers supply the documented prerequisites |
| 6 | Authoring a consumer's companion skill | Its paths, commands and stack belong to the consumer |

## 6. Concern Separation

Not authored; follows the agreed delivery sequence.

## 7. Technical Design

Not authored; follows the agreed delivery sequence.

## 8. Testing Strategy

Not authored; follows the agreed delivery sequence.

## 9. Traceability Matrix

| Claim ID | Scenario | Test | Status |
| --- | --- | --- | --- |
| B-001 | @B-001 in install.feature | Missing | Missing |
| B-002 | @B-002 in install.feature | Missing | Missing |
| B-003 | @B-003 in install.feature | Missing | Missing |
| B-004 | @B-004 in install.feature | Missing | Missing |
| B-005 | @B-005 in install.feature | Missing | Missing |
| B-006 | @B-006 in install.feature | Missing | Missing |

## 10. Lessons / Spec Deltas

<!-- last written by: spec-author, 2026-10-16 -->

None.

## 11. Open Questions

<!-- last written by: spec-author, 2026-10-16 -->

| ID | Question | Blocks | Resolution |
| --- | --- | --- | --- |
| OQ-1 | Which apm and installation scope are intended? | B-001–B-004 | Resolved by the user, 2026-10-16: Microsoft apm; project folders. |
| OQ-2 | Does compatibility mean script invocation or direct whole-repository installation through either manager? | B-003, B-004 | Resolved by the user, 2026-10-16: the script invokes either manager. |
| OQ-3 | Does this agreement belong in the proposed installation epic 0038, separate from projection epic 0001? | B-001–B-006 | Resolved by the user, 2026-10-16: approved the scope and defaults; epic 0038 accepted. |
| OQ-4 | May the design use Python 3.11+ with no third-party Python dependencies, explicit host and manager arguments, and a checkout as its source? | B-001–B-005 | Resolved by the user, 2026-10-16: approved. |
| OQ-5 | Should a differing destination file fail without overwriting it, with any replacement available only through an explicit installer option? | B-001, B-002, B-006 | Resolved by the user, 2026-10-16: approved; replacement requires --force. |

## 12. Sign-off

<!-- last written by: spec-author, 2026-10-16 -->

| Section | Status | Reviewer | Note |
| --- | --- | --- | --- |
| 1-5 | 🟢 | spec-reviewer | Stakeholder and Jessica approved the agreement, 2026-10-16. |
| 6-12 | 🟡 | spec-reviewer | Design, testing and final review are recorded in subsequent commits. |

## Tasks

[0039 — Host installer](../.issue/0039-host-installer.yml), B-001–B-006.

## Scoring

| Field | Value | Basis |
| --- | --- | --- |
| value | 0 | Not scored before agreement |
| risk | 0 | Manager compatibility and source references need design validation |
