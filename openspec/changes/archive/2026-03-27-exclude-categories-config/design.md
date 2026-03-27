## Context

The target proportion report currently displays all non-hidden, non-deleted categories that have a target set. Some category groups (e.g., "Internal Master Category", "Credit Card Payments") or individual categories are not useful in a proportional budget view. Users need a configuration-driven way to exclude them without modifying code.

The existing `YnabApiOptions` handles API settings. A separate options class will handle report-level settings to keep concerns separated.

## Goals / Non-Goals

**Goals:**
- Allow users to exclude specific category groups and/or individual categories by name via `appsettings.json`.
- Apply exclusions before proportion calculation so excluded items never affect totals or percentages.
- Keep the feature non-breaking — empty exclusion lists produce the current behavior.

**Non-Goals:**
- UI-based exclusion management (this is config-file only).
- Exclusion by ID (names are more readable and stable from the user's perspective).
- Regex or wildcard pattern matching.

## Decisions

### 1. Configuration via `appsettings.json` section

**Choice**: New `ReportOptions` section with `ExcludedCategoryGroups` and `ExcludedCategories` string arrays.
**Rationale**: Consistent with the existing `YnabApi` section pattern. Overridable via user-secrets or environment variables. No new dependencies.
**Alternative considered**: A separate JSON config file — rejected for added complexity with no benefit.

### 2. Case-insensitive name matching

**Choice**: Compare category/group names using `StringComparer.OrdinalIgnoreCase`.
**Rationale**: Users may not remember exact casing. YNAB category names are free-text and casing varies.

### 3. Filter in the calculator, not the API client

**Choice**: Apply exclusions in `TargetProportionCalculator` rather than in `YnabApiClient`.
**Rationale**: The API client should return raw data faithfully. Filtering is a report concern. Other future reports may want different exclusion rules or no exclusions at all.

## Risks / Trade-offs

- **[Name changes in YNAB]** → If a user renames a category in YNAB, the config exclusion stops matching. Mitigation: Acceptable for a config-driven feature; user updates the config.
- **[Typos in config]** → A misspelled name silently does nothing. Mitigation: Log a warning at startup or page load if an excluded name matches zero categories. Could be added later.
