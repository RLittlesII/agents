// Projection: turn host-free role contracts plus a host overlay into host-native
// agent files, copy skills to each host's discovery directory, and generate the
// section-ownership table, the review-pass registry and the persona roster, and
// project commands. No top-level effects: project.fsx and project.tests.fsx load
// this file and call into it.
//
// Agreement: scripts/.spec/README.md. Claims are cited as B-00n in comments.

#r "nuget: YamlDotNet, 16.3.0"

open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open YamlDotNet.RepresentationModel

exception ProjectionError of string

let fail message = raise (ProjectionError message)

// ---------------------------------------------------------------- YAML values

/// The subset of YAML this projector accepts: a scalar, a list of scalars, or a
/// mapping of either. Mapping order is preserved.
type Value =
    | Scalar of string
    | List of string list
    | Mapping of (string * Value) list

let rec private toValue (node: YamlNode) : Value =
    match node with
    | :? YamlScalarNode as scalar -> Scalar scalar.Value
    | :? YamlSequenceNode as sequence ->
        sequence.Children
        |> Seq.map (fun child ->
            match child with
            | :? YamlScalarNode as scalar -> scalar.Value
            | _ -> fail "A list may hold scalars only")
        |> List.ofSeq
        |> List
    | :? YamlMappingNode as mapping ->
        mapping.Children
        |> Seq.map (fun pair -> (pair.Key :?> YamlScalarNode).Value, toValue pair.Value)
        |> List.ofSeq
        |> Mapping
    | _ -> fail (sprintf "Unsupported YAML node %s" (node.GetType().Name))

let parseYaml (text: string) : (string * Value) list =
    if String.IsNullOrWhiteSpace text then [] else
    let stream = YamlStream()
    stream.Load(new StringReader(text))
    if stream.Documents.Count = 0 then [] else
    match toValue stream.Documents.[0].RootNode with
    | Mapping pairs -> pairs
    | _ -> fail "Expected a YAML mapping"

let tryGet key (pairs: (string * Value) list) =
    pairs |> List.tryFind (fun (k, _) -> k = key) |> Option.map snd

let getScalar key pairs =
    match tryGet key pairs with
    | Some (Scalar s) -> s
    | Some _ -> fail (sprintf "'%s' must be a single value" key)
    | None -> fail (sprintf "'%s' is required" key)

// ---------------------------------------------------------------- Files

let normalize (text: string) = text.Replace("\r\n", "\n").Replace("\r", "\n")

let readText (path: string) = File.ReadAllText(path, Encoding.UTF8) |> normalize

let utf8 = UTF8Encoding(false)

let bytes (text: string) = utf8.GetBytes text

let relative (root: string) (path: string) =
    Path.GetRelativePath(root, path).Replace('\\', '/')

let filesUnder (directory: string) =
    if Directory.Exists directory then
        Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
        |> Seq.filter (fun f -> not (f.Contains("/.spec/") || f.Contains("\\.spec\\")))
        |> Seq.sort
        |> List.ofSeq
    else []

/// Split a Markdown file into its frontmatter pairs and the body after the
/// closing `---`. The body keeps every byte after the single newline that
/// follows the closing marker.
let splitFrontmatter (path: string) (text: string) =
    if not (text.StartsWith "---\n") then fail (sprintf "%s: missing frontmatter" path)
    let close = text.IndexOf("\n---\n", 4)
    if close < 0 then fail (sprintf "%s: unterminated frontmatter" path)
    let yaml = text.Substring(4, close - 4)
    let body = text.Substring(close + 5)
    parseYaml yaml, body

// ---------------------------------------------------------------- Roles

/// 0001-F2 B-007: the name a role signs its prose with, and how that prose reads.
type Persona = { Name: string; Voice: string }

type Role =
    { Name: string
      Description: string
      Version: string
      OwnedSections: string list
      Persona: Persona option
      /// 0001-F2 B-008: the review units this role reviews.
      Reviews: string list
      Body: string
      Source: string }

let hostKeys = [ "model"; "effort"; "tools"; "model_reasoning_effort" ]

let private sectionKey = Regex(@"^(\d+|Tasks|Scoring|\d+/[A-Za-z][A-Za-z ]*)$")
let private semver = Regex(@"^\d+\.\d+\.\d+$")

let loadRole (root: string) (path: string) : Role =
    let source = relative root path
    let pairs, body = splitFrontmatter source (readText path)
    // B-010: a role file carries no host key.
    for key in hostKeys do
        if (tryGet key pairs).IsSome then
            fail (sprintf "%s: host key '%s' belongs in hosts/<host>.yaml, not in a role file" source key)
    let name = getScalar "name" pairs
    // B-009: the name matches the file name.
    let stem = Path.GetFileNameWithoutExtension path
    if name <> stem then fail (sprintf "%s: name '%s' does not match the file name '%s'" source name stem)
    let description = getScalar "description" pairs
    if description.Contains "\n" then fail (sprintf "%s: description must be a single line" source)
    let version = getScalar "version" pairs
    if not (semver.IsMatch version) then fail (sprintf "%s: version '%s' is not semver" source version)
    let owned =
        match tryGet "owned_spec_sections" pairs with
        | Some (List sections) -> sections
        | Some (Scalar "") | None -> fail (sprintf "%s: owned_spec_sections is required (an empty list is valid)" source)
        | Some _ -> fail (sprintf "%s: owned_spec_sections must be a list" source)
    for section in owned do
        if not (sectionKey.IsMatch section) then
            fail (sprintf "%s: owned section '%s' must be \"<n>\", \"Tasks\", \"Scoring\" or \"<n>/<Subsection>\"" source section)
    // 0001-F2 B-007: a persona carries a name and a voice, one line each.
    let persona =
        match tryGet "persona" pairs with
        | None -> None
        | Some (Mapping keys) ->
            let text key =
                match tryGet key keys with
                | Some (Scalar s) when not (String.IsNullOrWhiteSpace s) && not (s.Contains "\n") -> s
                | _ -> fail (sprintf "%s: persona.%s is required and must be a single line" source key)
            Some { Name = text "name"; Voice = text "voice" }
        | Some _ -> fail (sprintf "%s: persona must be a mapping of name and voice" source)
    // 0001-F2 B-008: a reviewing role states what it looks for and how it reports.
    let reviews =
        match tryGet "reviews" pairs with
        | Some (List units) -> units
        | Some (Scalar "") | None -> []
        | Some _ -> fail (sprintf "%s: reviews must be a list" source)
    if not reviews.IsEmpty && not (body.Contains "\n## Look for\n" && body.Contains "\n## Report\n") then
        fail (sprintf "%s: reviews %s but lacks ## Look for or ## Report" source (String.Join(", ", reviews)))
    { Name = name; Description = description; Version = version
      OwnedSections = owned; Persona = persona; Reviews = reviews; Body = body; Source = source }

/// Files in agents/ that are not roles: the roles index and the persona roster.
let notRoles = set [ "README.md"; "PERSONAS.md" ]

let loadRoles (root: string) : Role list =
    let directory = Path.Combine(root, "agents")
    if not (Directory.Exists directory) then fail "agents/ is missing"
    Directory.EnumerateFiles(directory, "*.md")
    |> Seq.filter (fun f -> not (notRoles.Contains(Path.GetFileName f)))
    |> Seq.sort
    |> Seq.map (loadRole root)
    |> List.ofSeq

let private skillLink = Regex(@"\.\./skills/([^/\s)]+)/SKILL\.md")

/// 0001-F2 B-006: every skill a role body links resolves to a skill directory.
let validateSkillLinks (root: string) (roles: Role list) =
    for role in roles do
        for m in skillLink.Matches role.Body do
            let skill = m.Groups.[1].Value
            if not (File.Exists(Path.Combine(root, "skills", skill, "SKILL.md"))) then
                fail (sprintf "%s: links skill '%s', which has no skills/%s/SKILL.md" role.Source skill skill)

/// A text-valued mapping from the roles index's frontmatter, in declared order.
let private indexMapping (root: string) (key: string) : (string * string) list =
    let path = Path.Combine(root, "agents", "README.md")
    if not (File.Exists path) then [] else
    let pairs, _ = splitFrontmatter "agents/README.md" (readText path)
    match tryGet key pairs with
    | Some (Mapping entries) ->
        entries |> List.map (fun (k, value) ->
            match value with
            | Scalar text -> k, text
            | _ -> fail (sprintf "agents/README.md: %s values must be text" key))
    | Some _ -> fail (sprintf "agents/README.md: %s must be a mapping" key)
    | None -> []

/// Sections no single role owns, declared in agents/README.md frontmatter as
/// `unowned_spec_sections: { "<key>": <who> }`.
let loadUnowned (root: string) = indexMapping root "unowned_spec_sections"

/// B-013: the review units, in pass order, declared in agents/README.md
/// frontmatter as `review_units: { <unit>: <what it reviews> }`.
let loadReviewUnits (root: string) = indexMapping root "review_units"

// ---------------------------------------------------------------- Hosts

type Host =
    { Id: string
      Output: string
      SkillsOutput: string
      InstructionsPrefix: string option
      /// 0001-F3: where a command lands, with `{name}`; None projects no commands.
      CommandsOutput: string option
      /// 0001-F3 B-002: the text that replaces `$ARGUMENTS` on a host without it.
      CommandsArguments: string option
      Defaults: (string * Value) list
      Roles: (string * (string * Value) list) list }

let private efforts = set [ "low"; "medium"; "high"; "xhigh"; "max" ]

let loadHost (root: string) (id: string) : Host =
    let path = Path.Combine(root, "hosts", id + ".yaml")
    if not (File.Exists path) then fail (sprintf "hosts/%s.yaml is missing" id)
    let pairs = parseYaml (readText path)
    let output = getScalar "output" pairs
    if not (output.Contains "{name}") then fail (sprintf "hosts/%s.yaml: output must contain {name}" id)
    let skillsOutput = getScalar "skills_output" pairs
    let optional key =
        match tryGet key pairs with
        | Some (Scalar s) -> Some s
        | Some _ -> fail (sprintf "hosts/%s.yaml: %s must be text" id key)
        | None -> None
    let prefix = optional "instructions_prefix"
    let commandsOutput = optional "commands_output"
    match commandsOutput with
    | Some o when not (o.Contains "{name}") -> fail (sprintf "hosts/%s.yaml: commands_output must contain {name}" id)
    | _ -> ()
    let mapping key =
        match tryGet key pairs with
        | Some (Mapping m) -> m
        | Some (Scalar "") | None -> []
        | Some _ -> fail (sprintf "hosts/%s.yaml: %s must be a mapping" id key)
    let roles =
        mapping "roles"
        |> List.map (fun (role, value) ->
            match value with
            | Mapping overlay -> role, overlay
            | Scalar "" -> role, []
            | _ -> fail (sprintf "hosts/%s.yaml: roles.%s must be a mapping" id role))
    { Id = id; Output = output; SkillsOutput = skillsOutput
      InstructionsPrefix = prefix; CommandsOutput = commandsOutput
      CommandsArguments = optional "commands_arguments"
      Defaults = mapping "defaults"; Roles = roles }

/// The overlay for one role: defaults, then the role's own keys, later keys
/// replacing earlier ones while keeping first-seen order.
let overlay (host: Host) (role: Role) : (string * Value) list =
    let own = host.Roles |> List.tryFind (fun (r, _) -> r = role.Name) |> Option.map snd |> Option.defaultValue []
    let merged =
        (host.Defaults @ own)
        |> List.fold (fun acc (key, value) ->
            if acc |> List.exists (fun (k, _) -> k = key) then
                acc |> List.map (fun (k, v) -> if k = key then (k, value) else (k, v))
            else acc @ [ (key, value) ]) []
    match tryGet "effort" merged with
    | Some (Scalar effort) when host.Id = "claude" && not (efforts.Contains effort) ->
        fail (sprintf "hosts/claude.yaml: effort '%s' for %s is not one of low, medium, high, xhigh, max" effort role.Name)
    | _ -> ()
    merged

let validateHostRoles (host: Host) (roles: Role list) =
    // B-009: every role the overlay names has a file.
    for name, _ in host.Roles do
        if not (roles |> List.exists (fun r -> r.Name = name)) then
            fail (sprintf "hosts/%s.yaml: role '%s' has no file agents/%s.md" host.Id name name)

// ---------------------------------------------------------------- Ownership

type TemplateSection = { Key: string; Heading: string }

let private heading = Regex(@"^##\s+(?:(\d+)\.\s+(.*?)|(\S.*?))\s*$")

let loadTemplateSections (root: string) : TemplateSection list =
    let path = Path.Combine(root, ".specht", "templates", "feature.md")
    if not (File.Exists path) then fail ".specht/templates/feature.md is missing; run `dotnet specht init --templates`"
    readText path
    |> fun text -> text.Split('\n')
    |> Array.choose (fun line ->
        let m = heading.Match line
        if not m.Success then None
        elif m.Groups.[1].Success then Some { Key = m.Groups.[1].Value; Heading = sprintf "%s. %s" m.Groups.[1].Value m.Groups.[2].Value }
        else Some { Key = m.Groups.[3].Value; Heading = m.Groups.[3].Value })
    |> List.ofArray

/// B-005, B-006, B-007: one owner per section, every template section owned,
/// no owner for a section the template lacks.
let ownershipRows (roles: Role list) (unowned: (string * string) list) (template: TemplateSection list) : (string * string) list =
    let mutable owners : (string * string) list = []
    let claim section owner =
        match owners |> List.tryFind (fun (s, _) -> s = section) with
        | Some (_, existing) when existing <> owner ->
            fail (sprintf "section '%s' is claimed by both '%s' and '%s'; a section has exactly one owner" section existing owner)
        | Some _ -> ()
        | None -> owners <- owners @ [ (section, owner) ]
    for role in roles do
        for section in role.OwnedSections do claim section (sprintf "`%s`" role.Name)
    for section, who in unowned do claim section who
    if template.IsEmpty then fail ".specht/templates/feature.md has no `## ` sections"
    let keys = template |> List.map (fun s -> s.Key)
    let isSub (section: string) = section.Contains "/"
    let missing = keys |> List.filter (fun k -> not (owners |> List.exists (fun (s, _) -> s = k)))
    let extra = owners |> List.map fst |> List.filter (fun s -> not (isSub s) && not (List.contains s keys))
    for s in owners |> List.map fst |> List.filter isSub do
        let parent = s.Substring(0, s.IndexOf '/')
        if not (List.contains parent keys) then
            fail (sprintf "section '%s' names parent '%s', which the template lacks" s parent)
    if not missing.IsEmpty then
        fail (sprintf "the section-ownership table is not total; owned by nobody: %s" (String.Join(", ", missing)))
    if not extra.IsEmpty then
        fail (sprintf "claimed but absent from the template: %s" (String.Join(", ", extra)))
    [ for section in template do
        yield (section.Heading, (owners |> List.find (fun (s, _) -> s = section.Key) |> snd))
        for s, owner in owners do
            if isSub s && s.StartsWith(section.Key + "/") then
                yield (sprintf "%s. › `### %s`" section.Key (s.Substring(s.IndexOf '/' + 1)), owner) ]

/// A Markdown table with every column padded to its widest cell.
let renderTable (headers: string list) (rows: string list list) =
    let widths =
        headers |> List.mapi (fun i h -> rows |> List.fold (fun w row -> max w (String.length (List.item i row))) h.Length)
    let line cells = "| " + String.Join(" | ", List.map2 (fun (c: string) w -> c.PadRight w) cells widths) + " |"
    [ yield line headers
      yield line (widths |> List.map (fun w -> String('-', w)))
      for row in rows do yield line row ]
    |> String.concat "\n"

let renderOwnershipTable (rows: (string * string) list) =
    renderTable [ "Section"; "Owner" ] (rows |> List.map (fun (a, b) -> [ a; b ]))

/// B-013, B-014, B-015: one reviewer per unit, every unit reviewed, no role
/// reviewing a unit the vocabulary lacks. Rows follow the vocabulary's order.
let reviewRows (roles: Role list) (units: (string * string) list) : (string * string * string) list =
    let reviewers =
        [ for role in roles do
            for unit in role.Reviews do
                if not (units |> List.exists (fun (u, _) -> u = unit)) then
                    fail (sprintf "%s: reviews '%s', which agents/README.md review_units does not declare" role.Source unit)
                yield unit, role.Name ]
    for unit, holders in reviewers |> List.groupBy fst do
        match holders with
        | (_, first) :: (_, second) :: _ ->
            fail (sprintf "review unit '%s' is reviewed by both '%s' and '%s'; a unit has exactly one reviewer" unit first second)
        | _ -> ()
    let missing = units |> List.map fst |> List.filter (fun u -> not (reviewers |> List.exists (fun (r, _) -> r = u)))
    if not missing.IsEmpty then
        fail (sprintf "the review-pass registry is not total; reviewed by nobody: %s" (String.Join(", ", missing)))
    units |> List.map (fun (unit, what) -> unit, what, reviewers |> List.find (fun (u, _) -> u = unit) |> snd)

let renderReviewTable (rows: (string * string * string) list) =
    renderTable [ "Order"; "Unit"; "Reviews"; "Reviewer" ]
        (rows |> List.mapi (fun i (unit, what, role) -> [ string (i + 1); unit; what; sprintf "`%s`" role ]))

let markers (block: string) =
    sprintf "<!-- BEGIN GENERATED: %s -->" block, sprintf "<!-- END GENERATED: %s -->" block

let beginMarker, endMarker = markers "section-ownership"

/// Replace the text between a named pair of generated markers in `file`.
let replaceGenerated (file: string) (block: string) (text: string) (content: string) =
    let opening, closing = markers block
    let start = text.IndexOf opening
    let finish = text.IndexOf closing
    if start < 0 || finish < 0 || finish < start then fail (sprintf "%s lacks the %s markers" file block)
    text.Substring(0, start + opening.Length) + "\n" + content + "\n" + text.Substring finish

// ---------------------------------------------------------------- One-way rule

let private generatedBlock = Regex(@"<!-- BEGIN GENERATED: ([\w-]+) -->.*?<!-- END GENERATED: \1 -->", RegexOptions.Singleline)
let private agentsPath = Regex(@"\.\./agents/|(?<![\w./-])agents/[\w-]+\.md")

/// 0001-F4 B-001, B-002: no file under skills/ names a role outside a generated
/// block. A role is named by its name in backticks, a hyphenated name as a whole
/// word, its persona name as a whole word, or a path into agents/. A bare
/// one-word role name is ordinary English and is left to review.
let lintSkills (root: string) (roles: Role list) =
    let patterns =
        [ for role in roles do
            yield role.Name, Regex(Regex.Escape("`" + role.Name + "`"))
            if role.Name.Contains "-" then
                yield role.Name, Regex(@"(?<![\w-])" + Regex.Escape role.Name + @"(?![\w-])")
            match role.Persona with
            | Some p -> yield role.Name, Regex(@"\b" + Regex.Escape p.Name + @"\b")
            | None -> () ]
    let findings =
        [ for file in filesUnder (Path.Combine(root, "skills")) do
            let raw = File.ReadAllBytes file
            if not (Array.contains 0uy raw) then
                // Blank each generated block but keep its newlines, so line numbers hold.
                let text =
                    generatedBlock.Replace(normalize (utf8.GetString raw), fun m -> String('\n', m.Value.Split('\n').Length - 1))
                let rel = relative root file
                for i, line in text.Split('\n') |> Array.indexed do
                    for name, pattern in patterns do
                        if pattern.IsMatch line then yield sprintf "%s:%d: names role '%s'" rel (i + 1) name
                    if agentsPath.IsMatch line then yield sprintf "%s:%d: names a path into agents/" rel (i + 1) ]
        |> List.distinct
    if not findings.IsEmpty then
        fail (sprintf "a skill never names a role; name the section owner or the review pass instead:\n%s" (String.concat "\n" findings))

// ---------------------------------------------------------------- Personas

/// 0001-F2 B-007: a persona name belongs to one role.
let validatePersonas (roles: Role list) =
    roles
    |> List.choose (fun r -> r.Persona |> Option.map (fun p -> p.Name, r.Name))
    |> List.groupBy fst
    |> List.iter (fun (persona, holders) ->
        match holders with
        | (_, first) :: (_, second) :: _ ->
            fail (sprintf "persona '%s' is declared by both '%s' and '%s'; a persona signs for one role" persona first second)
        | _ -> ())

/// B-011: the persona roster, one row per persona, sorted by persona name.
let renderPersonaTable (roles: Role list) =
    roles
    |> List.choose (fun r -> r.Persona |> Option.map (fun p -> [ p.Name; sprintf "[`%s`](%s.md)" r.Name r.Name; p.Voice ]))
    |> List.sortBy List.head
    |> renderTable [ "Persona"; "Role"; "Voice" ]

/// B-012: the line that tells a role which name to sign with. Machine-read
/// stamps keep the role name, so a tool never has to map one to the other.
let personaLine (role: Role) =
    role.Persona
    |> Option.map (fun p ->
        sprintf "Persona: **%s**. %s Sign the prose you write — reports, findings, notes — as %s; a stamp a tool reads, such as `last written by` or `author`, keeps the role name `%s`."
            p.Name p.Voice p.Name role.Name)

/// Put a lead paragraph ahead of a body, separated by one blank line whether or
/// not the body opens with its own.
let private lead (paragraph: string) (body: string) =
    paragraph + (if body.StartsWith "\n" then "\n" else "\n\n") + body

// ---------------------------------------------------------------- Rendering

let private bare = Regex(@"^[A-Za-z0-9._-]+$")

let yamlScalar (s: string) =
    if bare.IsMatch s then s
    else "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""

let yamlValue (value: Value) =
    match value with
    | Scalar s -> yamlScalar s
    | List items -> "[" + String.Join(", ", items |> List.map yamlScalar) + "]"
    | Mapping _ -> fail "a nested mapping cannot be emitted as an overlay key"

/// A TOML basic string: `\\`, `"` and control characters escaped, every other
/// character — including non-ASCII and characters outside the BMP — written as
/// itself. A JSON serializer is not used because it escapes astral characters as
/// surrogate pairs, which TOML rejects.
let tomlString (s: string) =
    let sb = StringBuilder(s.Length + 2)
    sb.Append '"' |> ignore
    for c in s do
        match c with
        | '\\' -> sb.Append "\\\\" |> ignore
        | '"' -> sb.Append "\\\"" |> ignore
        | '\b' -> sb.Append "\\b" |> ignore
        | '\t' -> sb.Append "\\t" |> ignore
        | '\n' -> sb.Append "\\n" |> ignore
        | '\012' -> sb.Append "\\f" |> ignore
        | '\r' -> sb.Append "\\r" |> ignore
        | c when c < ' ' || c = '\127' -> sb.AppendFormat("\\u{0:X4}", int c) |> ignore
        | c -> sb.Append c |> ignore
    sb.Append '"' |> ignore
    sb.ToString()

let tomlValue (value: Value) =
    match value with
    | Scalar s -> tomlString s
    | List items -> "[" + String.Join(", ", items |> List.map tomlString) + "]"
    | Mapping _ -> fail "a nested mapping cannot be emitted as an overlay key"

/// B-001: Claude frontmatter is name, description, then the overlay; the body
/// follows verbatim. B-012: a persona line, when the role has one, comes first.
let renderClaude (host: Host) (role: Role) =
    let keys = overlay host role
    let body =
        match personaLine role with
        | Some line -> "\n" + lead line role.Body
        | None -> role.Body
    [ yield "---"
      yield "name: " + yamlScalar role.Name
      yield "description: " + "\"" + role.Description.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""
      for key, value in keys do yield key + ": " + yamlValue value
      yield "---" ]
    |> String.concat "\n"
    |> fun front -> front + "\n" + body

/// B-002: Codex TOML is name, description, developer_instructions, then the
/// overlay. The instructions are the prefix, the persona line (B-012), one blank
/// line, and the body verbatim; the body keeps the newline that follows the
/// frontmatter, so the text after the lead is byte-equal to the role body.
let renderCodex (host: Host) (role: Role) =
    let prefix = host.InstructionsPrefix |> Option.map (fun p -> p.Replace("{source}", role.Source))
    let instructions =
        match List.choose id [ prefix; personaLine role ] with
        | [] -> role.Body
        | paragraphs -> lead (String.concat "\n\n" paragraphs) role.Body
    [ yield "name = " + tomlString role.Name
      yield "description = " + tomlString role.Description
      yield "developer_instructions = " + tomlString instructions
      for key, value in overlay host role do yield key + " = " + tomlValue value ]
    |> String.concat "\n"
    |> fun text -> text + "\n"

let renderAgent (host: Host) (role: Role) =
    if host.Output.EndsWith ".toml" then renderCodex host role else renderClaude host role

// ---------------------------------------------------------------- Commands

/// 0001-F3: a composable prompt that convenes roles and names skills. Roles and
/// skills are named once, in frontmatter; projection writes the paths per host.
type Command =
    { Name: string
      Description: string
      Roles: string list
      Skills: string list
      ArgumentHint: string option
      Body: string
      Source: string }

let private relativeLink = Regex(@"\]\(\.\./(agents|skills)/")

let loadCommand (root: string) (path: string) : Command =
    let source = relative root path
    let pairs, body = splitFrontmatter source (readText path)
    // B-005: a well-formed command.
    let name = getScalar "name" pairs
    let stem = Path.GetFileNameWithoutExtension path
    if name <> stem then fail (sprintf "%s: name '%s' does not match the file name '%s'" source name stem)
    let description =
        match tryGet "description" pairs with
        | Some (Scalar s) when not (String.IsNullOrWhiteSpace s) && not (s.Contains "\n") -> s
        | _ -> fail (sprintf "%s: description is required and must be a single line" source)
    let list key =
        match tryGet key pairs with
        | Some (List items) -> items
        | Some (Scalar "") | None -> []
        | Some _ -> fail (sprintf "%s: %s must be a list" source key)
    let roles = list "roles"
    if roles.IsEmpty then fail (sprintf "%s: roles must name at least one role" source)
    if relativeLink.IsMatch body then
        fail (sprintf "%s: the body links into agents/ or skills/; name roles and skills in frontmatter, which projection turns into each host's paths" source)
    let hint =
        match tryGet "argument-hint" pairs with
        | Some (Scalar s) -> Some s
        | Some _ -> fail (sprintf "%s: argument-hint must be text" source)
        | None -> None
    { Name = name; Description = description; Roles = roles; Skills = list "skills"
      ArgumentHint = hint; Body = body; Source = source }

let loadCommands (root: string) : Command list =
    let directory = Path.Combine(root, "commands")
    if not (Directory.Exists directory) then [] else
    Directory.EnumerateFiles(directory, "*.md")
    |> Seq.filter (fun f -> Path.GetFileName f <> "README.md")
    |> Seq.sort
    |> Seq.map (loadCommand root)
    |> List.ofSeq

/// B-003, B-004, B-007: every role and skill a command names exists, and no
/// command shares a name with a skill.
let validateCommands (roles: Role list) (skills: string list) (commands: Command list) =
    for command in commands do
        for role in command.Roles do
            if not (roles |> List.exists (fun r -> r.Name = role)) then
                fail (sprintf "%s: names role '%s', which has no file agents/%s.md" command.Source role role)
        for skill in command.Skills do
            if not (List.contains skill skills) then
                fail (sprintf "%s: names skill '%s', which has no skills/%s/SKILL.md" command.Source skill skill)
        if List.contains command.Name skills then
            fail (sprintf "%s: command '%s' has the name of skill '%s'; a host that projects commands as skills would hold both in one place" command.Source command.Name command.Name)

/// The generated paragraphs ahead of a command body: each role's contract and
/// each skill at this host's paths.
let commandPreamble (host: Host) (command: Command) =
    let contract role = host.Output.Replace("{name}", role)
    let keep = "keep the boundaries it declares: a role that refuses a kind of work still refuses it here."
    let roles =
        match command.Roles with
        | [ role ] ->
            sprintf "Act as the `%s` role, whose contract is `%s`. Read it before acting, and %s" role (contract role) keep
        | many ->
            [ yield "Convene these roles, in this order:"
              yield ""
              for role in many do yield sprintf "- `%s`, whose contract is `%s`" role (contract role)
              yield ""
              yield "Read each role's contract before acting as it, and " + keep ]
            |> String.concat "\n"
    let skills =
        command.Skills
        |> List.map (fun s -> sprintf "`%s` (`%s/%s/SKILL.md`)" s host.SkillsOutput s)
    match skills with
    | [] -> roles
    | _ -> roles + "\n\nUse these skills: " + String.Join(", ", skills) + "."

let private quoted (s: string) = "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""

/// B-001: a command file — description and argument hint, the preamble, then
/// the body verbatim. B-002: a skill file — name and description, the preamble,
/// then the body with `$ARGUMENTS` replaced by the host's text for it.
let renderCommand (host: Host) (command: Command) =
    let asSkill = (defaultArg host.CommandsOutput "").EndsWith "/SKILL.md"
    let body =
        match host.CommandsArguments with
        | Some text -> command.Body.Replace("$ARGUMENTS", text)
        | None -> command.Body
    [ yield "---"
      if asSkill then yield "name: " + yamlScalar command.Name
      yield "description: " + quoted command.Description
      match command.ArgumentHint with
      | Some hint when not asSkill -> yield "argument-hint: " + quoted hint
      | _ -> ()
      yield "---" ]
    |> String.concat "\n"
    |> fun front -> front + "\n\n" + lead (commandPreamble host command) body

// ---------------------------------------------------------------- Plan

type Plan =
    { Outputs: Map<string, byte[]>
      OutputRoots: string list }

let outputRoots (hosts: Host list) =
    [ for host in hosts do
        yield Path.GetDirectoryName(host.Output).Replace('\\', '/')
        yield host.SkillsOutput
        // 0001-F3 B-006: the directory before `{name}`, so a stale command is found.
        match host.CommandsOutput with
        | Some output -> yield output.Substring(0, output.IndexOf "{name}").TrimEnd('/')
        | None -> () ]
    |> List.distinct
    |> List.sort

let plan (root: string) : Plan =
    let roles = loadRoles root
    if roles.IsEmpty then fail "agents/ holds no role files"
    validateSkillLinks root roles
    validatePersonas roles
    lintSkills root roles
    let hosts = [ loadHost root "claude"; loadHost root "codex" ]
    for host in hosts do validateHostRoles host roles
    let unowned = loadUnowned root
    let template = loadTemplateSections root
    let table = ownershipRows roles unowned template |> renderOwnershipTable
    let generated (rel: string) block content =
        let path = Path.Combine(root, rel)
        if not (File.Exists path) then fail (sprintf "%s is missing" rel)
        replaceGenerated rel block (readText path) content
    let reviews = reviewRows roles (loadReviewUnits root) |> renderReviewTable
    let companionText =
        let companion = "skills/sdd-conventions/SKILL.md"
        let withOwnership = generated companion "section-ownership" table
        replaceGenerated companion "review-passes" withOwnership reviews
    let personasText = generated "agents/PERSONAS.md" "personas" (renderPersonaTable roles)
    let skillsDirectory = Path.Combine(root, "skills")
    let skills =
        Directory.EnumerateDirectories skillsDirectory
        |> Seq.filter (fun d -> Path.GetFileName d <> ".spec")
        |> Seq.sort
        |> List.ofSeq
    let commands = loadCommands root
    validateCommands roles (skills |> List.map Path.GetFileName) commands
    let outputs =
        [ yield "skills/sdd-conventions/SKILL.md", bytes companionText
          yield "agents/PERSONAS.md", bytes personasText
          for host in hosts do
              for role in roles do
                  yield host.Output.Replace("{name}", role.Name), bytes (renderAgent host role)
              // B-004: every skill file, byte for byte, into the host's skills directory.
              for skill in skills do
                  for file in filesUnder skill do
                      let rel = relative skillsDirectory file
                      let content =
                          if rel = "sdd-conventions/SKILL.md" then bytes companionText
                          else File.ReadAllBytes file
                      yield host.SkillsOutput + "/" + rel, content
              match host.CommandsOutput with
              | Some output ->
                  for command in commands do
                      yield output.Replace("{name}", command.Name), bytes (renderCommand host command)
              | None -> () ]
        |> Map.ofList
    { Outputs = outputs; OutputRoots = outputRoots hosts }

type Difference =
    | Changed of string
    | Missing of string
    | Stale of string

/// B-008: every planned output compared byte for byte, plus files in the output
/// roots the plan does not produce.
let differences (root: string) (plan: Plan) : Difference list =
    let planned =
        [ for KeyValue(rel, content) in plan.Outputs do
            let path = Path.Combine(root, rel)
            if not (File.Exists path) then yield Missing rel
            elif File.ReadAllBytes path <> content then yield Changed rel ]
    let stale =
        [ for outputRoot in plan.OutputRoots do
            for file in filesUnder (Path.Combine(root, outputRoot)) do
                let rel = relative root file
                if not (plan.Outputs.ContainsKey rel) then yield Stale rel ]
    planned @ stale

let apply (root: string) (plan: Plan) : Difference list =
    let diffs = differences root plan
    for diff in diffs do
        match diff with
        | Changed rel | Missing rel ->
            let path = Path.Combine(root, rel)
            Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
            File.WriteAllBytes(path, plan.Outputs.[rel])
        | Stale rel -> File.Delete(Path.Combine(root, rel))
    diffs

let describe (diff: Difference) =
    match diff with
    | Changed rel -> "changed " + rel
    | Missing rel -> "missing " + rel
    | Stale rel -> "stale   " + rel
