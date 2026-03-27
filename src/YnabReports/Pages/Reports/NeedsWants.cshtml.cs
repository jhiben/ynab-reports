using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YnabReports.Models.ViewModels;
using YnabReports.Services;

namespace YnabReports.Pages.Reports;

public class NeedsWantsModel : PageModel
{
    private readonly IYnabApiClient _ynabClient;
    private readonly NeedsWantsCalculator _calculator;

    public NeedsWantsReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? BudgetId { get; set; }

    public NeedsWantsModel(IYnabApiClient ynabClient, NeedsWantsCalculator calculator)
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
        Report.Items = _calculator.Calculate(categoryGroups);
        Report.TotalTarget = Report.Items.Sum(i => i.Amount);

        return Page();
    }
}
