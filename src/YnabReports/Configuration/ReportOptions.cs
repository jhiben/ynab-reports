namespace YnabReports.Configuration;

public class ReportOptions
{
    public const string SectionName = "ReportOptions";

    public List<string> ExcludedCategoryGroups { get; set; } = [];
    public List<string> ExcludedCategories { get; set; } = [];
    public decimal HourlyRate { get; set; } = 33m;
    public decimal MonthlyIncome { get; set; } = 4760m;

    /// <summary>
    /// Maps classification labels (Need, Want, Savings, Debt) to category names.
    /// </summary>
    public Dictionary<string, List<string>> CategoryClassifications { get; set; } = new();

    /// <summary>
    /// Personal values with importance weights and linked categories.
    /// </summary>
    public List<ValueDefinition> Values { get; set; } = [];
}

public class ValueDefinition
{
    public string Name { get; set; } = string.Empty;
    public int Importance { get; set; } = 3;
    public List<string> Categories { get; set; } = [];
}
