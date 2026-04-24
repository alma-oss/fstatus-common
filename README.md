Alma.Status.Common
==================

[![NuGet](https://img.shields.io/nuget/v/Alma.Status.Common.svg)](https://www.nuget.org/packages/Alma.Status.Common)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Alma.Status.Common.svg)](https://www.nuget.org/packages/Alma.Status.Common)
[![Tests](https://github.com/alma-oss/fstatus-common/actions/workflows/tests.yaml/badge.svg)](https://github.com/alma-oss/fstatus-common/actions/workflows/tests.yaml)

> Shared DTOs and utility functions used by Alma.Status and Alma.Fable.Status libraries.

> The package provides a single place for the status payload model so both sides can exchange the same data contract.

## Install

Add the package reference with Paket:

```paket
nuget Alma.Status.Common
```

or with the .NET CLI:

```bash
dotnet add package Alma.Status.Common
```

## Use

### Status items and aggregation

```fs
open System
open Alma.ServiceIdentification
open Alma.Status.Common

let now = DateTimeOffset.UtcNow

let apiStatus =
	Status.Warning {
		DateTime = now
		Message = Some "Response time is above threshold"
		Note = Some "p95 latency is 1.8s"
	}

let dbStatus =
	Status.Normal {
		DateTime = now
		Message = Some "Connection pool is healthy"
		Note = None
	}

let items = [
	StatusItem.System {
		Name = "api"
		Status = apiStatus
		Tags = [ Tag "public"; TagKV ("region", "eu-west") ]
	}
	StatusItem.DataObject {
		Name = "database"
		Status = dbStatus
		Details = [ Detail { Name = "connections"; Value = Number 12 } ]
		Tags = [ Tag "postgres" ]
	}
]

let overallStatus = items |> Status.foldItems
```

`Status.add`, `Status.fold`, and `Status.foldItems` keep the highest severity found in the input while preserving the first message for that severity.

### Working with service instances

```fs
open System
open Alma.ServiceIdentification
open Alma.Status.Common

let statusItem =
	StatusItem.Service {
		Name = "worker"
		Instance = Instance "worker-01"
		Status =
			Status.Info {
				DateTime = DateTimeOffset.UtcNow
				Message = Some "Deployment in progress"
				Note = None
			}
		Tags = [ TagKV ("version", "2.3.0") ]
	}

let itemName = statusItem.Name
let itemStatus = statusItem.Status
```

### Date formatting helper

```fs
open System
open Alma.Status.Common

let formatted =
	DateTimeOffset.UtcNow
	|> DateTimeOffset.pretty

printfn "%s" formatted
```

The `DateTimeOffset.pretty` helper formats timestamps as `dd.MM.yyyy H:mm:ss`.

## Data model

Main DTOs exposed by the package:

- `StatusMessage` for timestamped status payloads with optional message and note.
- `Status` for severity levels: `Normal`, `Info`, `Warning`, and `Critical`.
- `Tag` for plain tags and key-value tags.
- `StatusItem` for `System`, `Service`, and `DataObject` status entries.
- `History.IncidentHistory` for incident history records.

## Release

1. Increment the version in `src/Alma.Fable.Status.Common/Alma.Fable.Status.Common.fsproj`.
2. Update `CHANGELOG.md`.
3. Commit the change and create a version tag.
4. Push the branch and tag so the publish workflow can release the package.

## Development

### Requirements

- .NET SDK
- Paket (`dotnet tool restore` in this repository restores the required tools)

### Build

```bash
./build.sh build
```

### Tests

```bash
./build.sh -t tests
```

### Lint

```bash
./build.sh -t lint
```
