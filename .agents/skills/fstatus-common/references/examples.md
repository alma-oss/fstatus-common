# Examples — Alma.Status.Common

All runnable code for this skill lives here. Examples are ordered by increasing
complexity and each one is self-contained.

## Basic — a single status

```fs
open System
open Alma.Status.Common

let status =
    Status.Warning {
        DateTime = DateTimeOffset.UtcNow
        Message = Some "Latency above threshold"
        Note = Some "p95 measured at 1.8s"
    }
```

## Basic — formatting a timestamp

```fs
open System
open Alma.Status.Common

let formatted =
    DateTimeOffset.UtcNow
    |> DateTimeOffset.pretty   // "dd.MM.yyyy H:mm:ss"

printfn "%s" formatted
```

## Realistic — building items and folding to one status

```fs
open System
open Alma.Status.Common

let now = DateTimeOffset.UtcNow

let serviceAStatus =
    Status.Warning {
        DateTime = now
        Message = Some "Response time is above threshold"
        Note = None
    }

let cacheStatus =
    Status.Normal {
        DateTime = now
        Message = Some "Connection pool is healthy"
        Note = None
    }

let items =
    [ StatusItem.System {
        Name = "ServiceA"
        Status = serviceAStatus
        Tags = [ Tag "public"; TagKV ("region", "zone-1") ] }
      StatusItem.DataObject {
        Name = "CacheInstance"
        Status = cacheStatus
        Details = [ { Name = "connections"; Value = Number 12 } ]
        Tags = [ Tag "cache" ] } ]

// Highest severity wins -> Warning; first Warning message is preserved.
let overall = items |> Status.foldItems

// Reading identity/severity via members:
let firstName = items.Head.Name
let firstSeverity = items.Head.Status
```

## Realistic — folding a plain status list and combining two

```fs
open System
open Alma.Status.Common

let mk msg severity =
    severity {
        DateTime = DateTimeOffset.UtcNow
        Message = Some msg
        Note = None
    }

let statuses =
    [ mk "ok" Status.Normal
      mk "first warning" Status.Warning
      mk "second warning" Status.Warning ]

// Folds to Warning, keeping "first warning".
let folded = Status.fold statuses

// Combine exactly two:
let combined = Status.add (mk "info" Status.Info) (mk "critical" Status.Critical)
// -> Critical
```

## Integration — service item and incident history with Alma.ServiceIdentification

```fs
open System
open Alma.ServiceIdentification
open Alma.Status.Common

let serviceItem =
    StatusItem.Service {
        Name = "Worker"
        Instance = Instance "worker-01"
        Status =
            Status.Info {
                DateTime = DateTimeOffset.UtcNow
                Message = Some "Deployment in progress"
                Note = None
            }
        Tags = [ TagKV ("version", "2.3.0") ]
    }

let incident: History.IncidentHistory =
    { Service = Instance "worker-01"
      System = Service "WebApi"
      CheckName = "latency"
      Level = History.Critical
      From = DateTimeOffset.UtcNow
      Occurrences = 3
      To = None
      Detail = Some "p95 latency exceeded budget" }
```

## Test — asserting severity and surviving message

```fs
open System
open Alma.Status.Common

let private msg text =
    { DateTime = DateTimeOffset.UtcNow; Message = Some text; Note = None }

// Empty list folds to the Normal baseline (Status.zero).
let emptyFold = Status.fold []

// Mixed severities with duplicate top severity keep the first such message.
let result =
    Status.fold
        [ Status.Normal (msg "ok")
          Status.Warning (msg "kept")
          Status.Warning (msg "dropped") ]

match result with
| Status.Warning m -> assert (m.Message = Some "kept")
| _ -> failwith "expected Warning"
```

## Full Workflow — collect, aggregate, format for display

```fs
open System
open Alma.Status.Common

let buildReport (items: StatusItem list) =
    let overall = Status.foldItems items

    let severityLabel =
        match overall with
        | Status.Normal _ -> "NORMAL"
        | Status.Info _ -> "INFO"
        | Status.Warning _ -> "WARNING"
        | Status.Critical _ -> "CRITICAL"

    let timestamp =
        match overall with
        | Status.Normal m
        | Status.Info m
        | Status.Warning m
        | Status.Critical m -> DateTimeOffset.pretty m.DateTime

    sprintf "[%s] %s (%d items)" severityLabel timestamp (List.length items)
```
