using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YnabReports.Models.ViewModels;
using YnabReports.Services;

namespace YnabReports.Pages.Reports;

public class TargetProportionsModel : PageModel
{
    private readonly IYnabApiClient _ynabClient;
    private readonly TargetProportionCalculator _calculator;

    public TargetProportionReportViewModel Report { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? BudgetId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool GroupByCategory { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CategoryGroup { get; set; }

    public TargetProportionsModel(IYnabApiClient ynabClient, TargetProportionCalculator calculator)
    {
        _ynabClient = ynabClient;
        _calculator = calculator;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var budgets = await _ynabClient.GetBudgetsAsync();
        Report.Budgets = budgets.Select(b => new BudgetOption { Id = b.Id, Name = b.Name }).ToList();

        if (Report.Budgets.Count == 0)
        {
            return Page();
        }

        // Use the first budget if none is selected
        if (string.IsNullOrEmpty(BudgetId))
        {
            BudgetId = Report.Budgets[0].Id;
        }

        Report.BudgetId = BudgetId;
        Report.BudgetName = Report.Budgets.FirstOrDefault(b => b.Id == BudgetId)?.Name ?? "Unknown";
        Report.GroupedByCategory = GroupByCategory;
        Report.CategoryGroupName = CategoryGroup;

        var categoryGroups = await _ynabClient.GetCategoriesAsync(BudgetId);

        Report.Items = !string.IsNullOrEmpty(CategoryGroup)
            ? _calculator.CalculateSubcategoryProportionsForGroup(categoryGroups, CategoryGroup)
            : GroupByCategory
                ? _calculator.CalculateCategoryGroupProportions(categoryGroups)
                : _calculator.CalculateSubcategoryProportions(categoryGroups);

        Report.TotalTarget = Report.Items.Sum(i => i.TargetAmount);
        Report.TotalHoursOfLife = Report.Items.Sum(i => i.HoursOfLife);

        return Page();
    }
}
