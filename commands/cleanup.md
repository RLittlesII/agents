---
name: cleanup
description: Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change.
roles: [organizer, document-writer, architect]
skills: [coding-conventions, deliver-change]
argument-hint: "[scope]"
---

The organizer finds what is misfiled or orphaned in the queue. The document
writer finds what is stale in the prose. The architect finds structure that has
drifted from the recorded design. Each reports in its own voice before anything
moves.

**Preconditions and halts.** Nothing changes outside the agreed scope, and no
behaviour changes at all; a finding that needs one becomes a work item.

Work on: $ARGUMENTS
