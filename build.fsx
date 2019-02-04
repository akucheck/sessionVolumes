#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

// Properties
// let buildDir = "./build/"
// let testDir  = "./test/"

Target.create "Clean" (fun _ ->
    !! "**/bin"
    ++ "**/obj"
    |> Shell.cleanDirs 
)

Target.create "BuildApp" (fun _ ->
    !! "**/*.*proj"
    |> Seq.iter (DotNet.build id) 
)

Target.create "All" ignore

"Clean"
  ==> "BuildApp"
  ==> "All"

Target.runOrDefault "All"
