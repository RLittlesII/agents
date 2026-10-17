// Tests for the projector. Names cite the claims they cover:
//   scripts/.spec/README.md  (0001-F1 Projection)    B001_..B018_
//   agents/.spec/README.md   (0001-F2 Role contract) F2_B001_..F2_B040_
//   commands/.spec/README.md (0001-F3 Commands)      F3_B001_..F3_B007_
//   skills/.spec/README.md   (0001-F4 Skill boundary) F4_B001_..F4_B003_
//
//   dotnet fsi scripts/project.tests.fsx        exit code = number of failures

#load "Projection.fsx"

open System
open System.IO
open System.Text
open Projection

let repoRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))
let scratch = Path.Combine(Path.GetTempPath(), "projection-tests", Guid.NewGuid().ToString("N"))

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
description: Synthetic skill fixture.
---

# SDD conventions

## Section ownership

<!-- BEGIN GENERATED: section-ownership -->
<!-- END GENERATED: section-ownership -->

The `.feature` file belongs to `spec-author`.

## Review passes

<!-- BEGIN GENERATED: review-passes -->
<!-- END GENERATED: review-passes -->

## Never add

- An invalid example.
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

let completeRoleBody (body: string) =
    let required = [ "Owns"; "Read first"; "Refuse" ]
    let missing = required |> List.filter (fun heading -> not (body.Contains ("## " + heading)))
    if missing.IsEmpty && (body.Contains "## Produce" || body.Contains "## Look for") then body
    else body + (missing |> List.map (fun heading -> "\n## " + heading + "\n\nFixture section.\n") |> String.concat "") +
         (if body.Contains "## Produce" || body.Contains "## Look for" then "" else "\n## Produce\n\nFixture section.\n")

let role name (owned: string) (body: string) =
    sprintf "---\nname: %s\ndescription: Does %s things.\nversion: 0.1.0\nowned_spec_sections: %s\n---\n%s" name name owned (completeRoleBody body)

/// A role with a persona block; `persona` is the YAML under `persona:`.
let personaRole name (owned: string) (persona: string) (body: string) =
    sprintf "---\nname: %s\ndescription: Does %s things.\nversion: 0.1.0\nowned_spec_sections: %s\npersona:\n%s\n---\n%s" name name owned persona (completeRoleBody body)

let alphaBody = completeRoleBody "\n# Alpha\n\nReads [`sdd-conventions`](../skills/sdd-conventions/SKILL.md).\n"
let betaBody = completeRoleBody "\n# Beta\n\nOwns nothing.\n"

let write (root: string) (rel: string) (text: string) =
    let path = Path.Combine(root, rel)
    Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
    File.WriteAllText(path, text, UTF8Encoding(false))

/// A synthetic checkout: two roles, both hosts, one skill, the companion, an
/// empty persona roster and a template with three numbered sections plus Tasks
/// and Scoring.
let checkout () =
    let root = Path.Combine(scratch, Guid.NewGuid().ToString("N"))
    Directory.CreateDirectory root |> ignore
    for schema in [ "agent"; "skill" ] do
        let source = Path.Combine(repoRoot, "schemas", schema + ".schema.json")
        write root ("schemas/" + schema + ".schema.json") (File.ReadAllText source)
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
    let body = completeRoleBody "\n# Alpha\n\nQuotes \"here\", a back\\slash, a tab\there, § and émoji 🟡 🟢.\n\n```\nraw \"\"\" triple\n```\n"
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
    write root "skills/other/SKILL.md" "---\nname: other\ndescription: Synthetic skill.\n---\n\n# Other ✓\r\nwith CRLF kept\n\n## Never add\n\n- Invalid input.\n"
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

let B024_HostOverlaysRemainAuthoritative () =
    for key in [ "model: opus"; "effort: high"; "tools: [Read]"; "model_reasoning_effort: high" ] do
        let root = checkout ()
        write root "agents/beta.md" (sprintf "---\nname: beta\ndescription: Beta.\nversion: 0.1.0\nowned_spec_sections: [\"3\"]\n%s\n---\n%s" key betaBody)
        let outputs = (plan root).Outputs
        let generated = Encoding.UTF8.GetString outputs.[".claude/agents/beta.md"]
        contains "model: inherit" generated
        contains "effort: medium" generated
        nothingWritten root

let B017_Personas_WriteRosterBetweenMarkers () =
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

let B018_Persona_PrecedesVerbatimBodyOnBothHosts () =
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

let B017_MixedPersonas_SortNamedThenUnnamedRoles () =
    let root = checkout ()
    write root "agents/alpha.md" (personaRole "alpha" "[\"1\", \"2\", \"Tasks\"]" "  voice: Terse." alphaBody)
    write root "agents/beta.md" (personaRole "beta" "[\"3\"]" "  voice: Calm." betaBody)
    write root "agents/gamma.md" (personaRole "gamma" "[]" "  name: Zed\n  voice: Direct." betaBody)
    write root "agents/delta.md" (personaRole "delta" "[]" "  name: Abe\n  voice: Brief." betaBody)
    write root "agents/epsilon.md" (role "epsilon" "[]" betaBody)
    apply root (plan root) |> ignore
    let text = read root "agents/PERSONAS.md"
    let expected =
        String.concat "\n"
            [ "<!-- BEGIN GENERATED: personas -->"
              "| Persona | Role                | Voice   |"
              "| ------- | ------------------- | ------- |"
              "| Abe     | [`delta`](delta.md) | Brief.  |"
              "| Zed     | [`gamma`](gamma.md) | Direct. |"
              "| —       | [`alpha`](alpha.md) | Terse.  |"
              "| —       | [`beta`](beta.md)   | Calm.   |"
              "<!-- END GENERATED: personas -->" ]
    contains expected text
    equal text (Encoding.UTF8.GetString (plan root).Outputs.["agents/PERSONAS.md"])

let B018_UnnamedPersona_PreservesVoiceOnBothHosts () =
    let root = checkout ()
    write root "agents/alpha.md" (personaRole "alpha" "[\"1\", \"2\", \"Tasks\"]" "  voice: Terse." alphaBody)
    apply root (plan root) |> ignore
    let claude = read root ".claude/agents/alpha.md"
    equal ("---\nname: alpha\ndescription: \"Does alpha things.\"\nmodel: opus\neffort: high\nskills: [sdd-conventions]\n---\n\nVoice: Terse.\n" + alphaBody) claude
    let instructions name =
        let codex = read root (sprintf ".codex/agents/%s.toml" name)
        let line = codex.Split('\n') |> Array.find (fun l -> l.StartsWith "developer_instructions = ")
        decodeToml (line.Substring "developer_instructions = ".Length)
    let prefix name = sprintf "Read AGENTS.md first. The authoritative role source is agents/%s.md. Resolve its relative links against that path in the repository." name
    equal (prefix "alpha" + "\n\nVoice: Terse.\n" + alphaBody) (instructions "alpha")
    equal ("---\nname: beta\ndescription: \"Does beta things.\"\nmodel: inherit\neffort: medium\n---\n" + betaBody) (read root ".claude/agents/beta.md")
    equal (prefix "beta" + "\n" + betaBody) (instructions "beta")

/// Two review units, each declaring what it reviews, when it runs and what it
/// never does.
let reviewUnits =
    String.concat "\n"
        [ "review_units:"
          "  design:"
          "    reviews: a design"
          "    runs_when: before code"
          "    never: edits"
          "  diff:"
          "    reviews: a diff"
          "    runs_when: before merge"
          "    never: rewrites"
          "---"
          "" ]

let reviewBody title = sprintf "\n# %s\n\n## Look for\n\n- Drift.\n\n## Report\n\nOne finding per line.\n" title

/// The synthetic checkout with two review units, alpha reviewing the diff and
/// beta the design.
let reviewsCheckout () =
    let root = checkout ()
    write root "agents/README.md" (readme.Replace("---\n", "") |> fun front -> "---\n" + front + reviewUnits)
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
              "| Order | Unit   | Reviews  | Runs when    | Never    | Reviewer |"
              "| ----- | ------ | -------- | ------------ | -------- | -------- |"
              "| 1     | design | a design | before code  | edits    | `beta`   |"
              "| 2     | diff   | a diff   | before merge | rewrites | `alpha`  |"
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

let B016_StampNamingANonOwner_FailsNamingTheSectionAndBothRoles () =
    let root = checkout ()
    let spec stamp1 =
        String.concat "\n"
            [ "## 1. Business Goal"; ""; sprintf "<!-- last written by: %s, 2026-10-14 -->" stamp1; ""
              "```markdown"; "## 3. Acceptance Criteria"; "<!-- last written by: alpha, 2026-10-14 -->"; "```"; ""
              "## Scoring"; ""; "<!-- last written by: beta, 2026-10-14 -->"; "" ]
    // The owner's stamp passes; a fenced example and a section without a single owner are not checked.
    write root "area/.spec/README.md" (spec "alpha")
    plan root |> ignore
    write root "area/.spec/README.md" (spec "beta")
    let message = expectFailure (fun () -> plan root)
    contains "area/.spec/README.md: section '1. Business Goal' is stamped by 'beta', but 'alpha' owns it" message
    nothingWritten root

let B016_DesignSectionsRejectAnImplementationStamp () =
    // Given design sections owned by alpha, when a template-derived copy names
    // an implementation writer, then planning rejects each section before writes.
    for number, title in [ "6", "Concern Separation"; "7", "Technical Design" ] do
        let root = checkout ()
        let section = number + ". " + title
        write root ".specht/templates/feature.md" (template.Replace("## Tasks", "## " + section + "\n\n## Tasks"))
        write root "agents/alpha.md" (role "alpha" (sprintf "[\"1\", \"2\", \"%s\", \"Tasks\"]" number) alphaBody)
        let document writer = sprintf "## %s\n\n<!-- last written by: %s, 2026-10-17 -->\n" section writer
        write root "area/.spec/README.md" (document "alpha")
        plan root |> ignore
        write root "area/.spec/README.md" (document "implementer")
        contains (sprintf "area/.spec/README.md: section '%s' is stamped by 'implementer', but 'alpha' owns it" section)
            (expectFailure (fun () -> plan root))
        nothingWritten root

let B016_SignOffRejectsAReviewersStamp () =
    // Given sign-off has a recording owner, when the reviewing role signs it,
    // then projection cannot publish that ownership violation.
    let root = checkout ()
    write root ".specht/templates/feature.md" (template.Replace("## Tasks", "## 12. Sign-off\n\n## Tasks"))
    write root "agents/beta.md" (role "beta" "[\"3\", \"12\"]" betaBody)
    write root "area/.spec/README.md" "## 12. Sign-off\n\n<!-- last written by: beta, 2026-10-17 -->\n"
    plan root |> ignore
    write root "area/.spec/README.md" "## 12. Sign-off\n\n<!-- last written by: spec-reviewer, 2026-10-17 -->\n"
    contains "area/.spec/README.md: section '12. Sign-off' is stamped by 'spec-reviewer', but 'beta' owns it"
        (expectFailure (fun () -> plan root))
    nothingWritten root

let B016_TildeFencedExampleDoesNotCreateAnOwnershipViolation () =
    // Given a valid stamp and a non-owner stamp inside a Markdown example,
    // when the example uses a tilde fence, then planning still succeeds.
    let root = checkout ()
    write root "area/.spec/README.md"
        "## 1. Business Goal\n\n<!-- last written by: alpha, 2026-10-17 -->\n\n~~~markdown\n## 3. Acceptance Criteria\n<!-- last written by: alpha, 2026-10-17 -->\n~~~\n"
    plan root |> ignore
    nothingWritten root

let B016_ShorterBackticksDoNotEndAFencedExample () =
    // Given a four-backtick example containing three literal backticks,
    // when an example stamp names a non-owner, then it remains exempt.
    let root = checkout ()
    write root "area/.spec/README.md"
        "## 1. Business Goal\n\n<!-- last written by: alpha, 2026-10-17 -->\n\n````markdown\n```\n## 3. Acceptance Criteria\n<!-- last written by: alpha, 2026-10-17 -->\n````\n"
    plan root |> ignore
    nothingWritten root

let B016_RealViolationAfterAFencedExampleIsStillRejected () =
    // Given the same nested example followed by a real ownership violation,
    // when planning runs, then the example cannot hide the following section.
    let root = checkout ()
    write root "area/.spec/README.md"
        "````markdown\n```\nAn example of a shorter fence.\n````\n\n## 1. Business Goal\n\n<!-- last written by: beta, 2026-10-17 -->\n"
    contains "area/.spec/README.md: section '1. Business Goal' is stamped by 'beta', but 'alpha' owns it"
        (expectFailure (fun () -> plan root))
    nothingWritten root

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
    [ "adversary"; "architect"; "auditor"; "benchmarker"; "bug-finder"; "build"; "database"
      "document-writer"; "domain"; "implementer"; "observability"; "organizer"; "release"
      "spec-author"; "spec-reviewer"; "test-writer"; "tester"; "ui"; "ux" ]

let F2_B041_EverySourceRoleDeclaresRepositoryKeys () =
    // The loader requires the repository role keys and numeric version.
    // Portable description and optional-host-setting validation is schema-owned.
    let roles = loadRoles repoRoot
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
    equal 16 rows.Length

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

let F2_B040_PersonaRequiresVoiceAndOptionalUniqueName () =
    // The real roster: every persona loads and no name is shared.
    let roles = loadRoles repoRoot
    validatePersonas roles
    // Multiple unnamed personas are valid and retain their voices.
    let unnamed = checkout ()
    write unnamed "agents/alpha.md" (personaRole "alpha" "[\"1\", \"2\", \"Tasks\"]" "  voice: Terse." alphaBody)
    write unnamed "agents/beta.md" (personaRole "beta" "[\"3\"]" "  voice: Calm." betaBody)
    plan unnamed |> ignore
    equal [ "Terse."; "Calm." ] (loadRoles unnamed |> List.choose (fun r -> r.Persona |> Option.map (fun p -> p.Voice)))
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

let F2_B040_InvalidSuppliedNameFails () =
    for name in [ ""; "\"   \""; "\"Two\\nLines\""; "\"Two\\rLines\""; "[Abe]" ] do
        let root = checkout ()
        write root "agents/beta.md" (personaRole "beta" "[\"3\"]" ("  name: " + name + "\n  voice: Calm.") betaBody)
        contains "agents/beta.md: persona.name" (expectFailure (fun () -> plan root))
        nothingWritten root

let F2_B040_InvalidVoiceFails () =
    for voice in [ ""; "\"   \""; "\"Two\\nLines\""; "\"Two\\rLines\""; "[Calm]" ] do
        let root = checkout ()
        write root "agents/beta.md" (personaRole "beta" "[\"3\"]" ("  voice: " + voice) betaBody)
        contains "agents/beta.md: persona.voice" (expectFailure (fun () -> plan root))
        nothingWritten root

let F2_B008_ReviewingRoleCarriesLookForAndReport () =
    // The real roster: the four review passes on three reviewing bodies.
    let reviewing = loadRoles repoRoot |> List.filter (fun r -> not r.Reviews.IsEmpty) |> List.map (fun r -> r.Name, r.Reviews)
    equal [ "adversary", [ "surface" ]; "auditor", [ "repository" ]; "spec-reviewer", [ "design"; "diff" ] ] reviewing
    // A role declaring a unit without the reviewing headings fails.
    let root = checkout ()
    write root "agents/beta.md" (role "beta" "[\"3\"]\nreviews: [diff]" betaBody)
    contains "agents/beta.md: reviews diff but lacks ## Look for or ## Report" (expectFailure (fun () -> plan root))
    nothingWritten root

let F2_B009_ReviewUnitDeclaresTriggerAndRefusal () =
    // The real index: every unit declares all three, so the registry renders.
    for unit in loadReviewUnits repoRoot do
        if String.IsNullOrWhiteSpace unit.RunsWhen || String.IsNullOrWhiteSpace unit.Never then
            failwithf "review unit '%s' lacks a trigger or a refusal" unit.Unit
    // A unit with no `never` fails naming the unit and the key.
    let root = reviewsCheckout ()
    write root "agents/README.md" (readme.Replace("---\n", "") |> fun front -> "---\n" + front + reviewUnits.Replace("    never: edits\n", ""))
    contains "agents/README.md: review unit 'design' lacks never" (expectFailure (fun () -> plan root))
    nothingWritten root
    // A unit given as bare text fails: it states no trigger or refusal.
    let root2 = reviewsCheckout ()
    write root2 "agents/README.md" (readme.Replace("---\n", "") |> fun front -> "---\n" + front + "review_units:\n  design: a design\n  diff: a diff\n---\n")
    contains "agents/README.md: review unit 'design' must declare reviews, runs_when and never" (expectFailure (fun () -> plan root2))
    nothingWritten root2

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
              "Convene these roles as the body directs:"
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

// ---------------------------------------------------------------- 0001-F4 Skill boundary

/// The synthetic checkout plus a hyphenated role, log-reader, signing as Zed.
let lintCheckout () =
    let root = checkout ()
    write root "agents/log-reader.md" (personaRole "log-reader" "[]" "  name: Zed\n  voice: Terse." "\n# Log reader\n")
    root

let F4_B001_SkillNamingRole_FailsNamingFileLineAndRole () =
    let cases =
        [ "Ask `alpha` first.", "skills/other/SKILL.md:6: names role 'alpha'"
          "Hand it to the log-reader.", "skills/other/SKILL.md:6: names role 'log-reader'"
          "Hand it to the Log reader.", "skills/other/SKILL.md:6: names role 'log-reader'"
          "Zed signs the report.", "skills/other/SKILL.md:6: names role 'log-reader'"
          "See [alpha](../agents/alpha.md).", "skills/other/SKILL.md:6: names a path into agents/"
          "Read agents/alpha.md first.", "skills/other/SKILL.md:6: names a path into agents/" ]
    for line, expected in cases do
        let root = lintCheckout ()
        write root "skills/other/SKILL.md" ("---\nname: other\n---\n\n# Other\n" + line + "\n")
        contains expected (expectFailure (fun () -> plan root))
        nothingWritten root
    // Ordinary words are not role names: a bare one-word name, a longer
    // hyphenated word, a host directory, and a lower-case word.
    let root = lintCheckout ()
    write root "skills/other/SKILL.md" "---\nname: other\n---\n\nThe alpha release; a log-reader-like tool; .claude/agents/ holds agents; zed.\n"
    lintSkills root (loadRoles root)

let F4_B001_UnnamedPersona_HasNoNamePattern () =
    let root = lintCheckout ()
    write root "agents/log-reader.md" (personaRole "log-reader" "[]" "  voice: Terse." "\n# Log reader\n")
    write root "skills/other/SKILL.md" "---\nname: other\ndescription: Synthetic skill.\n---\n\n# Other\n\nZed reports in a terse voice.\n\n## Never add\n\n- Invalid input.\n"
    plan root |> ignore
    write root "skills/other/SKILL.md" "---\nname: other\n---\n\nAsk the log-reader.\n"
    contains "names role 'log-reader'" (expectFailure (fun () -> plan root))
    nothingWritten root

let F4_B002_GeneratedBlockIsExempt () =
    let root = lintCheckout ()
    let text =
        "---\nname: other\n---\n\n<!-- BEGIN GENERATED: table -->\n| `alpha` | log-reader | Zed |\n<!-- END GENERATED: table -->\n\nAsk `beta`.\n"
    write root "skills/other/SKILL.md" text
    // Only the line after the block is reported, at its own line number.
    let message = expectFailure (fun () -> plan root)
    contains "skills/other/SKILL.md:9: names role 'beta'" message
    if message.Contains "'alpha'" || message.Contains "'log-reader'" then failwithf "a generated block was linted:\n%s" message
    write root "skills/other/SKILL.md" (text.Replace("Ask `beta`.", "Ask the owner."))
    lintSkills root (loadRoles root)

let F4_B003_RepositorySkillsNameNoRole () =
    lintSkills repoRoot (loadRoles repoRoot)

// ---------------------------------------------------------------- 0001-F2 Migration (Kevin)

let F2_B033_NineteenRolesHaveExplicitMappingsOnBothHosts () =
    // Given delivered sources; when loaded; then their exact roster and mappings agree.
    equal roster (loadRoles repoRoot |> List.map (fun r -> r.Name))
    for id in [ "claude"; "codex" ] do
        equal roster ((loadHost repoRoot id).Roles |> List.map fst |> List.sort)

let F2_B033_NewSpecialistsProjectSourceBodiesAndAgreedProfiles () =
    // Given new specialists; when planned; then source bytes and judgment profiles survive.
    let roles = loadRoles repoRoot
    for name in [ "database"; "observability" ] do
        if not (roles |> List.exists (fun r -> r.Name = name)) then failwithf "agreed specialist '%s' is absent" name
    let outputs = (plan repoRoot).Outputs
    for name in [ "database"; "observability" ] do
        let r = roles |> List.find (fun r -> r.Name = name)
        equal (if name = "database" then [] else [ "7/Observability" ]) r.OwnedSections
        let ch = overlay (loadHost repoRoot "claude") r |> Map.ofList
        let cx = overlay (loadHost repoRoot "codex") r |> Map.ofList
        equal (Scalar (if name = "database" then "opus" else "sonnet")) ch.["model"]
        equal (Scalar (if name = "database" then "high" else "medium")) ch.["effort"]
        equal (if name = "database" then List ["sdd-conventions"; "coding-conventions"; "clarify-requirements"] else List ["sdd-conventions"]) ch.["skills"]
        if Map.containsKey "tools" ch then failwithf "%s restricts assigned writes" name
        equal ((loadHost repoRoot "codex").Defaults |> List.find (fun (k, _) -> k = "model") |> snd) cx.["model"]
        equal (Scalar (if name = "database" then "high" else "medium")) cx.["model_reasoning_effort"]
        let claude = Encoding.UTF8.GetString outputs.[sprintf ".claude/agents/%s.md" name]
        let codex = Encoding.UTF8.GetString outputs.[sprintf ".codex/agents/%s.toml" name]
        let line = codex.Split('\n') |> Array.find (fun l -> l.StartsWith "developer_instructions = ")
        let decoded = decodeToml (line.Substring "developer_instructions = ".Length)
        if not (claude.EndsWith r.Body && decoded.EndsWith r.Body) then failwithf "%s body changed in projection" name

let F2_B034_FourReviewUnitsAndExistingSectionOwnersStayUnchanged () =
    // Given retained declarations; when computed; then consolidation preserves existing owners and adds only the optional nested owner.
    let roles = loadRoles repoRoot
    equal [ "design", "spec-reviewer"; "diff", "spec-reviewer"; "surface", "adversary"; "repository", "auditor" ]
        (reviewRows roles (loadReviewUnits repoRoot) |> List.map (fun (u, r) -> u.Unit, r))
    let expected = [ "1", "spec-author"; "2", "spec-author"; "3", "spec-author"; "4", "spec-author"; "5", "spec-author"
                     "6", "architect"; "7", "architect"; "7/Observability", "observability"; "8", "test-writer"; "8/Performance", "benchmarker"
                     "9", "test-writer"; "10", "spec-author"; "12", "spec-author"; "Scoring", "organizer"; "Tasks", "organizer" ]
    equal (List.sort expected) (roles |> List.collect (fun r -> r.OwnedSections |> List.map (fun s -> s, r.Name)) |> List.sort)
    ownershipRows roles (loadUnowned repoRoot) (loadTemplateSections repoRoot) |> ignore

let F2_B034_ActiveConveningBindingsResolveToRetainedRoles () =
    // Given active contracts, commands and overlays; when resolved; then no retired identity remains bound.
    let roles = loadRoles repoRoot
    let retired = set [ "analyst"; "pair-programmer"; "yak-shaver"; "bug-fixer"; "critic"; "log-parser"; "metric-nerd" ]
    for c in loadCommands repoRoot do
        for name in c.Roles do
            if retired.Contains name then failwithf "%s still convenes retired role '%s'" c.Source name
    validateCommands roles (Directory.GetDirectories(Path.Combine(repoRoot, "skills")) |> Array.map Path.GetFileName |> List.ofArray) (loadCommands repoRoot)
    for id in [ "claude"; "codex" ] do validateHostRoles (loadHost repoRoot id) roles
    for r in roles do
        for m in System.Text.RegularExpressions.Regex.Matches(r.Body, @"\]\((?:\.\./agents/|\./)?([a-z-]+)\.md(?:#[^)]*)?\)") do
            let name = m.Groups.[1].Value
            if retired.Contains name then failwithf "%s links retired contract '%s'" r.Source name

/// Invented role contracts, shared by migration and rejected-plan cases.
let rosterCheckout names =
    let root = commandsCheckout ()
    write root "skills/sdd-conventions/SKILL.md" (companion.Replace("The `.feature` file belongs to `spec-author`.", "The scenario file belongs to the agreement owner."))
    for name in [ "alpha"; "beta" ] do File.Delete(Path.Combine(root, "agents/" + name + ".md"))
    for name in names do
        let owned = if name = "spec-author" then "[\"1\", \"2\", \"Tasks\"]" elif name = "architect" then "[\"3\"]" else "[]"
        write root ("agents/" + name + ".md") (role name owned ("\n# " + name + "\n"))
    for id, source in [ "claude", claudeHost; "codex", codexHost ] do
        let head = source.Substring(0, source.IndexOf "roles:")
        let commands = if id = "claude" then "commands_output: .claude/commands/{name}.md\n" else "commands_output: .agents/skills/{name}/SKILL.md\n"
        write root ("hosts/" + id + ".yaml") (head + commands + "roles:\n" + (names |> List.map (sprintf "  %s: {}\n") |> String.concat ""))
    write root "commands/convene.md" (command "convene" "[spec-author]" "[]" commandBody)
    root

let runProject root check =
    let start = System.Diagnostics.ProcessStartInfo("dotnet")
    start.WorkingDirectory <- repoRoot
    start.UseShellExecute <- false
    start.RedirectStandardOutput <- true
    start.RedirectStandardError <- true
    for arg in [ "fsi"; Path.Combine(repoRoot, "scripts/project.fsx"); "--root"; root ] do start.ArgumentList.Add arg
    if check then start.ArgumentList.Add "--check"
    use proc = System.Diagnostics.Process.Start start
    let stdout = proc.StandardOutput.ReadToEndAsync()
    let stderr = proc.StandardError.ReadToEndAsync()
    proc.WaitForExit()
    proc.ExitCode, stdout.Result + stderr.Result

let F2_B034_MigrationRemovesThreeRolesAddsTwoAndPassesPublicCheck () =
    // Given the projected twenty-role baseline, when migrated, then missing/stale diagnostics precede a clean apply.
    let retired = [ "analyst"; "pair-programmer"; "yak-shaver" ]
    let historicalRoster = roster |> List.map (fun name -> if name = "observability" then "metric-nerd" else name)
    let root = rosterCheckout ((historicalRoster |> List.except [ "database"; "metric-nerd" ]) @ retired)
    equal 20 (loadRoles root).Length
    apply root (plan root) |> ignore
    for name in retired do File.Delete(Path.Combine(root, "agents/" + name + ".md"))
    for name in [ "database"; "metric-nerd" ] do write root ("agents/" + name + ".md") (role name "[]" ("\n# " + name + "\n"))
    for id in [ "claude"; "codex" ] do
        let current = read root ("hosts/" + id + ".yaml")
        write root ("hosts/" + id + ".yaml") (current.Substring(0, current.IndexOf "roles:") + "roles:\n" + (historicalRoster |> List.map (sprintf "  %s: {}\n") |> String.concat ""))
    let expected =
        [ for prefix, ext in [ ".claude/agents/", ".md"; ".codex/agents/", ".toml" ] do
            for name in retired do yield Stale (prefix + name + ext)
            for name in [ "database"; "metric-nerd" ] do yield Missing (prefix + name + ext) ] |> List.sort
    equal expected (differences root (plan root) |> List.sort)
    let code, report = runProject root true
    equal 1 code
    for diff in expected do contains (describe diff) report
    equal expected (differences root (plan root) |> List.sort)
    apply root (plan root) |> ignore
    equal [] (differences root (plan root))
    for dir in [ ".claude/agents"; ".codex/agents" ] do equal 19 (Directory.GetFiles(Path.Combine(root, dir)).Length)
    equal 0 (fst (runProject root true))

let F2_B034_RetiredBindingsFailBeforeWriting () =
    // Given absent retired identities, when commands or overlays bind them, then validation rejects without output.
    for name in [ "analyst"; "pair-programmer"; "yak-shaver"; "bug-fixer"; "critic"; "log-parser" ] do
        let root = rosterCheckout roster
        write root "commands/convene.md" (command "convene" ("[" + name + "]") "[]" commandBody)
        contains ("names role '" + name + "'") (expectFailure (fun () -> plan root))
        nothingWritten root
        let root2 = rosterCheckout roster
        write root2 "hosts/codex.yaml" (read root2 "hosts/codex.yaml" + "  " + name + ": {}\n")
        contains ("role '" + name + "' has no file") (expectFailure (fun () -> plan root2))
        nothingWritten root2
    let root = rosterCheckout roster
    write root "commands/convene.md" (command "convene" "[analyst]" "[]" commandBody)
    let code, report = runProject root false
    equal 2 code
    contains "names role 'analyst'" report
    nothingWritten root

let F2_B035_HistoricalEvidenceAndExistingOutputsSurviveRejectedPlans () =
    // Given invented old identities and claim anchors; when projected or rejected; then evidence bytes remain.
    let root = rosterCheckout roster
    let history = "---\ntitle: Historical evidence\n---\ncritic approved B-020; analyst recorded B-015.\r\n"
    write root "agents/.spec/decisions/history.md" history
    apply root (plan root) |> ignore
    let before = (plan root).Outputs |> Map.toList |> List.map (fun (rel, _) -> rel, File.ReadAllBytes(Path.Combine(root, rel)))
    apply root (plan root) |> ignore
    equal (Encoding.UTF8.GetBytes history) (File.ReadAllBytes(Path.Combine(root, "agents/.spec/decisions/history.md")))
    write root "commands/convene.md" (command "convene" "[critic]" "[]" commandBody)
    expectFailure (fun () -> plan root) |> ignore
    for rel, bytes in before do equal bytes (File.ReadAllBytes(Path.Combine(root, rel)))
    equal history (read root "agents/.spec/decisions/history.md")

// ---------------------------------------------------------------- 0001-F2 Optional observability (Kevin)

/// Reuse the delivered declaration so a missing/incorrect ownership cannot be hidden in the fixture.
let observabilityCheckout () =
    let specialist = loadRoles repoRoot |> List.tryFind (fun r -> r.Name = "observability")
    let specialist = specialist |> Option.defaultWith (fun () -> failwith "agreed observability contract is absent from delivered roles")
    let root = checkout ()
    write root ".specht/templates/feature.md" (template + "\n## 7. Technical Design\n\n## 8. Testing Strategy\n")
    write root "agents/architect.md" (role "architect" "[\"7\"]" "\n# Architect\n")
    write root "agents/test-writer.md" (role "test-writer" "[\"8\"]" "\n# Test writer\n")
    write root "agents/benchmarker.md" (role "benchmarker" "[\"8/Performance\"]" "\n# Benchmarker\n")
    let owned = specialist.OwnedSections |> List.map (sprintf "\"%s\"") |> String.concat ", "
    write root "agents/observability.md" (role specialist.Name ("[" + owned + "]") "\n# Observability\n")
    root

let F2_B037_OptionalObservabilityAcceptsItsDeclaredOwnerStamp () =
    // Given a present subsection and its delivered ownership, when planned, then its stamp is accepted.
    let root = observabilityCheckout ()
    write root "area/.spec/README.md" "## 7. Technical Design\n<!-- last written by: architect, 2026-10-16 -->\n### Observability\n<!-- last written by: observability, 2026-10-16 -->\n"
    let rows = ownershipRows (loadRoles root) (loadUnowned root) (loadTemplateSections root)
    equal (Some ("7. › `### Observability`", "`observability`")) (rows |> List.tryFind (fun (h, _) -> h = "7. › `### Observability`"))
    plan root |> ignore
    nothingWritten root

let F2_B037_AbsentObservabilityNeedsNoSubsectionOrTemplateEdit () =
    // Given no subsection, when projected, then ownership remains valid and no subsection is created.
    let root = observabilityCheckout ()
    let spec = "## 7. Technical Design\n<!-- last written by: architect, 2026-10-16 -->\n"
    write root "area/.spec/README.md" spec
    let beforeTemplate = read root ".specht/templates/feature.md"
    apply root (plan root) |> ignore
    equal spec (read root "area/.spec/README.md")
    equal beforeTemplate (read root ".specht/templates/feature.md")
    equal [] (differences root (plan root))

let F2_B037_ArchitectCannotStampObservabilityBeforeWrites () =
    // Given the parent author's stamp on the specialist subsection, when planned, then validation names its real owner.
    let root = observabilityCheckout ()
    write root "area/.spec/README.md" "## 7. Technical Design\n### Observability\n<!-- last written by: architect, 2026-10-16 -->\n"
    let message = expectFailure (fun () -> plan root)
    contains "section '7. › `### Observability`' is stamped by 'architect', but 'observability' owns it" message
    nothingWritten root

let F2_B037_SiblingDesignAndPerformanceKeepTheirExistingOwners () =
    // Given a sibling design subsection and Performance, when stamps are checked, then parent/performance ownership still applies.
    let root = observabilityCheckout ()
    let spec = "## 7. Technical Design\n### Storage\n<!-- last written by: architect, 2026-10-16 -->\n## 8. Testing Strategy\n### Performance\n<!-- last written by: benchmarker, 2026-10-16 -->\n"
    write root "area/.spec/README.md" spec
    plan root |> ignore
    write root "area/.spec/README.md" (spec.Replace("last written by: architect", "last written by: observability"))
    contains "section '7. Technical Design' is stamped by 'observability', but 'architect' owns it" (expectFailure (fun () -> plan root))
    nothingWritten root
    write root "area/.spec/README.md" (spec.Replace("last written by: benchmarker", "last written by: observability"))
    contains "section '8. › `### Performance`' is stamped by 'observability', but 'benchmarker' owns it" (expectFailure (fun () -> plan root))
    nothingWritten root

let F2_B039_RenameKeepsNineteenRolesMigratesBindingsAndPreservesHistory () =
    // Given nineteen projected roles and recorded old identities, when renamed, then outputs/bindings migrate and history remains.
    let root = observabilityCheckout ()
    let specialist = loadRoles root |> List.find (fun r -> r.Name = "observability")
    let baseline = roster |> List.map (fun name -> if name = "observability" then "metric-nerd" else name)
    let root = rosterCheckout baseline
    write root ".specht/templates/feature.md" (template + "\n## 7. Technical Design\n")
    write root "agents/architect.md" (role "architect" "[\"3\", \"7\"]" "\n# Architect\n")
    write root "commands/convene.md" (command "convene" "[metric-nerd]" "[]" commandBody)
    let history = "---\ntitle: Historical review\n---\nmetric-nerd supplied B-024 and B-035 evidence.\r\n"
    write root "agents/.spec/decisions/history.md" history
    apply root (plan root) |> ignore
    equal 19 (loadRoles root).Length
    File.Delete(Path.Combine(root, "agents/metric-nerd.md"))
    let owned = specialist.OwnedSections |> List.map (sprintf "\"%s\"") |> String.concat ", "
    write root "agents/observability.md" (role "observability" ("[" + owned + "]") "\n# Observability\n")
    write root "commands/convene.md" (command "convene" "[observability]" "[]" commandBody)
    for id in [ "claude"; "codex" ] do write root ("hosts/" + id + ".yaml") ((read root ("hosts/" + id + ".yaml")).Replace("  metric-nerd:", "  observability:"))
    let expected = [ Stale ".claude/agents/metric-nerd.md"; Stale ".codex/agents/metric-nerd.toml"; Missing ".claude/agents/observability.md"; Missing ".codex/agents/observability.toml" ] |> List.sort
    let rolesOnly = differences root (plan root) |> List.filter (fun diff -> let rel = match diff with Changed p | Missing p | Stale p -> p in rel.StartsWith ".claude/agents/" || rel.StartsWith ".codex/agents/") |> List.sort
    equal expected rolesOnly
    let before = differences root (plan root)
    let code, report = runProject root true
    equal 1 code
    for diff in expected do contains (describe diff) report
    equal before (differences root (plan root))
    apply root (plan root) |> ignore
    equal 19 (loadRoles root).Length
    for dir in [ ".claude/agents"; ".codex/agents" ] do equal 19 (Directory.GetFiles(Path.Combine(root, dir)).Length)
    contains ".claude/agents/observability.md" (read root ".claude/commands/convene.md")
    contains ".codex/agents/observability.toml" (read root ".agents/skills/convene/SKILL.md")
    equal (Encoding.UTF8.GetBytes history) (File.ReadAllBytes(Path.Combine(root, "agents/.spec/decisions/history.md")))
    equal [] (differences root (plan root))
    equal 0 (fst (runProject root true))

let F2_B039_FormerOperationalBindingsFailWithoutChangingExistingOutputs () =
    // Given a current roster, when an old command or host binding returns, then rejection preserves output/history bytes.
    let root = rosterCheckout roster
    let history = "metric-nerd reviewed B-024.\r\n"
    write root "agents/.spec/decisions/history.md" history
    apply root (plan root) |> ignore
    let before = (plan root).Outputs |> Map.toList |> List.map (fun (rel, _) -> rel, File.ReadAllBytes(Path.Combine(root, rel)))
    write root "commands/convene.md" (command "convene" "[metric-nerd]" "[]" commandBody)
    contains "names role 'metric-nerd'" (expectFailure (fun () -> plan root))
    for rel, bytes in before do equal bytes (File.ReadAllBytes(Path.Combine(root, rel)))
    write root "commands/convene.md" (command "convene" "[observability]" "[]" commandBody)
    write root "hosts/codex.yaml" (read root "hosts/codex.yaml" + "  metric-nerd: {}\n")
    contains "role 'metric-nerd' has no file" (expectFailure (fun () -> plan root))
    for rel, bytes in before do equal bytes (File.ReadAllBytes(Path.Combine(root, rel)))
    equal history (read root "agents/.spec/decisions/history.md")


// ---------------------------------------------------------------- Schema enforcement

let B019_AgentSchemaRejectsInvalidFrontmatter () =
    let root = checkout ()
    let original = read root "agents/beta.md"
    for extra in ["metadata:\n  author: false\n"; "unexpected: value\n"] do
        write root "agents/beta.md" (original.Replace("version: 0.1.0\n", "version: 0.1.0\n" + extra))
        let error = expectFailure (fun () -> plan root)
        contains "agents/beta.md" error
        contains "schema validation failed" error
        nothingWritten root
    write root "agents/beta.md" (original.Replace("description: Does beta things.", "description: >\n  Folded description\n  on two lines."))
    plan root |> ignore

let B019_PortableFrontmatterAcceptsUserExamples () =
    for kind, json in [ "agent", """{"name":"mr-anderson","description":"Senior implementation specialist.","model":"claude-sonnet-5","effort":"low","metadata":{"author":"https://github.com/rlittlesii","technology":"infrastructure","complexity":"senior","version":"1.2.0","triggers":"skill-invocation"}}"""; "skill", """{"name":"adapter","description":"Adapter guidance.\nWrap an incompatible interface.","metadata":{"author":"https://github.com/rlittlesii","technology":"cross-platform","complexity":"mid","version":"1.0.0","domain":"engineering","triggers":"skill-invocation","role":"specialist","scope":"implementation","output-format":"markdown","related-skills":"design-laws, taxonomy"}}""" ] do
        use schemaDocument = System.Text.Json.JsonDocument.Parse(read repoRoot ("schemas/" + kind + ".schema.json"))
        let schema = global.Json.Schema.JsonSchema.Build(schemaDocument.RootElement, global.Json.Schema.BuildOptions(SchemaRegistry = global.Json.Schema.SchemaRegistry()))
        use instance = System.Text.Json.JsonDocument.Parse(json)
        equal true (schema.Evaluate(instance.RootElement).IsValid)

let B019_LiteralDescriptionRoundTripsThroughBothHosts () =
    let root = checkout ()
    let path = "agents/beta.md"
    write root path ((read root path).Replace("description: Does beta things.", "description: |\n  First line.\n  Second line."))
    let expected = (loadRoles root |> List.find (fun role -> role.Name = "beta")).Description
    let outputs = (plan root).Outputs
    let claude = Encoding.UTF8.GetString outputs.[".claude/agents/beta.md"]
    let pairs, _ = splitFrontmatter "projected beta" claude
    equal expected (getScalar "description" pairs)
    let codex = Encoding.UTF8.GetString outputs.[".codex/agents/beta.toml"]
    let line = codex.Split('\n') |> Array.find (fun line -> line.StartsWith "description = ")
    equal expected (decodeToml (line.Substring "description = ".Length))

let B020_SkillSchemaValidatesMetadataAndStructure () =
    let root = checkout ()
    let path = "skills/other/SKILL.md"
    let valid = "---\nname: other\ndescription: >\n  Synthetic folded description\n  for schema validation.\nmetadata:\n  author: https://example.com/synthetic\n  version: \"1.0.0\"\n  role: specialist\n  persona: synthetic\n  related-skills: first, second\n  custom-field: extension\n---\n\n# Other\n\n## Never add\n\n- Invalid input.\n"
    write root path valid
    plan root |> ignore
    for invalid in [ valid.Replace("description: >\n  Synthetic folded description\n  for schema validation.\n", ""); valid.Replace("## Never add", "## Other"); valid.Replace("author: https://example.com/synthetic", "author: false") ] do
        write root path invalid
        let error = expectFailure (fun () -> plan root)
        contains path error
        contains "schema validation failed" error
        nothingWritten root
    File.Delete(Path.Combine(root, path))
    contains "required skill document is missing" (expectFailure (fun () -> plan root))

let B021_ExamplesCannotSatisfyRequiredHeadings () =
    let root = checkout ()
    let original = read root "agents/beta.md"
    for example in [ "\n```markdown\n## Refuse\n```\n"; "\n<!--\n## Refuse\n-->\n"; "\n> ## Refuse\n" ] do
        write root "agents/beta.md" (original.Replace("## Refuse", "## Other") + example)
        let error = expectFailure (fun () -> plan root)
        contains "agents/beta.md" error
        contains "/sections" error
        nothingWritten root

let B022_InvalidSchemasPreserveExistingOutputs () =
    let root = checkout ()
    let output = plan root
    apply root output |> ignore
    let snapshot = output.Outputs |> Map.toList |> List.map (fun (path, _) -> path, File.ReadAllBytes(Path.Combine(root, path)))
    let schema = "schemas/agent.schema.json"
    let original = read root schema
    for invalid in [ "{"; "{\"type\":123}" ] do
        write root schema invalid
        contains schema (expectFailure (fun () -> plan root))
        for path, bytes in snapshot do equal bytes (File.ReadAllBytes(Path.Combine(root, path)))
    File.Delete(Path.Combine(root, schema))
    contains schema (expectFailure (fun () -> plan root))
    for path, bytes in snapshot do equal bytes (File.ReadAllBytes(Path.Combine(root, path)))
    write root schema original
    equal [] (differences root (plan root))

let B023_ChangedSchemaRulesTakeEffectOnNextPlan () =
    let root = checkout ()
    plan root |> ignore
    let schema = "schemas/skill.schema.json"
    write root schema ((read root schema).Replace("Never add", "Guard"))
    contains "schema validation failed" (expectFailure (fun () -> plan root))
    let skill = "skills/sdd-conventions/SKILL.md"
    write root skill ((read root skill).Replace("## Never add", "## Guard"))
    plan root |> ignore

let B025_TrackerDirectoryIsNotASkill () =
    let root = checkout ()
    let tracker = "id: synthetic\nstatus: ready\n"
    write root "skills/.issue/synthetic.yml" tracker
    let result = plan root
    apply root result |> ignore
    equal tracker (read root "skills/.issue/synthetic.yml")
    for host in [ ".claude/skills"; ".agents/skills" ] do
        equal false (Directory.Exists(Path.Combine(root, host, ".issue")))
        equal true (exists root (host + "/sdd-conventions/SKILL.md"))

let B025_NestedTrackerResourcesAreOmittedAndRuntimeBytesSurvive () =
    let root = checkout ()
    let metadata = [ "skills/sdd-conventions/.issue/item.yml"; "skills/sdd-conventions/references/.issue/item.yml" ]
    for path in metadata do write root path "id: synthetic\n"
    write root "skills/sdd-conventions/.spec/README.md" "producer spec\n"
    let runtime = [ ".runtime/payload.bin", "\000\001binary"; ".issue-notes.md", "runtime notes\n"; ".issue-data/payload.bin", "\000\255resource" ]
    for rel,content in runtime do write root ("skills/sdd-conventions/" + rel) content
    apply root (plan root) |> ignore
    for host in [ ".claude/skills"; ".agents/skills" ] do
        for rel in [ ".issue/item.yml"; "references/.issue/item.yml"; ".spec/README.md" ] do
            equal false (exists root (host + "/sdd-conventions/" + rel))
        for rel,_ in runtime do
            equal (File.ReadAllBytes(Path.Combine(root, "skills/sdd-conventions", rel)))
                  (File.ReadAllBytes(Path.Combine(root, host, "sdd-conventions", rel)))
    for path in metadata do equal "id: synthetic\n" (read root path)

let B008_B025_StaleTrackerOutputsAreReportedAndRemoved () =
    let root = checkout ()
    write root ".issue/source.yml" "source tracker\n"
    apply root (plan root) |> ignore
    let stale = [ ".claude/skills/sdd-conventions/.issue/old.yml"; ".agents/skills/.issue/old.yml" ]
    for path in stale do write root path "old projected tracker\n"
    let expected = stale |> List.map Stale |> List.sort
    equal expected (differences root (plan root) |> List.sort)
    for path in stale do equal "old projected tracker\n" (read root path)
    apply root (plan root) |> ignore
    for path in stale do equal false (exists root path)
    equal "source tracker\n" (read root ".issue/source.yml")

let F4_B004_TrackerRecordsAreIgnoredButRuntimeStillFailsLint () =
    let root = checkout ()
    let records = [ "skills/.issue/item.yml"; "skills/sdd-conventions/.issue/item.yml"; "skills/sdd-conventions/references/.issue/item.yml" ]
    for path in records do write root path "Ask `alpha`.\n"
    lintSkills root (loadRoles root)
    write root "skills/sdd-conventions/.runtime/guide.md" "Ask `alpha`.\n"
    let message = expectFailure (fun () -> lintSkills root (loadRoles root))
    contains "skills/sdd-conventions/.runtime/guide.md:1: names role 'alpha'" message
    for path in records do
        if message.Contains path then failwithf "tracker was linted: %s" path

// ---------------------------------------------------------------- Runner

let tests : (string * (unit -> unit)) list =
    [ nameof B025_TrackerDirectoryIsNotASkill, B025_TrackerDirectoryIsNotASkill
      nameof B025_NestedTrackerResourcesAreOmittedAndRuntimeBytesSurvive, B025_NestedTrackerResourcesAreOmittedAndRuntimeBytesSurvive
      nameof B008_B025_StaleTrackerOutputsAreReportedAndRemoved, B008_B025_StaleTrackerOutputsAreReportedAndRemoved
      nameof F4_B004_TrackerRecordsAreIgnoredButRuntimeStillFailsLint, F4_B004_TrackerRecordsAreIgnoredButRuntimeStillFailsLint
      nameof B019_LiteralDescriptionRoundTripsThroughBothHosts, B019_LiteralDescriptionRoundTripsThroughBothHosts
      nameof B019_PortableFrontmatterAcceptsUserExamples, B019_PortableFrontmatterAcceptsUserExamples
      nameof B019_AgentSchemaRejectsInvalidFrontmatter, B019_AgentSchemaRejectsInvalidFrontmatter
      nameof B020_SkillSchemaValidatesMetadataAndStructure, B020_SkillSchemaValidatesMetadataAndStructure
      nameof B021_ExamplesCannotSatisfyRequiredHeadings, B021_ExamplesCannotSatisfyRequiredHeadings
      nameof B022_InvalidSchemasPreserveExistingOutputs, B022_InvalidSchemasPreserveExistingOutputs
      nameof B023_ChangedSchemaRulesTakeEffectOnNextPlan, B023_ChangedSchemaRulesTakeEffectOnNextPlan
      nameof B001_RoleAndClaudeOverlay_EmitsFrontmatterAndVerbatimBody, B001_RoleAndClaudeOverlay_EmitsFrontmatterAndVerbatimBody
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
      nameof B024_HostOverlaysRemainAuthoritative, B024_HostOverlaysRemainAuthoritative
      nameof B017_Personas_WriteRosterBetweenMarkers, B017_Personas_WriteRosterBetweenMarkers
      nameof B018_Persona_PrecedesVerbatimBodyOnBothHosts, B018_Persona_PrecedesVerbatimBodyOnBothHosts
      nameof B013_ReviewUnits_WriteRegistryBetweenMarkers, B013_ReviewUnits_WriteRegistryBetweenMarkers
      nameof B014_TwoRolesReviewOneUnit_FailsNamingBoth, B014_TwoRolesReviewOneUnit_FailsNamingBoth
      nameof B015_UnreviewedOrUndeclaredUnit_FailsNamingIt, B015_UnreviewedOrUndeclaredUnit_FailsNamingIt
      nameof B016_StampNamingANonOwner_FailsNamingTheSectionAndBothRoles, B016_StampNamingANonOwner_FailsNamingTheSectionAndBothRoles
      nameof B016_DesignSectionsRejectAnImplementationStamp, B016_DesignSectionsRejectAnImplementationStamp
      nameof B016_SignOffRejectsAReviewersStamp, B016_SignOffRejectsAReviewersStamp
      nameof B016_TildeFencedExampleDoesNotCreateAnOwnershipViolation, B016_TildeFencedExampleDoesNotCreateAnOwnershipViolation
      nameof B016_ShorterBackticksDoNotEndAFencedExample, B016_ShorterBackticksDoNotEndAFencedExample
      nameof B016_RealViolationAfterAFencedExampleIsStillRejected, B016_RealViolationAfterAFencedExampleIsStillRejected
      nameof B017_MixedPersonas_SortNamedThenUnnamedRoles, B017_MixedPersonas_SortNamedThenUnnamedRoles
      nameof B018_UnnamedPersona_PreservesVoiceOnBothHosts, B018_UnnamedPersona_PreservesVoiceOnBothHosts
      nameof F2_B040_InvalidSuppliedNameFails, F2_B040_InvalidSuppliedNameFails
      nameof F2_B040_InvalidVoiceFails, F2_B040_InvalidVoiceFails
      nameof F4_B001_UnnamedPersona_HasNoNamePattern, F4_B001_UnnamedPersona_HasNoNamePattern
      nameof C3_Outputs_AreDeterministicAcrossRuns, C3_Outputs_AreDeterministicAcrossRuns
      nameof F2_B041_EverySourceRoleDeclaresRepositoryKeys, F2_B041_EverySourceRoleDeclaresRepositoryKeys
      nameof F2_B002_EveryTemplateSectionHasExactlyOneOwner, F2_B002_EveryTemplateSectionHasExactlyOneOwner
      nameof F2_B003_EveryRoleBodyCarriesTheFourSections, F2_B003_EveryRoleBodyCarriesTheFourSections
      nameof F2_B005_LookForRoleCarriesReport, F2_B005_LookForRoleCarriesReport
      nameof F2_B006_EverySkillLinkResolves, F2_B006_EverySkillLinkResolves
      nameof F2_B040_PersonaRequiresVoiceAndOptionalUniqueName, F2_B040_PersonaRequiresVoiceAndOptionalUniqueName
      nameof F2_B008_ReviewingRoleCarriesLookForAndReport, F2_B008_ReviewingRoleCarriesLookForAndReport
      nameof F2_B009_ReviewUnitDeclaresTriggerAndRefusal, F2_B009_ReviewUnitDeclaresTriggerAndRefusal
      nameof F3_B001_CommandProjectsToClaudeCommandFile, F3_B001_CommandProjectsToClaudeCommandFile
      nameof F3_B002_CommandProjectsToCodexSkill, F3_B002_CommandProjectsToCodexSkill
      nameof F3_B003_CommandNamingMissingRole_FailsNamingBoth, F3_B003_CommandNamingMissingRole_FailsNamingBoth
      nameof F3_B004_CommandNamingMissingSkill_FailsNamingBoth, F3_B004_CommandNamingMissingSkill_FailsNamingBoth
      nameof F3_B005_MalformedCommand_Fails, F3_B005_MalformedCommand_Fails
      nameof F3_B006_CommandsFollowTheHostAndStaleOnesAreReported, F3_B006_CommandsFollowTheHostAndStaleOnesAreReported
      nameof F3_B007_CommandNamedLikeSkill_Fails, F3_B007_CommandNamedLikeSkill_Fails
      nameof F4_B001_SkillNamingRole_FailsNamingFileLineAndRole, F4_B001_SkillNamingRole_FailsNamingFileLineAndRole
      nameof F4_B002_GeneratedBlockIsExempt, F4_B002_GeneratedBlockIsExempt
      nameof F4_B003_RepositorySkillsNameNoRole, F4_B003_RepositorySkillsNameNoRole
      nameof F2_B033_NineteenRolesHaveExplicitMappingsOnBothHosts, F2_B033_NineteenRolesHaveExplicitMappingsOnBothHosts
      nameof F2_B033_NewSpecialistsProjectSourceBodiesAndAgreedProfiles, F2_B033_NewSpecialistsProjectSourceBodiesAndAgreedProfiles
      nameof F2_B034_FourReviewUnitsAndExistingSectionOwnersStayUnchanged, F2_B034_FourReviewUnitsAndExistingSectionOwnersStayUnchanged
      nameof F2_B034_MigrationRemovesThreeRolesAddsTwoAndPassesPublicCheck, F2_B034_MigrationRemovesThreeRolesAddsTwoAndPassesPublicCheck
      nameof F2_B034_RetiredBindingsFailBeforeWriting, F2_B034_RetiredBindingsFailBeforeWriting
      nameof F2_B035_HistoricalEvidenceAndExistingOutputsSurviveRejectedPlans, F2_B035_HistoricalEvidenceAndExistingOutputsSurviveRejectedPlans
      nameof F2_B034_ActiveConveningBindingsResolveToRetainedRoles, F2_B034_ActiveConveningBindingsResolveToRetainedRoles
      nameof F2_B037_OptionalObservabilityAcceptsItsDeclaredOwnerStamp, F2_B037_OptionalObservabilityAcceptsItsDeclaredOwnerStamp
      nameof F2_B037_AbsentObservabilityNeedsNoSubsectionOrTemplateEdit, F2_B037_AbsentObservabilityNeedsNoSubsectionOrTemplateEdit
      nameof F2_B037_ArchitectCannotStampObservabilityBeforeWrites, F2_B037_ArchitectCannotStampObservabilityBeforeWrites
      nameof F2_B037_SiblingDesignAndPerformanceKeepTheirExistingOwners, F2_B037_SiblingDesignAndPerformanceKeepTheirExistingOwners
      nameof F2_B039_RenameKeepsNineteenRolesMigratesBindingsAndPreservesHistory, F2_B039_RenameKeepsNineteenRolesMigratesBindingsAndPreservesHistory
      nameof F2_B039_FormerOperationalBindingsFailWithoutChangingExistingOutputs, F2_B039_FormerOperationalBindingsFailWithoutChangingExistingOutputs ]

let mutable failures = 0
for name, test in tests do
    try
        test ()
        printfn "  ok   %s" name
    with ex ->
        failures <- failures + 1
        printfn "  FAIL %s\n       %s" name (ex.Message.Replace("\n", "\n       "))

if Directory.Exists scratch then Directory.Delete(scratch, true)

printfn "%d tests, %d failed" tests.Length failures
exit failures
