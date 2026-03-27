using System.Text.Json.Serialization;

namespace YnabReports.Models.Ynab;

public class CategoriesResponse
{
    [JsonPropertyName("data")]
    public CategoriesData Data { get; set; } = new();
}

public class CategoriesData
{
    [JsonPropertyName("category_groups")]
    public List<CategoryGroupWithCategories> CategoryGroups { get; set; } = [];
}

public class CategoryGroupWithCategories
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; } = [];
}

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("category_group_id")]
    public string CategoryGroupId { get; set; } = string.Empty;

    [JsonPropertyName("category_group_name")]
    public string CategoryGroupName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("budgeted")]
    public long Budgeted { get; set; }

    [JsonPropertyName("goal_type")]
    public string? GoalType { get; set; }

    [JsonPropertyName("goal_target")]
    public long? GoalTarget { get; set; }

    [JsonPropertyName("goal_target_month")]
    public string? GoalTargetMonth { get; set; }

    [JsonPropertyName("goal_target_date")]
    public string? GoalTargetDate { get; set; }

    [JsonPropertyName("goal_percentage_complete")]
    public int? GoalPercentageComplete { get; set; }

    [JsonPropertyName("goal_months_to_budget")]
    public int? GoalMonthsToBudget { get; set; }

    [JsonPropertyName("goal_cadence")]
    public int? GoalCadence { get; set; }

    [JsonPropertyName("goal_cadence_frequency")]
    public int? GoalCadenceFrequency { get; set; }

    [JsonPropertyName("activity")]
    public long Activity { get; set; }

    [JsonPropertyName("balance")]
    public long Balance { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}
