namespace YnabReports.Models.ViewModels;

public class SpendingTrendsReportViewModel
{
    public string BudgetName { get; set; } = string.Empty;
    public string BudgetId { get; set; } = string.Empty;
    public List<BudgetOption> Budgets { get; set; } = [];
    public bool GroupByCategory { get; set; }
    public List<string> Months { get; set; } = [];
    public List<SpendingTrendItem> Items { get; set; } = [];
    public bool HasData => Items.Count > 0;
}

public class SpendingTrendItem
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public List<decimal> MonthlyAmounts { get; set; } = [];
    public decimal Average => MonthlyAmounts.Count > 0 ? Math.Round(MonthlyAmounts.Average(), 2) : 0;
    public decimal Trend { get; set; }
    public string TrendDirection => Trend > 5 ? "rising" : Trend < -5 ? "falling" : "stable";
}
