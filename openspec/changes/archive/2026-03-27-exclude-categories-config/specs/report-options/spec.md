## ADDED Requirements

### Requirement: Report options configuration
The system SHALL read a `ReportOptions` configuration section from `appsettings.json` containing exclusion lists for category groups and individual categories.

#### Scenario: Configuration section present with exclusions
- **WHEN** `appsettings.json` contains a `ReportOptions` section with `ExcludedCategoryGroups` and/or `ExcludedCategories` arrays
- **THEN** the system SHALL bind those arrays into a `ReportOptions` object available via dependency injection

#### Scenario: Configuration section absent
- **WHEN** `appsettings.json` does not contain a `ReportOptions` section
- **THEN** the system SHALL use empty exclusion lists as the default (no categories excluded)

### Requirement: Exclusion matching is case-insensitive
The system SHALL compare configured exclusion names against YNAB category and category group names using case-insensitive ordinal comparison.

#### Scenario: Case mismatch between config and YNAB
- **WHEN** the config excludes "credit card payments" and YNAB has a group named "Credit Card Payments"
- **THEN** the system SHALL treat the group as excluded
