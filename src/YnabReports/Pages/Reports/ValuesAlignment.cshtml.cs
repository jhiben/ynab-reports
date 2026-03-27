using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YnabReports.Models.ViewModels;
using YnabReports.Services;

namespace YnabReports.Pages.Reports;

public class ValuesAlignmentModel : PageModel
{
    private readonly IYnabApiClient _ynabClient;
    private readonly ValuesAlignmentCalculator _calculator;

    public ValuesAlignmentReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? BudgetId { get; set; }

    public ValuesAlignmentModel(IYnabApiClient ynabClient, ValuesAlignmentCalculator calculator)
    {
        _ynabClient = ynabClient;
        _calculator = calculator;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var budgets = await _ynabClient.GetBudgetsAsync();
        Report.Budgets = budgets.Select(b => new BudgetOption { Id = b.Id, Name = b.Name }).ToList();

        if (Report.Budgets.Count == 0) return Page();

        if (string.IsNullOrEmpty(BudgetId))
            BudgetId = Report.Budgets[0].Id;

        Report.BudgetId = BudgetId;
        Report.BudgetName = Report.Budgets.FirstOrDefault(b => b.Id == BudgetId)?.Name ?? "Unknown";
        Report.IsConfigured = _calculator.IsConfigured();

        var categoryGroups = await _ynabClient.GetCategoriesAsync(BudgetId);
        var result = _calculator.Calculate(categoryGroups);

        Report.Items = result.Items;
        Report.AlignmentScore = result.AlignmentScore;
        Report.TotalSpending = result.TotalSpending;
        Report.TotalMappedSpending = result.TotalMappedSpending;
        Report.UnmappedSpending = result.UnmappedSpending;
        Report.IsConfigured = result.IsConfigured;

        return Page();
    }
}
