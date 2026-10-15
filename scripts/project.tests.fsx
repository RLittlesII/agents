// Tests for the projector. Names cite the claims they cover:
//   scripts/.spec/README.md  (0001-F1 Projection)    B001_..B015_
//   agents/.spec/README.md   (0001-F2 Role contract) F2_B001_..F2_B008_
//   commands/.spec/README.md (0001-F3 Commands)      F3_B001_..F3_B007_
//
//   dotnet fsi scripts/project.tests.fsx        exit code = number of failures

#load "Projection.fsx"

open System
open System.IO
open System.Text
open Projection

let repoRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))

// ---------------------------------------------------------------- Fixtures

let template =
    String.concat "\n"
        [ "## 1. Business Goal"; ""; "## 2. User Needs"; ""; "## 3. Acceptance Criteria"
          ""; "## Tasks"; ""; "## Scoring"; "" ]

let claudeHost = """output: .claude/agents/{name}.md
skills_output: .claude/skills
defaults:
  model: inherit
  effort: medium
roles:
  alpha:
    model: opus
    effort: high
    skills: [sdd-conventions]
"""

let codexHost = """output: .codex/agents/{name}.toml
skills_output: .agents/skills
instructions_prefix: "Read AGENTS.md first. The authoritative role source is {source}. Resolve its relative links against that path in the repository."
defaults:
  model: gpt-6.1-sol
  model_reasoning_effort: medium
roles:
  alpha:
    model_reasoning_effort: high
"""

let companion = """---
name: sdd-conventions
---

## Section ownership

<!-- BEGIN GENERATED: section-ownership -->
<!-- END GENERATED: section-ownership -->

The `.feature` file belongs to `spec-author`.

## Review passes

<!-- BEGIN GENERATED: review-passes -->
<!-- END GENERATED: review-passes -->
"""

let readme = """---
title: Roles
unowned_spec_sections:
  Scoring: derived — recompute, never hand-edit
---
"""

let personas = """---
title: Personas
---

<!-- BEGIN GENERATED: personas -->
<!-- END GENERATED: personas -->
"""

let role name (owned: string) (body: string) =
    sprintf "---\nname: %s\ndescription: Does %s things.\nversion: 0.1.0\nowned_spec_sections: %s\n---\n%s" name name owned body

/// A role with a persona block; `persona` is the YAML under `persona:`.
let personaRole name (owned: string) (persona: string) (body: string) =
    sprintf "---\nname: %s\ndescription: Does %s things.\nversion: 0.1.0\nowned_spec_sections: %s\npersona:\n%s\n---\n%s" name name owned persona body

let alphaBody = "\n# Alpha\n\nReads [`sdd-conventions`](../skills/sdd-conventions/SKILL.md).\n"
let betaBody = "\n# Beta\n\nOwns nothing.\n"

let write (root: string) (rel: string) (text: string) =
    let path = Path.Combine(root, rel)
    Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
    File.WriteAllText(path, text, UTF8Encoding(false))

/// A synthetic checkout: two roles, both hosts, one skill, the companion, an
/// empty persona roster and a template with three numbered sections plus Tasks
/// and Scoring.
let checkout () =
    let root = Path.Combine(Path.GetTempPath(), "projection-tests", Guid.NewGuid().ToString("N"))
    Directory.CreateDirectory root |> ignore
    write root ".specht/templates/feature.md" template
    write root "hosts/claude.yaml" claudeHost
    write root "hosts/codex.yaml" codexHost
    write root "skills/sdd-conventions/SKILL.md" companion
    write root "skills/sdd-conventions/references/extra.md" "extra\n"
    write root "agents/README.md" readme
    write root "agents/PERSONAS.md" personas
    write root "agents/alpha.md" (role "alpha" "[\"1\", \"2\", \"Tasks\"]" alphaBody)
    write root "agents/beta.md" (role "beta" "[\"3\"]" betaBody)
    root

let read (root: string) (rel: string) = File.ReadAllText(Path.Combine(root, rel), Encoding.UTF8)

let exists (root: string) (rel: string) = File.Exists(Path.Combine(root, rel))

let expectFailure (f: unit -> 'a) : string =
    try
        f () |> ignore
        failwith "expected a ProjectionError"
    with ProjectionError message -> message

let contains (needle: string) (haystack: string) =
    if not (haystack.Contains needle) then
        failwithf "expected to find %A in:\n%s" needle haystack

let equal (expected: 'a) (actual: 'a) =
    if expected <> actual then failwithf "expected %A but got %A" expected actual

let nothingWritten (root: string) =
    for dir in [ ".claude"; ".codex"; ".agents" ] do
        if Directory.Exists(Path.Combine(root, dir)) then failwithf "%s was written" dir

// ---------------------------------------------------------------- 0001-F1 Projection

let B001_RoleAndClaudeOverlay_EmitsFrontmatterAndVerbatimBody () =
    let root = checkout ()
    apply root (plan root) |> ignore
    let text = read root ".claude/agents/alpha.md"
    let expected =
        "---\nname: alpha\ndescription: \"Does alpha things.\"\nmodel: opus\neffort: high\nskills: [sdd-conventions]\n---\n" + alphaBody
    equal expected text
    // A role absent from the overlay gets the defaults.
    let beta = read root ".claude/agents/beta.md"
    equal ("---\nname: beta\ndescription: \"Does beta things.\"\nmodel: inherit\neffort: medium\n---\n" + betaBody) beta

let B002_RoleAndCodexOverlay_EmitsTomlWithPrefixedInstructions () =
    let root = checkout ()
    apply root (plan root) |> ignore
    let text = read root ".codex/agents/alpha.toml"
    let lines = text.Split('\n')
    equal "name = \"alpha\"" lines.[0]
    equal "description = \"Does alpha things.\"" lines.[1]
    contains "developer_instructions = \"Read AGENTS.md first. The authoritative role source is agents/alpha.md. Resolve its relative links against that path in the repository.\\n\\n# Alpha\\n\\n" lines.[2]
    equal "model = \"gpt-6.1-sol\"" lines.[3]
    equal "model_reasoning_effort = \"high\"" lines.[4]

/// Decode a TOML basic string the way a TOML parser does; enough for the claim.
let decodeToml (literal: string) =
    if not (literal.StartsWith "\"" && literal.EndsWith "\"") then failwith "not a basic string"
    let inner = literal.Substring(1, literal.Length - 2)
    let sb = StringBuilder()
    let mutable i = 0
    while i < inner.Length do
        if inner.[i] = '\\' then
            i <- i + 1
            match inner.[i] with
            | 'n' -> sb.Append '\n' |> ignore
            | 't' -> sb.Append '\t' |> ignore
            | 'r' -> sb.Append '\r' |> ignore
            | 'b' -> sb.Append '\b' |> ignore
            | 'f' -> sb.Append '\012' |> ignore
            | '"' -> sb.Append '"' |> ignore
            | '\\' -> sb.Append '\\' |> ignore
            | 'u' ->
                sb.Append(char (Convert.ToInt32(inner.Substring(i + 1, 4), 16))) |> ignore
                i <- i + 4
            | other -> failwithf "escape \\%c is not valid TOML" other
            i <- i + 1
        else
            if inner.[i] < ' ' then failwith "raw control character in a TOML basic string"
            sb.Append inner.[i] |> ignore
            i <- i + 1
    sb.ToString()

let B003_HostileBody_RoundTripsThroughToml () =
    let root = checkout ()
    let body = "\n# Alpha\n\nQuotes \"here\", a back\\slash, a tab\there, § and émoji 🟡 🟢.\n\n```\nraw \"\"\" triple\n```\n"
    write root "agents/alpha.md" (role "alpha" "[\"1\", \"2\", \"Tasks\"]" body)
    apply root (plan root) |> ignore
    let text = read root ".codex/agents/alpha.toml"
    let line = text.Split('\n') |> Array.find (fun l -> l.StartsWith "developer_instructions = ")
    let decoded = decodeToml (line.Substring "developer_instructions = ".Length)
    let prefix = "Read AGENTS.md first. The authoritative role source is agents/alpha.md. Resolve its relative links against that path in the repository.\n"
    equal (prefix + body) decoded
    if text.Contains "\\ud83d" || text.Contains "\\uD83D" then failwith "astral character was escaped as a surrogate pair"

let B004_SkillDirectory_CopiedByteForByteToBothHosts () =
    let root = checkout ()
    write root "skills/other/SKILL.md" "---\nname: other\n---\n\nOther ✓\r\nwith CRLF kept\n"
    write root "skills/other/assets/blob.bin" "\000\001binary"
    apply root (plan root) |> ignore
    for host in [ ".claude/skills"; ".agents/skills" ] do
        equal (File.ReadAllBytes(Path.Combine(root, "skills/other/SKILL.md"))) (File.ReadAllBytes(Path.Combine(root, host, "other/SKILL.md")))
        equal (File.ReadAllBytes(Path.Combine(root, "skills/other/assets/blob.bin"))) (File.ReadAllBytes(Path.Combine(root, host, "other/assets/blob.bin")))
        equal "extra\n" (read root (host + "/sdd-conventions/references/extra.md"))
        // The companion copy carries the generated table, same bytes as the source.
        equal (read root "skills/sdd-conventions/SKILL.md") (read root (host + "/sdd-conventions/SKILL.md"))

let B005_OwnedSections_WriteOwnershipTableBetweenMarkers () =
    let root = checkout ()
    apply root (plan root) |> ignore
    let text = read root "skills/sdd-conventions/SKILL.md"
    let expected =
        String.concat "\n"
            [ beginMarker
              "| Section                | Owner                                |"
              "| ---------------------- | ------------------------------------ |"
              "| 1. Business Goal       | `alpha`                              |"
              "| 2. User Needs          | `alpha`                              |"
              "| 3. Acceptance Criteria | `beta`                               |"
              "| Tasks                  | `alpha`                              |"
              "| Scoring                | derived — recompute, never hand-edit |"
              endMarker ]
    contains expected text
    contains "The `.feature` file belongs to `spec-author`." text
    // Idempotent: a second plan reproduces the same bytes.
    equal text (Encoding.UTF8.GetString (plan root).Outputs.["skills/sdd-conventions/SKILL.md"])

let B005b_Subsection_RendersUnderItsParentAndIsExemptFromTotality () =
    let root = checkout ()
    write root "agents/gamma.md" (role "gamma" "[\"2/Assumptions\"]" "\n# Gamma\n")
    let rows = ownershipRows (loadRoles root) (loadUnowned root) (loadTemplateSections root)
    equal [ "1. Business Goal", "`alpha`"; "2. User Needs", "`alpha`"; "2. › `### Assumptions`", "`gamma`"
            "3. Acceptance Criteria", "`beta`"; "Tasks", "`alpha`"; "Scoring", "derived — recompute, never hand-edit" ] rows

let B006_TwoRolesClaimOneSection_FailsNamingBoth () =
    let root = checkout ()
    write root "agents/beta.md" (role "beta" "[\"2\", \"3\"]" betaBody)
    let message = expectFailure (fun () -> plan root)
    contains "section '2'" message
    contains "'`alpha`'" message
    contains "'`beta`'" message
    nothingWritten root

let B007_UnownedTemplateSection_FailsNamingIt () =
    let root = checkout ()
    write root "agents/beta.md" (role "beta" "[]" betaBody)
    let message = expectFailure (fun () -> plan root)
    contains "owned by nobody: 3" message
    nothingWritten root
    // The other direction: a claim the template lacks.
    let root2 = checkout ()
    write root2 "agents/beta.md" (role "beta" "[\"3\", \"4\"]" betaBody)
    let message2 = expectFailure (fun () -> plan root2)
    contains "absent from the template: 4" message2

let B008_Check_IsSilentWhenCurrentAndNamesEveryDrift () =
    let root = checkout ()
    apply root (plan root) |> ignore
    equal [] (differences root (plan root))
    // changed
    write root ".claude/agents/alpha.md" "tampered"
    // missing
    File.Delete(Path.Combine(root, ".codex/agents/beta.toml"))
    // stale
    write root ".agents/skills/old/SKILL.md" "gone from source"
    let before = read root ".claude/agents/beta.md"
    let diffs = differences root (plan root)
    equal [ Changed ".claude/agents/alpha.md"; Missing ".codex/agents/beta.toml"; Stale ".agents/skills/old/SKILL.md" ] diffs
    equal before (read root ".claude/agents/beta.md")
    equal "tampered" (read root ".claude/agents/alpha.md")
    // apply repairs all three
    apply root (plan root) |> ignore
    equal [] (differences root (plan root))
    if exists root ".agents/skills/old/SKILL.md" then failwith "stale file survived apply"

let B009_OverlayNamesMissingRole_Fails () =
    let root = checkout ()
    write root "hosts/codex.yaml" (codexHost + "  delta: {}\n")
    let message = expectFailure (fun () -> plan root)
    contains "hosts/codex.yaml: role 'delta' has no file agents/delta.md" message
    nothingWritten root

let B009b_RoleNameDiffersFromFileName_Fails () =
    let root = checkout ()
    write root "agents/beta.md" (role "gamma" "[\"3\"]" betaBody)
    let message = expectFailure (fun () -> plan root)
    contains "agents/beta.md: name 'gamma' does not match the file name 'beta'" message
    nothingWritten root

let B010_RoleFileCarriesHostKey_FailsNamingTheKey () =
    for key in [ "model: opus"; "effort: high"; "tools: [Read]"; "model_reasoning_effort: high" ] do
        let root = checkout ()
        let name = key.Substring(0, key.IndexOf ':')
        write root "agents/beta.md" (sprintf "---\nname: beta\ndescription: Beta.\nversion: 0.1.0\nowned_spec_sections: [\"3\"]\n%s\n---\n%s" key betaBody)
        let message = expectFailure (fun () -> plan root)
        contains (sprintf "host key '%s'" name) message
        nothingWritten root

let B011_Personas_WriteRosterBetweenMarkers () =
    let root = checkout ()
    write root "agents/alpha.md" (personaRole "alpha" "[\"1\", \"2\", \"Tasks\"]" "  name: Zed\n  voice: Terse." alphaBody)
    write root "agents/beta.md" (personaRole "beta" "[\"3\"]" "  name: Abe\n  voice: \"Asks first — then acts.\"" betaBody)
    apply root (plan root) |> ignore
    let text = read root "agents/PERSONAS.md"
    let expected =
        String.concat "\n"
            [ "<!-- BEGIN GENERATED: personas -->"
              "| Persona | Role                | Voice                   |"
              "| ------- | ------------------- | ----------------------- |"
              "| Abe     | [`beta`](beta.md)   | Asks first — then acts. |"
              "| Zed     | [`alpha`](alpha.md) | Terse.                  |"
              "<!-- END GENERATED: personas -->" ]
    contains expected text
    contains "title: Personas" text
    // Idempotent, and a role without a persona has no row.
    equal text (Encoding.UTF8.GetString (plan root).Outputs.["agents/PERSONAS.md"])
    write root "agents/beta.md" (role "beta" "[\"3\"]" betaBody)
    let without = Encoding.UTF8.GetString (plan root).Outputs.["agents/PERSONAS.md"]
    if without.Contains "Abe" then failwith "a role without a persona has a row"
    // The roster is not a role.
    equal [ "alpha"; "beta" ] (loadRoles root |> List.map (fun r -> r.Name))

let B012_Persona_PrecedesVerbatimBodyOnBothHosts () =
    let root = checkout ()
    write root "agents/alpha.md" (personaRole "alpha" "[\"1\", \"2\", \"Tasks\"]" "  name: Zed\n  voice: Terse." alphaBody)
    apply root (plan root) |> ignore
    let line = "Persona: **Zed**. Terse. Sign the prose you write — reports, findings, notes — as Zed; a stamp a tool reads, such as `last written by` or `author`, keeps the role name `alpha`."
    let claude = read root ".claude/agents/alpha.md"
    equal ("---\nname: alpha\ndescription: \"Does alpha things.\"\nmodel: opus\neffort: high\nskills: [sdd-conventions]\n---\n\n" + line + "\n" + alphaBody) claude
    let codex = read root ".codex/agents/alpha.toml"
    let instructions = codex.Split('\n') |> Array.find (fun l -> l.StartsWith "developer_instructions = ")
    let prefix = "Read AGENTS.md first. The authoritative role source is agents/alpha.md. Resolve its relative links against that path in the repository."
    equal (prefix + "\n\n" + line + "\n" + alphaBody) (decodeToml (instructions.Substring "developer_instructions = ".Length))
    // A role without a persona is unchanged: beta's body follows the frontmatter directly.
    contains ("---\n" + betaBody) (read root ".claude/agents/beta.md")

let reviewBody title = sprintf "\n# %s\n\n## Look for\n\n- Drift.\n\n## Report\n\nOne finding per line.\n" title

/// The synthetic checkout with two review units, alpha reviewing the diff and
/// beta the design.
let reviewsCheckout () =
    let root = checkout ()
    write root "agents/README.md" (readme.Replace("---\n", "") |> fun front -> "---\n" + front + "review_units:\n  design: a design\n  diff: a diff\n---\n")
    write root "agents/alpha.md" (role "alpha" "[\"1\", \"2\", \"Tasks\"]\nreviews: [diff]" (reviewBody "Alpha"))
    write root "agents/beta.md" (role "beta" "[\"3\"]\nreviews: [design]" (reviewBody "Beta"))
    root

let B013_ReviewUnits_WriteRegistryBetweenMarkers () =
    let root = reviewsCheckout ()
    apply root (plan root) |> ignore
    let text = read root "skills/sdd-conventions/SKILL.md"
    let expected =
        String.concat "\n"
            [ "<!-- BEGIN GENERATED: review-passes -->"
              "| Order | Unit   | Reviews  | Reviewer |"
              "| ----- | ------ | -------- | -------- |"
              "| 1     | design | a design | `beta`   |"
              "| 2     | diff   | a diff   | `alpha`  |"
              "<!-- END GENERATED: review-passes -->" ]
    contains expected text
    // The ownership table still sits between its own markers.
    contains "| 3. Acceptance Criteria | `beta`" text
    equal text (Encoding.UTF8.GetString (plan root).Outputs.["skills/sdd-conventions/SKILL.md"])

let B014_TwoRolesReviewOneUnit_FailsNamingBoth () =
    let root = reviewsCheckout ()
    write root "agents/beta.md" (role "beta" "[\"3\"]\nreviews: [design, diff]" (reviewBody "Beta"))
    contains "review unit 'diff' is reviewed by both 'alpha' and 'beta'" (expectFailure (fun () -> plan root))
    nothingWritten root

let B015_UnreviewedOrUndeclaredUnit_FailsNamingIt () =
    let root = reviewsCheckout ()
    write root "agents/beta.md" (role "beta" "[\"3\"]" betaBody)
    contains "not total; reviewed by nobody: design" (expectFailure (fun () -> plan root))
    nothingWritten root
    let root2 = reviewsCheckout ()
    write root2 "agents/beta.md" (role "beta" "[\"3\"]\nreviews: [design, style]" (reviewBody "Beta"))
    contains "agents/beta.md: reviews 'style', which agents/README.md review_units does not declare" (expectFailure (fun () -> plan root2))
    nothingWritten root2

let C3_Outputs_AreDeterministicAcrossRuns () =
    let root = checkout ()
    let first = (plan root).Outputs
    let second = (plan root).Outputs
    equal (first |> Map.toList |> List.map fst) (second |> Map.toList |> List.map fst)
    for KeyValue(rel, content) in first do
        equal content second.[rel]
        if content.Length > 0 && content.[0] = 0xEFuy then failwithf "%s has a BOM" rel
        if (Encoding.UTF8.GetString content).Contains root then failwithf "%s carries an absolute path" rel

// ---------------------------------------------------------------- 0001-F2 Role contract

let roster =
    [ "adversary"; "analyst"; "architect"; "auditor"; "benchmarker"; "bug-finder"; "bug-fixer"; "build"
      "critic"; "document-writer"; "domain"; "implementer"; "log-parser"; "metric-nerd"; "organizer"
      "pair-programmer"; "release"; "spec-author"; "spec-reviewer"; "test-writer"; "tester"; "ui"; "ux"; "yak-shaver" ]

let F2_B001_EveryRoleDeclaresTheContractKeysAndNoHostKey () =
    // loadRole itself rejects a missing key, a non-semver version, a multi-line
    // description and a host key.
    let roles = loadRoles repoRoot
    equal roster (roles |> List.map (fun r -> r.Name))
    for role in roles do
        if String.IsNullOrWhiteSpace role.Description then failwithf "%s has no description" role.Name
    // Every role is mapped on both hosts.
    for id in [ "claude"; "codex" ] do
        let host = loadHost repoRoot id
        for role in roles do
            if not (host.Roles |> List.exists (fun (r, _) -> r = role.Name)) then
                failwithf "hosts/%s.yaml does not map %s" id role.Name

let F2_B002_EveryTemplateSectionHasExactlyOneOwner () =
    let rows = ownershipRows (loadRoles repoRoot) (loadUnowned repoRoot) (loadTemplateSections repoRoot)
    let sections = loadTemplateSections repoRoot
    equal sections.Length (rows |> List.filter (fun (s, _) -> not (s.Contains "›")) |> List.length)
    equal 15 rows.Length

let F2_B003_EveryRoleBodyCarriesTheFourSections () =
    for role in loadRoles repoRoot do
        let heads = role.Body.Split('\n') |> Array.filter (fun l -> l.StartsWith "## ") |> Array.map (fun l -> l.Substring 3) |> List.ofArray
        for required in [ "Owns"; "Read first"; "Refuse" ] do
            if not (List.contains required heads) then failwithf "%s lacks ## %s" role.Name required
        if not (List.contains "Produce" heads || List.contains "Look for" heads) then
            failwithf "%s lacks ## Produce or ## Look for" role.Name

let F2_B005_LookForRoleCarriesReport () =
    let reviewing = loadRoles repoRoot |> List.filter (fun r -> r.Body.Contains "\n## Look for\n")
    if reviewing.IsEmpty then failwith "no role carries ## Look for"
    for role in reviewing do
        if not (role.Body.Contains "\n## Report\n") then failwithf "%s has ## Look for but no ## Report" role.Name

let F2_B006_EverySkillLinkResolves () =
    // The real checkout passes.
    validateSkillLinks repoRoot (loadRoles repoRoot)
    // A link to a skill with no directory fails naming the role and the skill.
    let root = checkout ()
    write root "agents/beta.md" (role "beta" "[\"3\"]" "\n# Beta\n\nReads [`ghost`](../skills/ghost/SKILL.md).\n")
    let message = expectFailure (fun () -> plan root)
    contains "agents/beta.md: links skill 'ghost'" message
    nothingWritten root

let F2_B007_PersonaRequiresNameAndVoiceAndIsUnique () =
    // The real roster: every persona loads and no name is shared.
    let roles = loadRoles repoRoot
    validatePersonas roles
    for r in roles do
        match r.Persona with
        | Some p when p.Name = r.Name -> failwithf "%s signs with its role name" r.Name
        | _ -> ()
    // A persona without a voice fails naming the role and the key.
    let root = checkout ()
    write root "agents/beta.md" (personaRole "beta" "[\"3\"]" "  name: Abe" betaBody)
    contains "agents/beta.md: persona.voice is required" (expectFailure (fun () -> plan root))
    nothingWritten root
    // A persona that is not a mapping fails.
    let root2 = checkout ()
    write root2 "agents/beta.md" (role "beta" "[\"3\"]\npersona: Abe" betaBody)
    contains "agents/beta.md: persona must be a mapping" (expectFailure (fun () -> plan root2))
    // Two roles signing with one name fails naming the persona and both roles.
    let root3 = checkout ()
    write root3 "agents/alpha.md" (personaRole "alpha" "[\"1\", \"2\", \"Tasks\"]" "  name: Abe\n  voice: Terse." alphaBody)
    write root3 "agents/beta.md" (personaRole "beta" "[\"3\"]" "  name: Abe\n  voice: Calm." betaBody)
    let message = expectFailure (fun () -> plan root3)
    contains "persona 'Abe' is declared by both 'alpha' and 'beta'" message
    nothingWritten root3

let F2_B008_ReviewingRoleCarriesLookForAndReport () =
    // The real roster: exactly the four review passes, each on a reviewing body.
    let reviewing = loadRoles repoRoot |> List.filter (fun r -> not r.Reviews.IsEmpty) |> List.map (fun r -> r.Name, r.Reviews)
    equal [ "adversary", [ "surface" ]; "auditor", [ "repository" ]; "critic", [ "design" ]; "spec-reviewer", [ "diff" ] ] reviewing
    // A role declaring a unit without the reviewing headings fails.
    let root = checkout ()
    write root "agents/beta.md" (role "beta" "[\"3\"]\nreviews: [diff]" betaBody)
    contains "agents/beta.md: reviews diff but lacks ## Look for or ## Report" (expectFailure (fun () -> plan root))
    nothingWritten root

// ---------------------------------------------------------------- 0001-F3 Commands

let command name (roles: string) (skills: string) (body: string) =
    sprintf "---\nname: %s\ndescription: Convene for %s.\nroles: %s\nskills: %s\nargument-hint: \"<what>\"\n---\n%s" name name roles skills body

let commandBody = "\nDo the thing.\n\nWork on: $ARGUMENTS\n"

/// The synthetic checkout with commands enabled on both hosts and one command,
/// `convene`, naming both roles and the companion skill.
let commandsCheckout () =
    let root = checkout ()
    write root "hosts/claude.yaml" (claudeHost + "commands_output: .claude/commands/{name}.md\n")
    write root "hosts/codex.yaml" (codexHost + "commands_output: .agents/skills/{name}/SKILL.md\ncommands_arguments: the request that invoked this skill\n")
    write root "commands/convene.md" (command "convene" "[alpha, beta]" "[sdd-conventions]" commandBody)
    root

let keep = "keep the boundaries it declares: a role that refuses a kind of work still refuses it here."

let F3_B001_CommandProjectsToClaudeCommandFile () =
    let root = commandsCheckout ()
    apply root (plan root) |> ignore
    let expected =
        String.concat "\n"
            [ "---"
              "description: \"Convene for convene.\""
              "argument-hint: \"<what>\""
              "---"
              ""
              "Convene these roles, in this order:"
              ""
              "- `alpha`, whose contract is `.claude/agents/alpha.md`"
              "- `beta`, whose contract is `.claude/agents/beta.md`"
              ""
              "Read each role's contract before acting as it, and " + keep
              ""
              "Use these skills: `sdd-conventions` (`.claude/skills/sdd-conventions/SKILL.md`)."
              "" ]
    equal (expected + commandBody) (read root ".claude/commands/convene.md")

let F3_B002_CommandProjectsToCodexSkill () =
    let root = commandsCheckout ()
    write root "commands/solo.md" (command "solo" "[beta]" "[]" commandBody)
    apply root (plan root) |> ignore
    let expected =
        String.concat "\n"
            [ "---"
              "name: solo"
              "description: \"Convene for solo.\""
              "---"
              ""
              "Act as the `beta` role, whose contract is `.codex/agents/beta.toml`. Read it before acting, and " + keep
              ""
              "Do the thing."
              ""
              "Work on: the request that invoked this skill"
              "" ]
    equal expected (read root ".agents/skills/solo/SKILL.md")
    contains "`sdd-conventions` (`.agents/skills/sdd-conventions/SKILL.md`)" (read root ".agents/skills/convene/SKILL.md")
    // Claude keeps the placeholder; it substitutes the arguments itself.
    contains "Work on: $ARGUMENTS" (read root ".claude/commands/solo.md")

let F3_B003_CommandNamingMissingRole_FailsNamingBoth () =
    // The real commands name only roles and skills that exist.
    let real = loadCommands repoRoot
    if real.IsEmpty then failwith "commands/ holds no command"
    let skills = Directory.EnumerateDirectories(Path.Combine(repoRoot, "skills")) |> Seq.map Path.GetFileName |> List.ofSeq
    validateCommands (loadRoles repoRoot) skills real
    let root = commandsCheckout ()
    write root "commands/convene.md" (command "convene" "[alpha, ghost]" "[]" commandBody)
    contains "commands/convene.md: names role 'ghost'" (expectFailure (fun () -> plan root))
    nothingWritten root

let F3_B004_CommandNamingMissingSkill_FailsNamingBoth () =
    let root = commandsCheckout ()
    write root "commands/convene.md" (command "convene" "[alpha]" "[ghost]" commandBody)
    contains "commands/convene.md: names skill 'ghost'" (expectFailure (fun () -> plan root))
    nothingWritten root

let F3_B005_MalformedCommand_Fails () =
    let cases =
        [ "convene.md", command "other" "[alpha]" "[]" commandBody, "name 'other' does not match the file name 'convene'"
          "convene.md", "---\nname: convene\ndescription: |\n  Two\n  lines.\nroles: [alpha]\n---\n" + commandBody, "description is required and must be a single line"
          "convene.md", command "convene" "[]" "[]" commandBody, "roles must name at least one role"
          "convene.md", command "convene" "[alpha]" "[]" "\nSee [alpha](../agents/alpha.md).\n", "the body links into agents/ or skills/" ]
    for file, text, expected in cases do
        let root = commandsCheckout ()
        write root ("commands/" + file) text
        contains expected (expectFailure (fun () -> plan root))
        nothingWritten root

let F3_B006_CommandsFollowTheHostAndStaleOnesAreReported () =
    // A host without commands_output projects no command.
    let root = checkout ()
    write root "commands/convene.md" (command "convene" "[alpha]" "[]" commandBody)
    apply root (plan root) |> ignore
    if exists root ".claude/commands/convene.md" || exists root ".agents/skills/convene/SKILL.md" then
        failwith "a host without commands_output projected a command"
    // A command output with no source is stale.
    let root2 = commandsCheckout ()
    apply root2 (plan root2) |> ignore
    write root2 ".claude/commands/gone.md" "no source"
    equal [ Stale ".claude/commands/gone.md" ] (differences root2 (plan root2))

let F3_B007_CommandNamedLikeSkill_Fails () =
    let root = commandsCheckout ()
    write root "commands/sdd-conventions.md" (command "sdd-conventions" "[alpha]" "[]" commandBody)
    contains "command 'sdd-conventions' has the name of skill 'sdd-conventions'" (expectFailure (fun () -> plan root))
    nothingWritten root

// ---------------------------------------------------------------- Runner

let tests : (string * (unit -> unit)) list =
    [ nameof B001_RoleAndClaudeOverlay_EmitsFrontmatterAndVerbatimBody, B001_RoleAndClaudeOverlay_EmitsFrontmatterAndVerbatimBody
      nameof B002_RoleAndCodexOverlay_EmitsTomlWithPrefixedInstructions, B002_RoleAndCodexOverlay_EmitsTomlWithPrefixedInstructions
      nameof B003_HostileBody_RoundTripsThroughToml, B003_HostileBody_RoundTripsThroughToml
      nameof B004_SkillDirectory_CopiedByteForByteToBothHosts, B004_SkillDirectory_CopiedByteForByteToBothHosts
      nameof B005_OwnedSections_WriteOwnershipTableBetweenMarkers, B005_OwnedSections_WriteOwnershipTableBetweenMarkers
      nameof B005b_Subsection_RendersUnderItsParentAndIsExemptFromTotality, B005b_Subsection_RendersUnderItsParentAndIsExemptFromTotality
      nameof B006_TwoRolesClaimOneSection_FailsNamingBoth, B006_TwoRolesClaimOneSection_FailsNamingBoth
      nameof B007_UnownedTemplateSection_FailsNamingIt, B007_UnownedTemplateSection_FailsNamingIt
      nameof B008_Check_IsSilentWhenCurrentAndNamesEveryDrift, B008_Check_IsSilentWhenCurrentAndNamesEveryDrift
      nameof B009_OverlayNamesMissingRole_Fails, B009_OverlayNamesMissingRole_Fails
      nameof B009b_RoleNameDiffersFromFileName_Fails, B009b_RoleNameDiffersFromFileName_Fails
      nameof B010_RoleFileCarriesHostKey_FailsNamingTheKey, B010_RoleFileCarriesHostKey_FailsNamingTheKey
      nameof B011_Personas_WriteRosterBetweenMarkers, B011_Personas_WriteRosterBetweenMarkers
      nameof B012_Persona_PrecedesVerbatimBodyOnBothHosts, B012_Persona_PrecedesVerbatimBodyOnBothHosts
      nameof B013_ReviewUnits_WriteRegistryBetweenMarkers, B013_ReviewUnits_WriteRegistryBetweenMarkers
      nameof B014_TwoRolesReviewOneUnit_FailsNamingBoth, B014_TwoRolesReviewOneUnit_FailsNamingBoth
      nameof B015_UnreviewedOrUndeclaredUnit_FailsNamingIt, B015_UnreviewedOrUndeclaredUnit_FailsNamingIt
      nameof C3_Outputs_AreDeterministicAcrossRuns, C3_Outputs_AreDeterministicAcrossRuns
      nameof F2_B001_EveryRoleDeclaresTheContractKeysAndNoHostKey, F2_B001_EveryRoleDeclaresTheContractKeysAndNoHostKey
      nameof F2_B002_EveryTemplateSectionHasExactlyOneOwner, F2_B002_EveryTemplateSectionHasExactlyOneOwner
      nameof F2_B003_EveryRoleBodyCarriesTheFourSections, F2_B003_EveryRoleBodyCarriesTheFourSections
      nameof F2_B005_LookForRoleCarriesReport, F2_B005_LookForRoleCarriesReport
      nameof F2_B006_EverySkillLinkResolves, F2_B006_EverySkillLinkResolves
      nameof F2_B007_PersonaRequiresNameAndVoiceAndIsUnique, F2_B007_PersonaRequiresNameAndVoiceAndIsUnique
      nameof F2_B008_ReviewingRoleCarriesLookForAndReport, F2_B008_ReviewingRoleCarriesLookForAndReport
      nameof F3_B001_CommandProjectsToClaudeCommandFile, F3_B001_CommandProjectsToClaudeCommandFile
      nameof F3_B002_CommandProjectsToCodexSkill, F3_B002_CommandProjectsToCodexSkill
      nameof F3_B003_CommandNamingMissingRole_FailsNamingBoth, F3_B003_CommandNamingMissingRole_FailsNamingBoth
      nameof F3_B004_CommandNamingMissingSkill_FailsNamingBoth, F3_B004_CommandNamingMissingSkill_FailsNamingBoth
      nameof F3_B005_MalformedCommand_Fails, F3_B005_MalformedCommand_Fails
      nameof F3_B006_CommandsFollowTheHostAndStaleOnesAreReported, F3_B006_CommandsFollowTheHostAndStaleOnesAreReported
      nameof F3_B007_CommandNamedLikeSkill_Fails, F3_B007_CommandNamedLikeSkill_Fails ]

let mutable failures = 0
for name, test in tests do
    try
        test ()
        printfn "  ok   %s" name
    with ex ->
        failures <- failures + 1
        printfn "  FAIL %s\n       %s" name (ex.Message.Replace("\n", "\n       "))

let scratch = Path.Combine(Path.GetTempPath(), "projection-tests")
if Directory.Exists scratch then Directory.Delete(scratch, true)

printfn "%d tests, %d failed" tests.Length failures
exit failures
