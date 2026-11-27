open Alma.Build
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
