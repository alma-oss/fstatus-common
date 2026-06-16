---
name: fstatus-common
description: >-
  Use whenever generating or reviewing F# code that builds or aggregates status
  payloads with the Alma.Status.Common package (namespace Alma.Status.Common).
  Trigger on Status.fold, Status.foldItems, Status.add, Status.zero, the Status
  severity DU (Normal | Info | Warning | Critical), StatusItem (System | Service |
  DataObject), SystemStatusItem / ServiceStatusItem / DataObjectStatusItem,
  StatusMessage, Tag / TagKV, Detail / DetailValue, History.IncidentHistory, and
  the DateTimeOffset.pretty formatter. Also trigger on mentions of composing or
  folding health/status checks, severity precedence, or exchanging status data
  contracts between Alma.Status and Alma.Fable.Status.
---

# F-Status-Common

Library: [alma-oss/fstatus-common](https://github.com/alma-oss/fstatus-common)
NuGet: `Alma.Status.Common`

## Purpose

`Alma.Status.Common` is a shared F# library that defines the status payload data
contract (DTOs) and a small set of helpers used by both producer and consumer
sides so they exchange identical data. It models severity levels, status items
for systems/services/data objects, and aggregation of many statuses into one.

## When to Use

- Constructing a `Status`, `StatusMessage`, `StatusItem`, `Tag`, or `Detail` value.
- Aggregating several statuses or items into a single overall status.
- Formatting a `DateTimeOffset` for status display.
- Modeling incident history records.

## When NOT to Use

- Running or scheduling health checks (this library only defines the data contract).
- Transport, serialization configuration, or HTTP wiring.
- Business/domain modeling — this is a generic status contract only.

## Main Concepts

- `StatusMessage` — timestamped payload: `DateTime`, optional `Message`, optional `Note`.
- `Status` — `[RequireQualifiedAccess]` severity DU: `Normal`, `Info`, `Warning`, `Critical`, each carrying a `StatusMessage`.
- `Status` `(+)` operator — combines two statuses by severity precedence.
- `Tag` — `Tag of string` (plain) or `TagKV of string * string` (key/value).
- `Detail` / `DetailValue` — named extra data; value is `Number of int` or `String of string`.
- `SystemStatusItem` / `ServiceStatusItem` / `DataObjectStatusItem` — concrete item records.
- `StatusItem` — DU wrapper `System | Service | DataObject` with `.Name` and `.Status` members.
- `Status` module — `add`, `zero`, `fold`, `foldItems` aggregation helpers.
- `History.IncidentLevel` / `History.IncidentHistory` — incident record types.
- `DateTimeOffset.pretty` — `[RequireQualifiedAccess]` helper formatting a timestamp.

## Related Libraries

- `Alma.ServiceIdentification` — supplies `Instance` (used by `ServiceStatusItem` and `History`) and `Service` (used by `History`).

## Keywords for Search

status, severity, Normal, Info, Warning, Critical, fold, foldItems, aggregate,
StatusItem, System, Service, DataObject, StatusMessage, Tag, TagKV, Detail,
DetailValue, IncidentHistory, DateTimeOffset.pretty, Alma.Status.Common, F#, DTO.

## Reference Files

- For composition principles and recommended API usage, read `references/preferred-patterns.md`.
- For known pitfalls and incorrect assumptions, read `references/anti-patterns.md`.
- For worked code examples, read `references/examples.md`.
