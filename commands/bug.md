---
name: bug
description: Reproduce a failure and prove its cause, pin it with a failing test, then fix it and record the lesson the fix reveals.
roles: [bug-finder, test-writer, bug-fixer]
skills: [test-from-scenarios, spec-and-traceability]
argument-hint: "<the report, or a path to it>"
---

The bug finder reproduces the failure with steps anyone can run, classifies it,
and traces the cause back from the symptom. Given several reports, it ranks
them and drops any it cannot attribute, with the reason. The test writer turns the
reproduction into a test that fails for that cause. The bug fixer makes it pass
and writes a lesson when the fix reveals a gap; a fix that reveals nothing writes
none.

**Preconditions and halts.** No fix before the reproduction is reliable, and no
close on a cause nobody proved. A failure that a claim does not cover goes to
the agreement owner before it is fixed.

Work on: $ARGUMENTS
