// ========================================================================================================
// === F# / Project fake build ==================================================================== 1.6.0 =
// --------------------------------------------------------------------------------------------------------
// Options:
//  - no-clean   - disables clean of dirs in the first step (required on CI)
//  - no-lint    - lint will be executed, but the result is not validated
// ========================================================================================================

open Fake.Core
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

open ProjectBuild
open Utils

[<EntryPoint>]
let main args =
    args |> Args.init

    Targets.init {
        Project = {
            Name = "Alma.Fable.Status.Common"
            Summary = "Shared DTOs and utility functions for fstatus client and server applications."
            Git = Git.init ()
        }
        Specs =
            Spec.defaultLibrary
            |> Spec.mapLibrary (fun library -> {
                library with
                    NugetApi = NugetApi.KeyInEnvironment "NUGET_API_KEY"
            })
    }

    args |> Args.run
