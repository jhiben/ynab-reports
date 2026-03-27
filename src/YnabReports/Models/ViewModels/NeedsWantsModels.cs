namespace YnabReports.Models.ViewModels;

public class NeedsWantsReportViewModel
{
    public string BudgetName { get; set; } = string.Empty;
    public string BudgetId { get; set; } = string.Empty;
    public List<BudgetOption> Budgets { get; set; } = [];
    public List<ClassificationItem> Items { get; set; } = [];
    public decimal TotalTarget { get; set; }
    public bool HasData => Items.Count > 0;
    public bool IsConfigured { get; set; }

    // 50/30/20 benchmark
    public decimal NeedsPercent => GetPercent("Need");
    public decimal WantsPercent => GetPercent("Want");
    public decimal SavingsDebtPercent => GetPercent("Savings") + GetPercent("Debt");

    private decimal GetPercent(string classification)
    {
        if (TotalTarget == 0) return 0;
        return Items.Where(i => i.Classification == classification).Sum(i => i.Amount) / TotalTarget * 100m;
    }
}

public class ClassificationItem
{
    public string Classification { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
    public List<ClassificationCategoryDetail> Categories { get; set; } = [];
}

public class ClassificationCategoryDetail
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
