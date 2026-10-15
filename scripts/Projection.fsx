// Projection: turn host-free role contracts plus a host overlay into host-native
// agent files, copy skills to each host's discovery directory, and generate the
// section-ownership table and the persona roster. No top-level effects: project.fsx and
// project.tests.fsx load this file and call into it.
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
    { Name = name; Description = description; Version = version
      OwnedSections = owned; Persona = persona; Body = body; Source = source }

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

/// Sections no single role owns, declared in agents/README.md frontmatter as
/// `unowned_spec_sections: { "<key>": <who> }`.
let loadUnowned (root: string) : (string * string) list =
    let path = Path.Combine(root, "agents", "README.md")
    if not (File.Exists path) then [] else
    let pairs, _ = splitFrontmatter "agents/README.md" (readText path)
    match tryGet "unowned_spec_sections" pairs with
    | Some (Mapping entries) ->
        entries |> List.map (fun (key, value) ->
            match value with
            | Scalar who -> key, who
            | _ -> fail "agents/README.md: unowned_spec_sections values must be text")
    | Some _ -> fail "agents/README.md: unowned_spec_sections must be a mapping"
    | None -> []

// ---------------------------------------------------------------- Hosts

type Host =
    { Id: string
      Output: string
      SkillsOutput: string
      InstructionsPrefix: string option
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
    let prefix =
        match tryGet "instructions_prefix" pairs with
        | Some (Scalar s) -> Some s
        | Some _ -> fail (sprintf "hosts/%s.yaml: instructions_prefix must be text" id)
        | None -> None
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
      InstructionsPrefix = prefix; Defaults = mapping "defaults"; Roles = roles }

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

let renderOwnershipTable (rows: (string * string) list) =
    let width (column: string * string -> string) = rows |> List.map (fun r -> (column r).Length) |> List.max
    let w1 = max (width fst) "Section".Length
    let w2 = max (width snd) "Owner".Length
    let line (a: string) (b: string) = sprintf "| %s | %s |" (a.PadRight w1) (b.PadRight w2)
    [ yield line "Section" "Owner"
      yield sprintf "| %s | %s |" (String('-', w1)) (String('-', w2))
      for a, b in rows do yield line a b ]
    |> String.concat "\n"

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
    let rows =
        roles
        |> List.choose (fun r -> r.Persona |> Option.map (fun p -> p.Name, sprintf "[`%s`](%s.md)" r.Name r.Name, p.Voice))
        |> List.sortBy (fun (persona, _, _) -> persona)
    let width f header = rows |> List.map (fun r -> String.length (f r)) |> List.fold max (String.length header)
    let w1 = width (fun (a, _, _) -> a) "Persona"
    let w2 = width (fun (_, b, _) -> b) "Role"
    let w3 = width (fun (_, _, c) -> c) "Voice"
    let line (a: string) (b: string) (c: string) = sprintf "| %s | %s | %s |" (a.PadRight w1) (b.PadRight w2) (c.PadRight w3)
    [ yield line "Persona" "Role" "Voice"
      yield sprintf "| %s | %s | %s |" (String('-', w1)) (String('-', w2)) (String('-', w3))
      for a, b, c in rows do yield line a b c ]
    |> String.concat "\n"

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

// ---------------------------------------------------------------- Plan

type Plan =
    { Outputs: Map<string, byte[]>
      OutputRoots: string list }

let outputRoots (hosts: Host list) =
    [ for host in hosts do
        yield Path.GetDirectoryName(host.Output).Replace('\\', '/')
        yield host.SkillsOutput ]
    |> List.distinct
    |> List.sort

let plan (root: string) : Plan =
    let roles = loadRoles root
    if roles.IsEmpty then fail "agents/ holds no role files"
    validateSkillLinks root roles
    validatePersonas roles
    let hosts = [ loadHost root "claude"; loadHost root "codex" ]
    for host in hosts do validateHostRoles host roles
    let unowned = loadUnowned root
    let template = loadTemplateSections root
    let table = ownershipRows roles unowned template |> renderOwnershipTable
    let generated (rel: string) block content =
        let path = Path.Combine(root, rel)
        if not (File.Exists path) then fail (sprintf "%s is missing" rel)
        replaceGenerated rel block (readText path) content
    let companionText = generated "skills/sdd-conventions/SKILL.md" "section-ownership" table
    let personasText = generated "agents/PERSONAS.md" "personas" (renderPersonaTable roles)
    let skillsDirectory = Path.Combine(root, "skills")
    let skills =
        Directory.EnumerateDirectories skillsDirectory
        |> Seq.filter (fun d -> Path.GetFileName d <> ".spec")
        |> Seq.sort
        |> List.ofSeq
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
                      yield host.SkillsOutput + "/" + rel, content ]
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
