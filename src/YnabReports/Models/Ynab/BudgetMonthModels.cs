using System.Text.Json.Serialization;

namespace YnabReports.Models.Ynab;

public class BudgetMonthsResponse
{
    [JsonPropertyName("data")]
    public BudgetMonthsData Data { get; set; } = new();
}

public class BudgetMonthsData
{
    [JsonPropertyName("months")]
    public List<BudgetMonthSummary> Months { get; set; } = [];
}

public class BudgetMonthSummary
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = string.Empty;

    [JsonPropertyName("income")]
    public long Income { get; set; }

    [JsonPropertyName("budgeted")]
    public long Budgeted { get; set; }

    [JsonPropertyName("activity")]
    public long Activity { get; set; }

    [JsonPropertyName("to_be_budgeted")]
    public long ToBeBudgeted { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}

public class BudgetMonthDetailResponse
{
    [JsonPropertyName("data")]
    public BudgetMonthDetailData Data { get; set; } = new();
}

public class BudgetMonthDetailData
{
    [JsonPropertyName("month")]
    public BudgetMonthDetail Month { get; set; } = new();
}

public class BudgetMonthDetail
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = string.Empty;

    [JsonPropertyName("income")]
    public long Income { get; set; }

    [JsonPropertyName("budgeted")]
    public long Budgeted { get; set; }

    [JsonPropertyName("activity")]
    public long Activity { get; set; }

    [JsonPropertyName("to_be_budgeted")]
    public long ToBeBudgeted { get; set; }

    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; } = [];
}
