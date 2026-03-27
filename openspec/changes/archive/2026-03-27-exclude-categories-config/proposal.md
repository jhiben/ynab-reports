## Why

Some YNAB category groups (e.g., "Internal Master Category", "Credit Card Payments", or one-off savings goals) are not meaningful in a proportional target report. Currently every category with a target appears in the chart, cluttering the visualization and skewing percentages. Users need a way to permanently exclude specific categories or category groups via configuration so the report focuses on recurring, meaningful budget allocations.

## What Changes

- Add a `ReportOptions` configuration section in `appsettings.json` with lists of category group names and/or category names to exclude from reports.
- Create an options class bound via DI to expose exclusion lists to the calculation layer.
- Filter out excluded categories/groups before computing proportions so they never appear in the chart or summary table.
- Matching is by name (case-insensitive) since YNAB category names are user-defined and stable.

## Capabilities

### New Capabilities
- `report-options`: Configuration model and binding for report-level settings, starting with category exclusion lists.

### Modified Capabilities
- `target-proportion-report`: The proportion calculation now filters out categories and category groups whose names appear in the configured exclusion lists before computing proportions.

## Impact

- **Configuration**: New `ReportOptions` section in `appsettings.json` (non-breaking — empty lists mean no exclusions).
- **Code**: New `ReportOptions` class, DI registration in `Program.cs`, filter step added to `TargetProportionCalculator`.
- **Data models**: No changes to YNAB DTOs.
