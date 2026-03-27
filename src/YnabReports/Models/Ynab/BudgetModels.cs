using System.Text.Json.Serialization;

namespace YnabReports.Models.Ynab;

public class BudgetSummaryResponse
{
    [JsonPropertyName("data")]
    public BudgetSummaryData Data { get; set; } = new();
}

public class BudgetSummaryData
{
    [JsonPropertyName("budgets")]
    public List<BudgetSummary> Budgets { get; set; } = [];
}

public class BudgetSummary
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
