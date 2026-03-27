using System.Net.Http.Json;
using YnabReports.Models.Ynab;

namespace YnabReports.Services;

public class YnabApiClient : IYnabApiClient
{
    private readonly HttpClient _httpClient;

    public YnabApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BudgetSummary>> GetBudgetsAsync()
    {
        var response = await _httpClient.GetAsync("budgets");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BudgetSummaryResponse>();
        return result?.Data.Budgets ?? [];
    }

    public async Task<List<CategoryGroupWithCategories>> GetCategoriesAsync(string budgetId)
    {
        var response = await _httpClient.GetAsync($"budgets/{budgetId}/categories");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CategoriesResponse>();
        var groups = result?.Data.CategoryGroups ?? [];

        // Filter out hidden/deleted groups and categories
        return groups
            .Where(g => !g.Hidden && !g.Deleted)
            .Select(g => new CategoryGroupWithCategories
            {
                Id = g.Id,
                Name = g.Name,
                Hidden = g.Hidden,
                Deleted = g.Deleted,
                Categories = g.Categories
                    .Where(c => !c.Hidden && !c.Deleted)
                    .ToList()
            })
            .ToList();
    }

    public async Task<BudgetMonthDetail> GetBudgetMonthAsync(string budgetId, string month)
    {
        var response = await _httpClient.GetAsync($"budgets/{budgetId}/months/{month}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BudgetMonthDetailResponse>();
        var detail = result?.Data.Month ?? new BudgetMonthDetail();

        detail.Categories = detail.Categories
            .Where(c => !c.Hidden && !c.Deleted)
            .ToList();

        return detail;
    }

    public async Task<List<BudgetMonthSummary>> GetBudgetMonthsAsync(string budgetId)
    {
        var response = await _httpClient.GetAsync($"budgets/{budgetId}/months");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BudgetMonthsResponse>();
        return result?.Data.Months
            .Where(m => !m.Deleted)
            .ToList() ?? [];
    }
}
