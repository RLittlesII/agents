---
name: review
description: Judge a diff against the claims and decisions it cites, and add a hostile pass when it touches a boundary; findings ranked most severe first.
roles: [spec-reviewer, adversary]
skills: [spec-and-traceability, coding-conventions]
argument-hint: "<diff, branch or path>"
---

The independent reviewer reads the diff against every claim it cites and every
accepted decision it touches. The surface pass joins only when the diff changes a
trust boundary, a credential, an input from outside, or a permission; say which
of those it touches, or that it touches none.

Work on: $ARGUMENTS
