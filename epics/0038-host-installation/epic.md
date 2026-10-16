---
title: "Epic 0038: Host installation"
description: "Consumers install the complete method for their selected host through their preferred package manager."
id: "0038"
type: epic
status: in-progress
priority: med
milestone: null
children: ["0038-F1"]
created: "2026-10-16"
updated: "2026-10-16"
github_issue: null
---

# Epic 0038: Host installation

## Summary

A consumer project receives the repository's complete roles, method skills
and commands for Claude Code or Codex through a cross-platform install script
that invokes Skillfile or Microsoft apm.

## Business Value

Consumers adopt the method without assembling individual paths, mixing host
formats or omitting contracts that installed instructions reference.

## Measure

The epic is delivered when the script installs a complete, usable host into a
fresh consumer project for each of the four host/manager combinations on
Windows, macOS and Linux, without requiring the projection toolchain.

## Features

| Feature | Name | Specification |
| --- | --- | --- |
| F1 | Host installation | [Host installation](../../install/.spec/README.md) |

## Out of this epic

- Projection and changes to host output formats belong to epic 0001.
- Consumer-specific companion skills belong to the consuming repository.
- User-wide installation and direct whole-repository package-manager installs
  are excluded by the user's project/script scope decision.
