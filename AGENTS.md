# AGENTS.md — Alma.Status.Common

## Project Purpose

F# library published as NuGet package `Alma.Status.Common`. It provides shared DTOs and utility functions used by Alma.Status and Alma.Fable.Status libraries so both sides can exchange the same status payloads and helper behavior.

## Tech Stack

- **Language:** F# (.NET 10)
- **Framework:** .NET SDK class library
- **Package management:** Paket
- **Build system:** FAKE via `build.sh`
- **Linting:** fsharplint
- **CI/CD:** GitHub Actions
- **Key dependencies:** `FSharp.Core`, `Alma.ServiceIdentification`

## Commands

```bash
# Restore tools and packages
dotnet tool restore
dotnet tool run paket restore

# Build using the standard FAKE pipeline entrypoint
./build.sh build

# Run the test target
./build.sh -t tests

# Run lint directly for the library project
./build.sh -t lint
```

## Project Structure

```text
fstatus-common/
├── src/
│   └── Alma.Fable.Status.Common/
│       ├── Alma.Fable.Status.Common.fsproj # Main library project (PackageId: Alma.Status.Common)
│       ├── AssemblyInfo.fs           		# Checked-in assembly info source
│       ├── Utils.fs                  		# Small general-purpose helpers
│       ├── Dto.fs                    		# Shared status DTOs and related helpers
│       └── paket.references
├── build/
│   ├── Build.fs                  # FAKE entrypoint and project definition
│   └── Targets.fs                # Shared FAKE target implementation
├── build.sh                      # Shell wrapper for tool restore + FAKE
├── paket.dependencies
├── fsharplint.json
├── CHANGELOG.md
└── .github/workflows/
	├── tests.yaml
	├── pr-check.yaml
	└── publish.yaml
```

## Architecture

Two source modules define the public surface:

### `Alma.Status.Common` (`src/Alma.Fable.Status.Common/Dto.fs`)
- Record and DU types for shared health/status data.
- Models include `StatusMessage`, `Status`, `Tag`, `StatusItem`, and incident history records.
- `StatusItem` exposes convenience members for `Name` and `Status`.
- `Status` module provides aggregation helpers such as `add`, `fold`, and `foldItems`.

### `Alma.Status.Common` (`src/Alma.Fable.Status.Common/Utils.fs`)
- Small utility helpers intended to be shared by server and client code.
- Currently includes `DateTimeOffset.pretty` for consistent timestamp formatting.

## Build System (FAKE)

The repository uses the shared build infrastructure from the `build/` folder. For this library, the default target chain is:

`Clean -> AssemblyInfo -> Build -> Lint -> Tests -> Release -> Publish`

Relevant behavior:
- `Build.fs` defines the package metadata and configures the repo as a library build.
- `Tests` prints `There are no tests yet.` when no test projects are present.
- `Release` packs the NuGet package and moves `.nupkg` artifacts into the release directory.
- `Publish` pushes the package when the required environment configuration is present.

## CI/CD

- **tests.yaml** runs the repository test/build validation.
- **pr-check.yaml** contains pull request checks.
- **publish.yaml** handles package publishing.
- NuGet publishing is configured in FAKE to read the API key from `NUGET_API_KEY`.

## Release Process

1. Increment the version in `src/Alma.Fable.Status.Common/Alma.Fable.Status.Common.fsproj`.
2. Update `CHANGELOG.md`.
3. Commit the change and create a version tag.
4. Push the branch and tag so the publish workflow can release the package.

## Conventions

- Keep shared wire-format types in `src/Alma.Fable.Status.Common/Dto.fs`; keep general helpers in `src/Alma.Fable.Status.Common/Utils.fs`.
- Use discriminated unions and records to model status payloads explicitly.
- Preserve public namespaces: DTOs live under `Alma.Status.Common`, utilities under `Alma.Status.Common`.
- Follow the repository lint configuration in `fsharplint.json`, including 4-space indentation and standard F# naming rules.
- Keep library dependencies minimal because this package is intended to be shared across client and server code.

## Pitfalls

- There is currently **no test project** in the repository, so the FAKE `Tests` target is effectively a no-op.
- `src/Alma.Fable.Status.Common/AssemblyInfo.fs` is included explicitly by the project file; avoid changing compile order casually.
- `Status.add` keeps the first higher-severity status it encounters, so folding order can matter when messages differ.
- `Dto.fs` depends on `Alma.ServiceIdentification.Instance`; changes to those DTOs affect both producers and consumers.
- `build.sh` already restores tools and Paket dependencies, so duplicating restore steps in local scripts is usually unnecessary.
