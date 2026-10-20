---
name: pr
description: Take a finished change through required review passes to an open pull request, and work each round of feedback until the person merges it.
roles: [spec-reviewer, adversary, implementer, spec-author, architect, test-writer, document-writer, build]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or pull request>"
---

The implementation lead supplies one review bundle: identified baseline and
target, or a working snapshot; the complete diff including deletions and untracked
changes; linked claims and decisions; applicable approvals and verification evidence.
The independent reviewer runs the diff pass, then the surface pass joins when the
change touches a boundary, credential, permission or input from outside; it does
not wait for a clean diff verdict. Reviewers route findings to artifact owners.
Relevant owners make bounded corrections; the implementation lead integrates
them. Reviewers re-read changed scope and affected dependencies at the new
identified snapshot, retaining applicable baseline approval. The owner of § 12
records returned verdicts. The implementation lead then runs the local gates,
pushes, opens the pull request citing the claims the commits satisfy, and works
each round of review feedback: a commit per thread, each thread resolved with a
reply naming only that commit, a disputed finding answered in its thread and
left open. The build owner joins only when a gate itself is wrong.

Work on: $ARGUMENTS
