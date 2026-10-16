---
name: document
description: Repair documentation against what the repository actually holds, and regenerate what is generated rather than editing it.
roles: [document-writer]
skills: [coding-conventions]
argument-hint: "[documents or a scope]"
---

Check each document in scope against the code, the specifications and the
generated outputs it describes. Fix the wrong document, not the right one;
regenerate a generated block instead of editing it. A document that contradicts
a specification is a finding for the specification's owner. Report which
documents changed and which were found wrong and left alone.

Work on: $ARGUMENTS
