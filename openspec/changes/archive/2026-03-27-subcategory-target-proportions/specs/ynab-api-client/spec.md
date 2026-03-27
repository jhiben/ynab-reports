## ADDED Requirements

### Requirement: Authenticate with YNAB API
The system SHALL authenticate with the YNAB API v1 using a personal access token provided via application configuration.

#### Scenario: Valid token configured
- **WHEN** the application starts with a valid YNAB personal access token in configuration
- **THEN** the YNAB API client SHALL include the token as a Bearer token in all API requests

#### Scenario: Missing token
- **WHEN** the application starts without a YNAB personal access token configured
- **THEN** the system SHALL throw a configuration error at startup with a descriptive message

### Requirement: Fetch budget list
The system SHALL retrieve the list of budgets for the authenticated user from the YNAB API endpoint `GET /v1/budgets`.

#### Scenario: Successful budget list retrieval
- **WHEN** the client calls the budget list endpoint
- **THEN** the system SHALL return a collection of `Budget` objects containing at minimum `Id` and `Name` properties

#### Scenario: API error on budget list
- **WHEN** the YNAB API returns a non-success status code
- **THEN** the system SHALL throw an exception containing the HTTP status code and YNAB error detail

### Requirement: Fetch categories for a budget
The system SHALL retrieve all category groups and their categories (subcategories) for a given budget from the YNAB API endpoint `GET /v1/budgets/{budget_id}/categories`.

#### Scenario: Successful category retrieval
- **WHEN** the client requests categories for a valid budget ID
- **THEN** the system SHALL return a collection of `CategoryGroup` objects, each containing a list of `Category` objects with properties: `Id`, `Name`, `Budgeted`, `GoalType`, `GoalTarget`, `GoalTargetMonth`, `GoalPercentageComplete`, and `GoalMonthsToBudget`

#### Scenario: Hidden and deleted categories excluded
- **WHEN** the API response includes categories or category groups marked as hidden or deleted
- **THEN** the system SHALL exclude those items from the returned collection

### Requirement: Data transfer objects mirror YNAB API shape
The system SHALL define DTOs for `Budget`, `CategoryGroup`, `Category`, and their nested target/goal fields that map to the YNAB API v1 JSON response structure.

#### Scenario: Deserialization of API response
- **WHEN** the YNAB API returns a JSON response for budgets or categories
- **THEN** the system SHALL deserialize the response into the corresponding DTO without data loss for the fields listed in the requirements above
