using YnabReports.Models.Ynab;

namespace YnabReports.Services;

public interface IYnabApiClient
{
    Task<List<BudgetSummary>> GetBudgetsAsync();
    Task<List<CategoryGroupWithCategories>> GetCategoriesAsync(string budgetId);
}
