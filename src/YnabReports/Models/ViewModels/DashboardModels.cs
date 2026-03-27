namespace YnabReports.Models.ViewModels;

public class DashboardViewModel
{
    public string BudgetName { get; set; } = string.Empty;
    public decimal TotalMonthlyCommitments { get; set; }
    public decimal HoursOfLifeCommitted { get; set; }
    public decimal PercentOfIncomeCommitted { get; set; }
    public decimal UnallocatedIncome { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal HourlyRate { get; set; }
    public List<DashboardTopExpense> TopExpenses { get; set; } = [];
    public bool HasData => TopExpenses.Count > 0;
}

public class DashboardTopExpense
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal HoursOfLife { get; set; }
    public decimal PercentOfIncome { get; set; }
    public string Color { get; set; } = string.Empty;
}
