# Anti-Patterns — Alma.Status.Common

Each entry is **mistake → why → fix**.

## Aggregation

- **Assuming `(+)` / `fold` keeps the last or most recent message at the top
  severity** → The operator keeps the *first* message encountered at the highest
  severity, not the newest by `DateTime` → Order the input so the message you want
  to surface comes first, or compare timestamps yourself before folding.

- **Hand-rolling a reduction without a seed** (e.g. `List.reduce (+)` on a possibly
  empty list) → Throws on an empty list and skips the defined `Normal` baseline →
  Use `Status.fold`, which seeds with `Status.zero`.

- **Treating `foldItems` and `fold` as interchangeable** → `fold` takes a
  `Status list`; `foldItems` takes a `StatusItem list` and projects `.Status` first
  → Pick the helper that matches your input type instead of manually mapping then
  calling the wrong one.

## Construction

- **Building `Status` cases unqualified** (relying on `open` to expose `Warning`)
  → `Status` is `[RequireQualifiedAccess]`, so unqualified cases will not compile →
  Always write `Status.Warning`, `Status.Normal`, etc.

- **Using a `DateTime` for `StatusMessage.DateTime`** → The field is a
  `DateTimeOffset` → Construct with `DateTimeOffset` values (e.g. `DateTimeOffset.UtcNow`).

- **Passing raw strings for `Message`/`Note`** → Both fields are `string option` →
  Wrap present values in `Some` and use `None` for absence.

- **Encoding numeric details as `String`** → `DetailValue` offers `Number of int`
  for integers → Use `Number` for integer detail values and `String` only for text.

## Identity / Access

- **Pattern-matching the `StatusItem` DU just to read the name or severity** →
  Verbose and easy to get wrong across three cases → Use the `.Name` and `.Status`
  members.

- **Confusing `Tag` and `TagKV`** → `Tag of string` is a plain label;
  `TagKV of string * string` is a key/value pair → Use `TagKV` only when you have a
  key and a value; otherwise use `Tag`.

## Scope

- **Expecting this library to run health checks or schedule probes** → It only
  defines the status data contract and aggregation helpers → Perform check
  execution in the consuming application; use these types only to represent results.
