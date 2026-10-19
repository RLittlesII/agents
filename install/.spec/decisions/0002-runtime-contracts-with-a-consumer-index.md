---
title: "Decision 0002: Runtime contracts with a consumer index"
description: "Private Codex sources contain runtime contracts and resources, excluding maintainer specifications."
type: decision
---

# Decision 0002: Runtime contracts with a consumer index

**Date:** 2026-10-17
**Status:** accepted

The user selected: "Ship runtime contracts and resources with a consumer-facing
index; exclude maintainer specifications (Recommended)".

The private Codex bundle supplies the authoritative sources for selected native
roles, their runtime resources, a consumer-facing role index and method skills
with runtime support files. `.spec` metadata is excluded at every depth of a
private method directory, matching projection's existing boundary. The index
links the selected contracts and installed method entry points; it contains no
producer specification or repository-setup links. Source contracts, resources,
committed projections and consumer instructions remain byte-preserved.

Copying maintainer documents and their recursive dependency closure was rejected:
it would pull producer specifications, decisions, tool configuration and source
into a runtime package. Rewriting source role bodies was also rejected because
it changes the authoritative contracts. The consumer index is a separate derived
artifact; its generation does not rewrite a source role or the producer index.

This refines 0038-F1 B-006 and adds B-007; prefix rebasing, isolated manager
staging, destination conflict handling and the non-pruning policy are retained.
No third-party parser or dependency is introduced.
