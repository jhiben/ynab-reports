namespace YnabReports.Models.ViewModels;

public class ActualVsTargetReportViewModel
{
    public string BudgetName { get; set; } = string.Empty;
    public string BudgetId { get; set; } = string.Empty;
    public string SelectedMonth { get; set; } = string.Empty;
    public string SelectedMonthDisplay { get; set; } = string.Empty;
    public List<BudgetOption> Budgets { get; set; } = [];
    public List<string> AvailableMonths { get; set; } = [];
    public List<ActualVsTargetItem> Items { get; set; } = [];
    public decimal TotalTarget { get; set; }
    public decimal TotalActual { get; set; }
    public decimal TotalDifference => TotalTarget - TotalActual;
    public bool HasData => Items.Count > 0;
}

public class ActualVsTargetItem
{
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Difference => TargetAmount - ActualAmount;
    public decimal DifferencePercent => TargetAmount != 0
        ? Math.Round(Difference / TargetAmount * 100m, 1)
        : 0;
    public string Status => Difference >= 0 ? "under" : "over";
    public string Color { get; set; } = string.Empty;
}
