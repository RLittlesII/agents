---
name: pr
description: Take a finished change through required review passes to an open pull request, and work each round of feedback until the person merges it.
roles: [spec-reviewer, adversary, implementer, spec-author, architect, test-writer, document-writer, build]
skills: [deliver-change, spec-and-traceability]
argument-hint: "<item id, branch or pull request>"
---

The implementation lead supplies one identified review bundle. Run the diff pass
before the conditional surface pass when a boundary, credential, permission or
external input changes. Artifact owners address findings; reviewers re-read
corrections. After local gates, push, open the pull request and handle feedback
under the delivery rule.

Work on: $ARGUMENTS
