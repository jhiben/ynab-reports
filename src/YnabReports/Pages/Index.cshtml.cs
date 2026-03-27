using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using YnabReports.Configuration;
using YnabReports.Models.ViewModels;
using YnabReports.Services;

namespace YnabReports.Pages;

public class IndexModel : PageModel
{
    private readonly IYnabApiClient _ynabClient;
    private readonly TargetProportionCalculator _calculator;
    private readonly IOptionsMonitor<ReportOptions> _options;

    public DashboardViewModel Dashboard { get; set; } = new();

    public IndexModel(
        IYnabApiClient ynabClient,
        TargetProportionCalculator calculator,
        IOptionsMonitor<ReportOptions> options)
    {
        _ynabClient = ynabClient;
        _calculator = calculator;
        _options = options;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var budgets = await _ynabClient.GetBudgetsAsync();
        if (budgets.Count == 0) return Page();

        var budget = budgets[0];
        Dashboard.BudgetName = budget.Name;

        var options = _options.CurrentValue;
        Dashboard.MonthlyIncome = options.MonthlyIncome;
        Dashboard.HourlyRate = options.HourlyRate;

        var categoryGroups = await _ynabClient.GetCategoriesAsync(budget.Id);
        var items = _calculator.CalculateSubcategoryProportions(categoryGroups);

        var totalTarget = items.Sum(i => i.TargetAmount);
        Dashboard.TotalMonthlyCommitments = totalTarget;
        Dashboard.HoursOfLifeCommitted = options.HourlyRate > 0
            ? Math.Round(totalTarget / options.HourlyRate, 1) : 0;
        Dashboard.PercentOfIncomeCommitted = options.MonthlyIncome > 0
            ? Math.Round(totalTarget / options.MonthlyIncome * 100m, 1) : 0;
        Dashboard.UnallocatedIncome = options.MonthlyIncome - totalTarget;

        Dashboard.TopExpenses = items
            .OrderByDescending(i => i.TargetAmount)
            .Take(7)
            .Select(i => new DashboardTopExpense
            {
                Name = i.Name,
                Amount = i.TargetAmount,
                HoursOfLife = i.HoursOfLife,
                PercentOfIncome = i.PercentageOfIncome,
                Color = i.Color
            })
            .ToList();

        return Page();
    }
}
