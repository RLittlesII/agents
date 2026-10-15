// Entry point. See scripts/.spec/README.md.
//
//   dotnet fsi scripts/project.fsx                 regenerate every output
//   dotnet fsi scripts/project.fsx --check         write nothing; exit 1 on drift
//   dotnet fsi scripts/project.fsx --root <dir>    operate on another checkout

#load "Projection.fsx"

open System
open System.IO
open Projection

let args = fsi.CommandLineArgs |> Array.skip 1 |> List.ofArray

let rec parse (check, root) args =
    match args with
    | [] -> check, root
    | "--check" :: rest -> parse (true, root) rest
    | "--root" :: dir :: rest -> parse (check, dir) rest
    | other :: _ -> eprintfn "error: unknown argument '%s'" other; exit 2

let check, root =
    parse (false, Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))) args

try
    let plan = plan root
    if check then
        let diffs = differences root plan
        if diffs.IsEmpty then
            printfn "up to date: %d outputs" plan.Outputs.Count
            exit 0
        else
            for diff in diffs do eprintfn "%s" (describe diff)
            eprintfn "error: %d outputs differ; run `dotnet fsi scripts/project.fsx`" diffs.Length
            exit 1
    else
        let diffs = apply root plan
        for diff in diffs do printfn "%s" (describe diff)
        printfn "%d outputs, %d written" plan.Outputs.Count diffs.Length
        exit 0
with ProjectionError message ->
    eprintfn "error: %s" message
    exit 2
