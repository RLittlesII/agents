---
name: cleanup
description: Run a tidying pass over the repository — what is misfiled, what is stale, and what has drifted structurally — and deliver it as one reviewable change.
roles: [auditor, organizer, document-writer]
skills: [coding-conventions, deliver-change]
argument-hint: "[scope]"
---

The auditor reads the repository at rest and reports drift, grouped by rule.
The organizer files an item for each finding that needs a behaviour change, and
puts right the queue's own drift: what is misfiled, orphaned, or done in fact.
The document writer applies the rest as one reviewable change inside the given
scope.

Work on: $ARGUMENTS
