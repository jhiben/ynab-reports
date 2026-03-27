## ADDED Requirements

### Requirement: Display proportional breakdown of subcategory targets
The system SHALL display a report page that shows each subcategory's **monthly normalized** target amount as a proportion of the total monthly target across all subcategories in the selected budget.

#### Scenario: Budget with multiple subcategories having targets
- **WHEN** the user navigates to the target proportion report page and selects a budget containing subcategories with targets
- **THEN** the system SHALL display a donut chart where each segment represents a subcategory and its size corresponds to that subcategory's **normalized monthly** target as a percentage of the total

#### Scenario: Budget with no subcategory targets
- **WHEN** the selected budget has no subcategories with a target set
- **THEN** the system SHALL display a message indicating no target data is available instead of an empty chart

### Requirement: Group proportions by category group
The system SHALL allow the user to toggle between a subcategory-level view and a category-group-level view of target proportions.

#### Scenario: Toggle to category group view
- **WHEN** the user selects the "Group by Category" toggle
- **THEN** the chart SHALL aggregate all subcategory targets within each category group and display one segment per group

#### Scenario: Toggle back to subcategory view
- **WHEN** the user selects the "Show Subcategories" toggle
- **THEN** the chart SHALL revert to displaying one segment per subcategory

### Requirement: Normalize targets to monthly amounts
The system SHALL normalize all YNAB goal types to a per-month amount before computing proportions. The normalization rules SHALL be:
- **MF (Monthly Funding)**: use `goal_target` as-is.
- **TBD (Target Balance by Date)**: divide `goal_target` by the number of months remaining until `goal_target_month`. If the target month is in the past or the current month, treat as a single-month amount.
- **TB (Target Balance)**: divide `goal_target` by 12.
- **NEED (Monthly Spending)**: use `goal_target` as-is.
- **DEBT**: use `goal_target` as-is.
- Categories with an unrecognized or null goal type SHALL be excluded.

#### Scenario: Yearly savings goal normalized to monthly
- **WHEN** a subcategory has goal type TBD with a target of $12,000 and 12 months remaining
- **THEN** the system SHALL display a monthly amount of $1,000 for that subcategory

#### Scenario: Monthly funding goal used as-is
- **WHEN** a subcategory has goal type MF with a target of $500
- **THEN** the system SHALL display a monthly amount of $500 for that subcategory

#### Scenario: Target balance goal annualized
- **WHEN** a subcategory has goal type TB with a target of $6,000
- **THEN** the system SHALL display a monthly amount of $500 for that subcategory

#### Scenario: Past-due TBD target treated as single month
- **WHEN** a subcategory has goal type TBD with a target month in the past
- **THEN** the system SHALL treat the full target as a single-month amount

### Requirement: Display summary table alongside chart
The system SHALL display a table next to the chart listing each item (subcategory or category group, depending on the current view) with its name, target amount, and percentage of the total.

#### Scenario: Summary table content
- **WHEN** the report page loads with valid target data
- **THEN** the summary table SHALL list each item with columns: Name, Monthly Target Amount (formatted as currency), and Percentage of Total (formatted to one decimal place)

#### Scenario: Summary table sorted by proportion
- **WHEN** the summary table is displayed
- **THEN** the items SHALL be sorted in descending order by target amount

### Requirement: Budget selector
The system SHALL provide a dropdown to select which budget to display the report for.

#### Scenario: Multiple budgets available
- **WHEN** the user has more than one budget in YNAB
- **THEN** the system SHALL display a dropdown listing all budgets by name, with the first (or default) budget pre-selected

#### Scenario: Single budget available
- **WHEN** the user has exactly one budget in YNAB
- **THEN** the system SHALL automatically select that budget and still display the dropdown (disabled) showing the budget name

### Requirement: Exclude subcategories without targets
The system SHALL exclude subcategories that have no target/goal set from the proportional calculation and chart display.

#### Scenario: Mixed subcategories with and without targets
- **WHEN** a budget contains some subcategories with targets and some without
- **THEN** the chart and table SHALL only include subcategories that have a target, and the total SHALL be calculated from only those subcategories

### Requirement: Chart interactivity
The chart SHALL support hover tooltips showing the item name, target amount, and percentage.

#### Scenario: Hover over chart segment
- **WHEN** the user hovers over a segment of the donut chart
- **THEN** the system SHALL display a tooltip showing the subcategory (or group) name, its target amount formatted as currency, and its percentage of the total
