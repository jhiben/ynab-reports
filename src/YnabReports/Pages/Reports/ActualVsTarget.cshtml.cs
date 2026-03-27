using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YnabReports.Models.ViewModels;
using YnabReports.Services;

namespace YnabReports.Pages.Reports;

public class ActualVsTargetModel : PageModel
{
    private readonly IYnabApiClient _ynabClient;
    private readonly ActualVsTargetCalculator _calculator;

    public ActualVsTargetReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? BudgetId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Month { get; set; }

    public ActualVsTargetModel(IYnabApiClient ynabClient, ActualVsTargetCalculator calculator)
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

        // Get available months
        var months = await _ynabClient.GetBudgetMonthsAsync(BudgetId);
        Report.AvailableMonths = months
            .Select(m => m.Month)
            .OrderByDescending(m => m)
            .Take(12)
            .ToList();

        if (Report.AvailableMonths.Count == 0) return Page();

        // Default to current month
        if (string.IsNullOrEmpty(Month))
        {
            var currentMonth = DateTime.UtcNow.ToString("yyyy-MM-01");
            Month = Report.AvailableMonths.Contains(currentMonth)
                ? currentMonth
                : Report.AvailableMonths[0];
        }

        Report.SelectedMonth = Month;
        if (DateOnly.TryParseExact(Month, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthDate))
            Report.SelectedMonthDisplay = monthDate.ToString("MMMM yyyy");
        else
            Report.SelectedMonthDisplay = Month;

        // Get category targets and month actuals
        var categoryGroups = await _ynabClient.GetCategoriesAsync(BudgetId);
        var monthDetail = await _ynabClient.GetBudgetMonthAsync(BudgetId, Month);

        Report.Items = _calculator.Calculate(categoryGroups, monthDetail);
        Report.TotalTarget = Report.Items.Sum(i => i.TargetAmount);
        Report.TotalActual = Report.Items.Sum(i => i.ActualAmount);

        return Page();
    }
}
