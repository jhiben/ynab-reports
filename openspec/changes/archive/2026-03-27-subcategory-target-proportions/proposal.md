## Why

Users budgeting with YNAB assign targets (goals) to subcategories, but there is no quick way to see how each subcategory's target compares to the total budget target. A visual proportion report lets users instantly identify where their money is allocated, spot over-weighted categories, and make informed rebalancing decisions.

## What Changes

- Introduce a new Razor Page that displays a proportional breakdown of subcategory targets relative to the total target amount across all subcategories.
- **Normalize all targets to a monthly amount** regardless of the underlying YNAB goal type (e.g., monthly funding targets are used as-is, yearly targets like "Target Balance by Date" are divided by the number of months remaining, "Savings Balance" targets are divided by 12). This ensures an apples-to-apples comparison of how much each subcategory costs per month.
- Fetch budget, category group, category (subcategory), and target data from the YNAB API.
- Render an interactive chart (e.g., donut/pie or treemap) showing each subcategory's share of the total **monthly** target.
- Support grouping by category group so users can view proportions at either the subcategory or category-group level.
- Provide a summary table alongside the chart with monthly target amounts and percentages.

## Capabilities

### New Capabilities
- `ynab-api-client`: Service to authenticate and fetch budgets, category groups, categories, and their targets from the YNAB API.
- `target-proportion-report`: Razor Page and supporting logic to normalize targets to monthly amounts, calculate proportional breakdowns, and render them as a visual chart and summary table.

### Modified Capabilities
<!-- No existing capabilities to modify — this is the first feature. -->

## Impact

- **New code**: ASP.NET Core project scaffold, YNAB API client service, report page with Razor view and page model.
- **Dependencies**: YNAB API v1 (`api.ynab.com`), a JavaScript charting library (e.g., Chart.js) for client-side rendering.
- **APIs**: Consumes YNAB API endpoints — `GET /budgets`, `GET /budgets/{id}/categories`.
- **Data models**: `Budget`, `CategoryGroup`, `Category` (subcategory), `CategoryTarget` — mirroring the YNAB API resource shapes.
