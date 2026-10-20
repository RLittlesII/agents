---
description: "Take a finished change through required review passes to an open pull request, and work each round of feedback until the person merges it."
argument-hint: "<item id, branch or pull request>"
---

Convene these roles as the body directs:

- `spec-reviewer`, whose contract is `.claude/agents/spec-reviewer.md`
- `adversary`, whose contract is `.claude/agents/adversary.md`
- `implementer`, whose contract is `.claude/agents/implementer.md`
- `spec-author`, whose contract is `.claude/agents/spec-author.md`
- `architect`, whose contract is `.claude/agents/architect.md`
- `test-writer`, whose contract is `.claude/agents/test-writer.md`
- `document-writer`, whose contract is `.claude/agents/document-writer.md`
- `build`, whose contract is `.claude/agents/build.md`

Read each role's contract before acting as it, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `deliver-change` (`.claude/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.claude/skills/spec-and-traceability/SKILL.md`).

The implementation lead supplies one identified review bundle. Run the diff pass
before the conditional surface pass when a boundary, credential, permission or
external input changes. Artifact owners address findings; reviewers re-read
corrections. After local gates, push, open the pull request and handle feedback
under the delivery rule.

Work on: $ARGUMENTS
