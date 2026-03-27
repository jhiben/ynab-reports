namespace YnabReports.Configuration;

public class ReportOptions
{
    public const string SectionName = "ReportOptions";

    public List<string> ExcludedCategoryGroups { get; set; } = [];
    public List<string> ExcludedCategories { get; set; } = [];
    public decimal HourlyRate { get; set; } = 33m;
    public decimal MonthlyIncome { get; set; } = 4760m;
}
