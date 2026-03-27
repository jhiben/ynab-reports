## MODIFIED Requirements

### Requirement: Display proportional breakdown of subcategory targets
The system SHALL display a report page that shows each subcategory's **monthly normalized** target amount as a proportion of the total monthly target across all subcategories in the selected budget, **excluding any categories or category groups listed in the configured exclusion lists**.

#### Scenario: Budget with multiple subcategories having targets
- **WHEN** the user navigates to the target proportion report page and selects a budget containing subcategories with targets
- **THEN** the system SHALL display a donut chart where each segment represents a subcategory and its size corresponds to that subcategory's **normalized monthly** target as a percentage of the total

#### Scenario: Budget with no subcategory targets
- **WHEN** the selected budget has no subcategories with a target set (after applying exclusions)
- **THEN** the system SHALL display a message indicating no target data is available instead of an empty chart

#### Scenario: Excluded category group removes all its subcategories
- **WHEN** a category group name appears in `ExcludedCategoryGroups`
- **THEN** the system SHALL exclude all subcategories belonging to that group from the chart, table, and total calculation

#### Scenario: Excluded individual category
- **WHEN** a category name appears in `ExcludedCategories`
- **THEN** the system SHALL exclude only that specific category from the chart, table, and total calculation, leaving other categories in the same group unaffected

#### Scenario: No exclusions configured
- **WHEN** the exclusion lists are empty or not configured
- **THEN** the system SHALL display all categories with targets (current behavior)
