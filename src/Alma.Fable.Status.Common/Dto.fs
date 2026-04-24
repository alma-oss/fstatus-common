namespace Alma.Status.Common

open System
open Alma.ServiceIdentification

type StatusMessage = {
    DateTime: DateTimeOffset
    Message: string option
    Note: string option
}

[<RequireQualifiedAccess>]
type Status =
    | Normal of StatusMessage
    | Info of StatusMessage
    | Warning of StatusMessage
    | Critical of StatusMessage

    static member (+) (current: Status, status: Status) : Status =
        match current, status with
        // normal wont change status
        | current, Status.Normal _ -> current

        // normal | info -> info, keeping first info
        | Status.Normal _, (Status.Info _ as info)
        | (Status.Info _ as info), Status.Info _ -> info

        // normal | info | warning -> warning, keeping first warning
        | Status.Normal _, (Status.Warning _ as warning)
        | Status.Info _, (Status.Warning _ as warning)
        | (Status.Warning _ as warning), Status.Normal _
        | (Status.Warning _ as warning), Status.Info _
        | (Status.Warning _ as warning), Status.Warning _ -> warning

        // * | critical -> critical, keeping first critical
        | (Status.Critical _ as critical), _
        | _, (Status.Critical _ as critical) -> critical

type Tag =
    | Tag of string
    | TagKV of string * string

type SystemStatusItem = { Name: string; Status: Status; Tags: Tag list }

type ServiceStatusItem = {
    Name: string
    Instance: Instance
    Status: Status
    Tags: Tag list
}

type Detail = { Name: string; Value: DetailValue }

and DetailValue =
    | Number of int
    | String of string

type DataObjectStatusItem = {
    Name: string
    Status: Status
    Details: Detail list
    Tags: Tag list
}

type StatusItem =
    | System of SystemStatusItem
    | Service of ServiceStatusItem
    | DataObject of DataObjectStatusItem

    member this.Name =
        match this with
        | System { Name = name }
        | Service { Name = name }
        | DataObject { Name = name } -> name

    member this.Status: Status =
        match this with
        | System { Status = status }
        | Service { Status = status }
        | DataObject { Status = status } -> status

[<RequireQualifiedAccess>]
module Status =
    let add (current: Status) (status: Status) : Status =
        current + status

    let zero =
        Status.Normal {
            DateTime = DateTimeOffset.Now
            Message = None
            Note = None
        }

    let fold (statuses: Status list) : Status = statuses |> List.fold (+) zero

    let foldItems (statuses: StatusItem list) : Status =
        statuses |> List.map (fun item -> item.Status) |> fold

module History =
    type IncidentLevel =
        | Warning
        | Critical

    type IncidentHistory = {
        Service: Instance
        System: Service
        CheckName: string
        Level: IncidentLevel
        From: DateTimeOffset
        Occurrences: int
        To: DateTimeOffset option
        Detail: string option
    }
