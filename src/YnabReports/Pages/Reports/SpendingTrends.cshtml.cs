using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YnabReports.Models.ViewModels;
using YnabReports.Services;

namespace YnabReports.Pages.Reports;

public class SpendingTrendsModel : PageModel
{
    private readonly IYnabApiClient _ynabClient;
    private readonly SpendingTrendsCalculator _calculator;

    public SpendingTrendsReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? BudgetId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool GroupByCategory { get; set; } = true;

    public SpendingTrendsModel(IYnabApiClient ynabClient, SpendingTrendsCalculator calculator)
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

        // Get last 6 months
        var monthSummaries = await _ynabClient.GetBudgetMonthsAsync(BudgetId);
        var recentMonths = monthSummaries
            .OrderByDescending(m => m.Month)
            .Take(6)
            .OrderBy(m => m.Month)
            .ToList();

        if (recentMonths.Count == 0) return Page();

        // Get category groups for group membership
        var categoryGroups = await _ynabClient.GetCategoriesAsync(BudgetId);

        // Fetch each month's detail
        var monthDetails = new List<Models.Ynab.BudgetMonthDetail>();
        foreach (var m in recentMonths)
        {
            var detail = await _ynabClient.GetBudgetMonthAsync(BudgetId, m.Month);
            monthDetails.Add(detail);
        }

        Report = _calculator.Calculate(categoryGroups, monthDetails, GroupByCategory);
        Report.BudgetId = BudgetId;
        Report.BudgetName = budgets.FirstOrDefault(b => b.Id == BudgetId)?.Name ?? "Unknown";
        Report.Budgets = budgets.Select(b => new BudgetOption { Id = b.Id, Name = b.Name }).ToList();
        Report.GroupByCategory = GroupByCategory;

        return Page();
    }
}
