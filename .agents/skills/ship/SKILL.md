---
name: ship
description: "Run the final gate before a release — the diff, the exposed surface and the repository at rest — then release with a rollback and explicit confirmation."
---

Follow the artifact ownership and review boundaries in the repository guidance.

Use these skills: `deliver-change` (`.agents/skills/deliver-change/SKILL.md`), `spec-and-traceability` (`.agents/skills/spec-and-traceability/SKILL.md`).

The independent reviewer judges the diff against the claims it cites. The surface pass
joins when the release touches a trust boundary, a credential, a permission or
input from outside; say which, or that it touches none. The repository pass reads the
repository at rest for drift. When an operational decision needs evidence, the
signal owner states signal requirements, the baseline and the
comparison to read after rollout. When persistence is affected, the storage specialist supplies
compatibility and verified rollback or recovery evidence. The performance owner joins
only for a written performance, cost or structural concern. The release owner then
versions, writes the changelog and stages the rollout, with the rollback
stated.

Work on: the request that invoked this skill
