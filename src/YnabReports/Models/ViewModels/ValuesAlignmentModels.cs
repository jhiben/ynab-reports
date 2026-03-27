namespace YnabReports.Models.ViewModels;

public class ValuesAlignmentReportViewModel
{
    public string BudgetName { get; set; } = string.Empty;
    public string BudgetId { get; set; } = string.Empty;
    public List<BudgetOption> Budgets { get; set; } = [];
    public List<ValueAlignmentItem> Items { get; set; } = [];
    public decimal AlignmentScore { get; set; }
    public decimal TotalMappedSpending { get; set; }
    public decimal TotalSpending { get; set; }
    public decimal UnmappedSpending { get; set; }
    public bool HasData => Items.Count > 0;
    public bool IsConfigured { get; set; }
}

public class ValueAlignmentItem
{
    public string ValueName { get; set; } = string.Empty;
    public int Importance { get; set; }
    public decimal SpendingAmount { get; set; }
    public decimal SpendingPercent { get; set; }
    public decimal ImportancePercent { get; set; }
    public decimal Gap => SpendingPercent - ImportancePercent;
    public string GapDescription => Gap > 5 ? "Overspending" : Gap < -5 ? "Underspending" : "Aligned";
    public List<string> LinkedCategories { get; set; } = [];
}
