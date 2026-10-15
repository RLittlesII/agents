---
description: "Repair documentation against what the repository actually holds, and regenerate what is generated rather than editing it."
argument-hint: "[documents or a scope]"
---

Act as the `document-writer` role, whose contract is `.claude/agents/document-writer.md`. Read it before acting, and keep the boundaries it declares: a role that refuses a kind of work still refuses it here.

Use these skills: `coding-conventions` (`.claude/skills/coding-conventions/SKILL.md`).

Check each document in scope against the code, the specifications and the
generated outputs it describes. Fix the wrong document, not the right one;
regenerate a generated block instead of editing it.

**Preconditions and halts.** A document that contradicts a specification is a
finding for the specification's owner, not something to settle here. Report
which documents changed and which were found wrong and left alone.

Work on: $ARGUMENTS
