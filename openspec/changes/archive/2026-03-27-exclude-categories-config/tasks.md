## 1. Configuration

- [x] 1.1 Create `ReportOptions` class in `Configuration/` with `ExcludedCategoryGroups` (string list) and `ExcludedCategories` (string list) properties, defaulting to empty lists
- [x] 1.2 Add `ReportOptions` section to `appsettings.json` with empty arrays for both exclusion lists
- [x] 1.3 Register `ReportOptions` in DI via `IOptions<ReportOptions>` binding in `Program.cs`

## 2. Filtering Logic

- [x] 2.1 Update `TargetProportionCalculator` to accept `ReportOptions` (via constructor injection) and filter out excluded category groups and categories by name (case-insensitive) before computing proportions
- [x] 2.2 Apply group-level exclusion: remove entire `CategoryGroupWithCategories` entries whose name matches `ExcludedCategoryGroups`
- [x] 2.3 Apply category-level exclusion: remove individual `Category` entries whose name matches `ExcludedCategories`, leaving sibling categories in the same group unaffected

## 3. Verification

- [x] 3.1 Build the project and verify no compilation errors
- [x] 3.2 Verify that empty exclusion lists produce unchanged behavior (no categories filtered)
