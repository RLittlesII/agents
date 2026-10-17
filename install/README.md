---
title: Install a host
description: Install this repository's complete Claude Code or Codex setup into a consumer project with Skillfile or Microsoft apm
type: guide
---

# Install a host

Clone or download this repository at the version you want. Run its installer
with Python 3.11 or newer and either [Skillfile](https://github.com/eljulians/skillfile)
or [Microsoft apm](https://github.com/microsoft/apm) already on PATH.
The consumer needs no .NET SDK and no Python packages for the installer itself.

```sh
python3 /path/to/agents/install/install.py --host claude --manager skillfile --target /path/to/project
python3 /path/to/agents/install/install.py --host codex --manager skillfile --target /path/to/project
python3 /path/to/agents/install/install.py --host claude --manager apm --target /path/to/project
python3 /path/to/agents/install/install.py --host codex --manager apm --target /path/to/project
```

On Windows, use your Python launcher and quote paths containing spaces:

```powershell
py -3.11 C:\src\agents\install\install.py --host codex --manager apm --target "C:\src\my project"
```

`--target` must name an existing project directory. The script's location
identifies the source checkout, so you can invoke it from any working directory.

| Host | Installed files |
| --- | --- |
| `claude` | `.claude/agents/*.md`, `.claude/skills/<name>/`, `.claude/commands/*.md` |
| `codex` | `.codex/agents/*.toml`, `.agents/skills/<name>/` including command skills, `.sdd/agents/*.md`, `.sdd/skills/<name>/` |

Codex's private `.sdd/` copies supply the authoritative source contracts its
agents reference. The installer adjusts that source path in the installed TOML;
role instructions, model settings and the committed checkout remain unchanged.
The private role index is generated for consumers and links the selected runtime
contracts and method guidance. Native and private skill copies omit `.issue`
tracker entries at every depth. Private method copies also omit `.spec`
metadata; other runtime resources, including nested and hidden files, retain
their bytes.
Producer documentation is not copied as the consumer index. On upgrades, review
and explicitly remove old private specifications left by earlier versions; the
installer's non-pruning policy also applies to those files.

Both managers install skills in a temporary project. apm receives each skill
as a separate local package to preserve links between skills. The installer verifies
their output and installs agents and Claude commands from the committed
projections, accommodating formats Skillfile does not support. Skillfile's
older `.codex/skills` staging layout is accepted; final Codex skills always go
to `.agents/skills`. The temporary manager manifests, locks and caches are
discarded, so rerun this script to update; the consumer's existing `Skillfile`
or `apm.yml` is not modified. Managers can still maintain their own user caches
and honor their own user configuration.

Identical files are left alone. A differing destination stops the installation
before any consumer file is changed. Review those differences, then add
`--force` to replace only the installer’s planned files. This option is never
passed to apm and does not bypass its security scan. Unrelated files, host
configuration, `AGENTS.md` and `CLAUDE.md` are preserved. Old files from an
earlier version are not pruned. After an upgrade that retires roles or commands,
review the old installation against the selected version and remove obsolete
installer files explicitly. A leftover native role or command can remain
discoverable; `--force` replaces planned files but does not remove those retired
entries. Preserve consumer-owned files when reconciling the installation.

Linked source, staged or destination paths beneath the selected roots are
rejected, including Windows junctions. Manager or validation failures leave
consumer files untouched. Caught publication failures restore changed files;
abrupt process termination or concurrent changes during publication are outside
that rollback guarantee.

Write your project's companion skill as described in
[sdd-conventions](../skills/sdd-conventions/SKILL.md#your-companion), supplying
the project's paths, commands and stack. Invoke commands as `/<name>` in
Claude Code or `$<name>` in Codex.

For contributors, the offline installer suite runs with:

```sh
python3 -m unittest discover -s install -p 'test_*.py' -v
```
