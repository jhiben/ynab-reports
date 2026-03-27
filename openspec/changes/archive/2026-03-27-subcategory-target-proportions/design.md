## Context

This is a greenfield ASP.NET Core Razor Pages application. No existing code or infrastructure is in place. The application will consume the YNAB (You Need A Budget) API to fetch budget data and render visual financial reports. The first report is a proportional breakdown of subcategory targets.

The YNAB API v1 exposes budgets, category groups, and categories (subcategories) with optional target/goal information. Authentication uses a personal access token passed as a Bearer token.

## Goals / Non-Goals

**Goals:**
- Establish the foundational ASP.NET Core project structure for future reports.
- Build a reusable YNAB API client service that can be shared across reports.
- Deliver a single report page showing subcategory target proportions as an interactive chart with a summary table.
- **Normalize all target types to a per-month amount** so proportions are always comparable regardless of goal cadence.
- Support toggling between subcategory-level and category-group-level views.

**Non-Goals:**
- Real-time or webhook-based data updates — data is fetched on page load.
- Multi-user authentication or account management — uses a single personal access token.
- Editing or writing back to YNAB (read-only).
- Mobile-optimized or responsive design beyond basic usability.
- Caching or offline support.

## Decisions

### 1. ASP.NET Core Razor Pages (not MVC or Blazor)

**Choice**: Razor Pages with server-side rendering and lightweight client-side JS for charts.
**Rationale**: Razor Pages are simpler for page-oriented reports. Blazor adds unnecessary complexity for a read-only dashboard. MVC's controller/view separation is overkill for page-per-report structure.
**Alternative considered**: Blazor Server — rejected because the interactivity is limited to chart filtering, which Chart.js handles natively.

### 2. Chart.js for visualization

**Choice**: Chart.js (CDN-hosted) for rendering donut/pie and optional treemap charts.
**Rationale**: Lightweight, widely adopted, zero build-tool dependencies. Works well with server-rendered JSON data passed to the page.
**Alternative considered**: D3.js — more powerful but higher complexity for standard chart types.

### 3. YNAB API client as an injected service

**Choice**: `IYnabApiClient` interface with `HttpClient`-based implementation, registered via DI.
**Rationale**: Follows ASP.NET Core conventions, enables testability via interface mocking, and allows `IHttpClientFactory` for proper `HttpClient` lifecycle management.
**Alternative considered**: Static helper class — rejected for poor testability and lifecycle management.

### 4. Personal access token via configuration

**Choice**: Store the YNAB API token in `appsettings.json` (user secrets for development).
**Rationale**: Simplest approach for a single-user tool. No OAuth flow needed.

### 5. Data flow architecture

**Choice**: PageModel calls `IYnabApiClient` → maps API DTOs to view models → serializes to JSON for Chart.js.
**Rationale**: Clean separation. DTOs mirror the API shape; view models carry only what the chart and table need (name, amount, percentage, color).

### 6. Monthly normalization of targets

**Choice**: Normalize every target to a per-month amount before computing proportions. The YNAB API exposes `goal_type`, `goal_target`, `goal_cadence`, `goal_cadence_frequency`, and `goal_target_date`. Normalization rules:
- **MF (Monthly Funding) / NEED / DEBT** — use `goal_cadence` and `goal_cadence_frequency` to determine the period, then divide to monthly. Cadence 1 = monthly (÷ frequency), cadence 2 = weekly (× 52/12/freq), cadence 3-12 = every N months, cadence 13 = yearly (÷ 12 × freq), cadence 14 = every 2 years (÷ 24).
- **TBD (Target Balance by Date)** — divide `goal_target` by the number of months remaining until `goal_target_date`. If the target date is in the past or the current month, treat as a single-month target.
- **TB (Target Balance)** — apply the same cadence-based normalization as MF.
- Unknown / null goal type — exclude from the report.

**Rationale**: Without normalization, a yearly savings goal of $12,000 would visually dwarf a $1,000/month rent target, even though they cost the same per month. Per-month normalization gives an accurate picture of monthly budget allocation.
**Alternative considered**: Showing raw target amounts — rejected because mixed cadences produce misleading proportions.

## Risks / Trade-offs

- **[YNAB API rate limits]** → The API has a rate limit of 200 requests per hour. Mitigation: A single page load requires only 1-2 API calls; acceptable for personal use. Future: add response caching.
- **[API token exposure]** → Token stored in configuration could leak. Mitigation: Use user-secrets in development; environment variables in production.
- **[Categories without targets]** → Some subcategories may have no target set. Mitigation: Exclude them from the chart or show as "No Target" with zero value; display a note to the user.
- **[Normalization edge cases]** → TBD targets with past due dates or zero months remaining could produce infinite/invalid values. Mitigation: Treat past-due TBD targets as single-month amounts. Unrecognized goal types are excluded entirely.
- **[Chart.js CDN dependency]** → Requires internet to load the library. Mitigation: Acceptable trade-off for simplicity; can vendor the library later.
