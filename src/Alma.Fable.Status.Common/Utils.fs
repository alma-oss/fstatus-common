namespace Alma.Status.Common

[<RequireQualifiedAccess>]
module DateTimeOffset =
    let pretty (dateTime: System.DateTimeOffset) = dateTime.ToString "dd.MM.yyyy H:mm:ss"
