## 1. Project Scaffold

- [x] 1.1 Create ASP.NET Core Razor Pages project (`dotnet new webapp`) with solution file
- [x] 1.2 Add `appsettings.json` configuration section for YNAB API settings (BaseUrl, AccessToken) and configure user-secrets for local development
- [x] 1.3 Add Chart.js CDN reference to the shared layout (`_Layout.cshtml`)

## 2. YNAB API Data Models

- [x] 2.1 Create DTOs: `BudgetSummaryResponse`, `BudgetSummary` (Id, Name) mapping to YNAB `GET /v1/budgets` response
- [x] 2.2 Create DTOs: `CategoriesResponse`, `CategoryGroupWithCategories`, `Category` (Id, Name, Budgeted, GoalType, GoalTarget, GoalPercentageComplete, Hidden, Deleted) mapping to YNAB `GET /v1/budgets/{id}/categories` response
- [x] 2.3 Create configuration class `YnabApiOptions` with `BaseUrl` and `AccessToken` properties

## 3. YNAB API Client Service

- [x] 3.1 Define `IYnabApiClient` interface with methods: `GetBudgetsAsync()` and `GetCategoriesAsync(string budgetId)`
- [x] 3.2 Implement `YnabApiClient` using `HttpClient` — add Bearer token via `DelegatingHandler`, call YNAB endpoints, deserialize responses into DTOs
- [x] 3.3 Register `IYnabApiClient` in DI with `IHttpClientFactory`; bind `YnabApiOptions` from configuration; validate token is present at startup

## 4. Report View Models and Calculation Logic

- [x] 4.1 Create view models: `TargetProportionItem` (Name, TargetAmount, Percentage, Color) and `TargetProportionReportViewModel` (BudgetName, Items list, TotalTarget, GroupedByCategory flag, Budgets list for selector)
- [x] 4.2 Implement proportion calculation service/helper: filter out categories without targets and those hidden/deleted, compute each subcategory's percentage of total, sort descending by amount
- [x] 4.3 Implement category-group aggregation logic: sum subcategory targets per group, compute group-level percentages

## 5. Report Razor Page

- [x] 5.1 Create `Pages/Reports/TargetProportions.cshtml.cs` PageModel — inject `IYnabApiClient`, accept budget ID and grouping toggle as query/form parameters, build `TargetProportionReportViewModel`
- [x] 5.2 Create `Pages/Reports/TargetProportions.cshtml` Razor view — render budget dropdown selector, category/subcategory toggle, and summary table with Name, Target Amount (currency-formatted), and Percentage columns
- [x] 5.3 Add Chart.js donut chart rendering via `<script>` block — serialize view model items to JSON, initialize Chart.js doughnut chart with labels, data, and colors; configure hover tooltips showing name, amount, and percentage
- [x] 5.4 Wire up toggle behavior: form POST or JS-driven re-render to switch between subcategory and category-group views
- [x] 5.5 Handle edge case: display "No target data available" message when the selected budget has no subcategories with targets

## 6. Navigation and Layout

- [x] 6.1 Add a navigation link to the Target Proportions report page in the shared layout or home page
- [x] 6.2 Style the report page layout — chart and table side-by-side using CSS grid or flexbox
