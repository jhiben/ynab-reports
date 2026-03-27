namespace YnabReports.Models.ViewModels;

public class TargetProportionItem
{
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal HoursOfLife { get; set; }
    public decimal PercentageOfIncome { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class TargetProportionReportViewModel
{
    public string BudgetName { get; set; } = string.Empty;
    public string BudgetId { get; set; } = string.Empty;
    public List<TargetProportionItem> Items { get; set; } = [];
    public decimal TotalTarget { get; set; }
    public decimal TotalHoursOfLife { get; set; }
    public bool GroupedByCategory { get; set; }
    public string? CategoryGroupName { get; set; }
    public bool IsDrilledDown => !string.IsNullOrEmpty(CategoryGroupName);
    public List<BudgetOption> Budgets { get; set; } = [];
    public bool HasData => Items.Count > 0;
}

public class BudgetOption
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
