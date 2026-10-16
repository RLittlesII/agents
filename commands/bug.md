---
name: bug
description: Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson the fix reveals.
roles: [bug-finder, test-writer, implementer]
skills: [test-from-scenarios, spec-and-traceability, deliver-change]
argument-hint: "<the report, or a path to it>"
---

The bug finder reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. Given several reports, it ranks
them and drops any it cannot attribute, with the reason. When the report
carries a log, the finder reads it first for the sequence and the decisive line. The test writer turns the reproduction into a test that
fails for that cause. The implementer makes it pass and commits the fix with its
lesson when the fix reveals a gap; a fix that reveals nothing writes none.

Work on: $ARGUMENTS
