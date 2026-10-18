---
title: "Lesson 0002: Projected skills keep the one-way boundary"
description: "A command source does not exempt its generated skill from the skill boundary"
type: lesson
---

# Lesson 0002: Projected skills keep the one-way boundary

## Incident and evidence

PR #9 review 5560928256 identified agent references in
`.agents/skills/spec/SKILL.md` at `c7e3a9d`. Its generated preamble named
`.codex/agents/spec-author.toml`. `commandPreamble` in `scripts/Projection.fsx`
used host agent paths for both native command files and skill outputs.

## Cause

The command-source exception to role-name lint was treated as permission for
its generated skill to name agents. Output discovery as a skill still imposes
the one-way boundary; choosing another host path would preserve the violation.

## Correction

0001-F3 B-008 replaces B-002. Skill outputs follow repository artifact ownership
and review boundaries, name only skill dependencies, and retain invocation
substitution. Shared command bodies describe owners and review passes. Native
command files keep explicit agent convening. ADR-0011 supersedes ADR-0005.

`F3_B008_CommandSkillUsesOwnershipWithoutAgentBindings` checks the emitted skill;
`F3_B008_AgentOutputPathCannotChangeCommandSkill` checks path independence;
`F3_B008_RepositoryCommandSkillsNameNoAgent` runs the role-name guard over real
command skills. All three failed before the correction. The existing skill
boundary remains the rule; this lesson records its projection failure.
