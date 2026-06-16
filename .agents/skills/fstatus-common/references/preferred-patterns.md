# Preferred Patterns — Alma.Status.Common

## Core Principles

- Severity precedence is fixed and total: `Normal < Info < Warning < Critical`.
  Aggregation always yields the highest severity present in the input.
- Aggregation is message-preserving: when several statuses share the highest
  severity, the **first** one encountered (in fold order) keeps its `StatusMessage`.
  Input order therefore matters when messages differ at the same severity.
- The DTOs are the wire contract. Keep them stable and construct them with
  qualified access (`Status.Warning`, `StatusItem.System`, `Tag`, `TagKV`).

## Recommended API Usage

- Build a leaf status with one of the four `Status` cases, each wrapping a
  `StatusMessage` whose `DateTime` is a `DateTimeOffset` and whose `Message`/`Note`
  are `string option`. See `examples.md` → Basic.
- Aggregate a plain `Status list` with `Status.fold`; it seeds with `Status.zero`
  (a `Normal` status) and combines with the `(+)` operator. See `examples.md` → Realistic.
- Aggregate a heterogeneous `StatusItem list` with `Status.foldItems`; it projects
  each item through its `.Status` member before folding. See `examples.md` → Realistic.
- Combine exactly two statuses with `Status.add a b` (equivalent to `a + b`).
- Read an item's identity and severity through the `.Name` and `.Status` members
  rather than pattern-matching the DU when you only need those two fields.

## Error Handling

- There are no exceptions or result types in this library; absence is modeled with
  `option`. Use `None` for a missing `Message` or `Note`, and `Some` otherwise.
- `Detail` values are constrained to `Number of int` or `String of string`; pick the
  matching case instead of stringifying numbers.

## Composition

- Prefer the provided helpers (`fold`, `foldItems`, `add`) over hand-rolled
  reductions so the seed and precedence rules stay consistent.
- When ordering matters for which message survives, sort or arrange the list before
  folding rather than post-processing the result.

## Integration with Other Libraries

- `ServiceStatusItem.Instance` and `History.IncidentHistory.Service` expect an
  `Instance` from `Alma.ServiceIdentification`; `History.IncidentHistory.System`
  expects a `Service` from the same package. See `examples.md` → Integration.

## Naming Conventions

- `Status` and `DateTimeOffset` are `[RequireQualifiedAccess]` modules/types —
  always call them qualified (`Status.fold`, `DateTimeOffset.pretty`).
- Construct DU cases with their qualified names to keep call sites unambiguous.

## Testing Recommendations

- Assert aggregation by checking both the resulting severity case and the surviving
  `StatusMessage`, since the first-message-wins rule is easy to regress.
- Cover at least: empty list (folds to `Normal`/`zero`), single item, and mixed
  severities with duplicate top severities. See `examples.md` → Test.
