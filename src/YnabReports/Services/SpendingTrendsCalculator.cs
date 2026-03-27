using Microsoft.Extensions.Options;
using YnabReports.Configuration;
using YnabReports.Models.Ynab;
using YnabReports.Models.ViewModels;

namespace YnabReports.Services;

public class SpendingTrendsCalculator
{
    private readonly IOptionsMonitor<ReportOptions> _options;

    private static readonly string[] ChartColors =
    [
        "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
        "#FF9F40", "#C9CBCF", "#7BC67E", "#E77C8E", "#55BFC7",
        "#DBAD6A", "#6C8EBF", "#D4A5A5", "#9ED2C6", "#FFB6C1",
        "#B0E0E6", "#DDA0DD", "#F0E68C", "#98D8C8", "#F7DC6F"
    ];

    public SpendingTrendsCalculator(IOptionsMonitor<ReportOptions> options)
    {
        _options = options;
    }

    /// <summary>
    /// Builds spending trend data from multiple months' category data.
    /// </summary>
    public SpendingTrendsReportViewModel Calculate(
        List<CategoryGroupWithCategories> groups,
        List<BudgetMonthDetail> monthDetails,
        bool groupByCategory)
    {
        var options = _options.CurrentValue;
        var excludedGroups = new HashSet<string>(options.ExcludedCategoryGroups, StringComparer.OrdinalIgnoreCase);
        var excludedCategories = new HashSet<string>(options.ExcludedCategories, StringComparer.OrdinalIgnoreCase);

        // Build group membership lookup from categories data
        var categoryToGroup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in groups)
        {
            if (excludedGroups.Contains(group.Name)) continue;
            foreach (var cat in group.Categories)
            {
                if (!excludedCategories.Contains(cat.Name))
                    categoryToGroup[cat.Id] = group.Name;
            }
        }

        var months = monthDetails
            .OrderBy(m => m.Month)
            .ToList();

        var monthLabels = months.Select(m => m.Month[..7]).ToList(); // "YYYY-MM"

        // Aggregate spending per entity (category or group) per month
        var spendingData = new Dictionary<string, List<decimal>>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < months.Count; i++)
        {
            var monthCategories = months[i].Categories
                .Where(c => !excludedCategories.Contains(c.Name))
                .Where(c => categoryToGroup.ContainsKey(c.Id));

            if (groupByCategory)
            {
                // Group by category group
                var grouped = monthCategories
                    .GroupBy(c => categoryToGroup.GetValueOrDefault(c.Id, "Other"))
                    .Where(g => !excludedGroups.Contains(g.Key));

                foreach (var group in grouped)
                {
                    if (!spendingData.ContainsKey(group.Key))
                        spendingData[group.Key] = Enumerable.Repeat(0m, months.Count).ToList();

                    spendingData[group.Key][i] = Math.Abs(group.Sum(c => c.Activity)) / 1000m;
                }
            }
            else
            {
                foreach (var cat in monthCategories)
                {
                    if (!spendingData.ContainsKey(cat.Name))
                        spendingData[cat.Name] = Enumerable.Repeat(0m, months.Count).ToList();

                    spendingData[cat.Name][i] = Math.Abs(cat.Activity) / 1000m;
                }
            }
        }

        // Build items sorted by average spending
        var items = spendingData
            .Where(kvp => kvp.Value.Any(v => v > 0))
            .OrderByDescending(kvp => kvp.Value.Average())
            .Take(15) // Limit to top 15 for chart readability
            .Select((kvp, index) =>
            {
                var amounts = kvp.Value;
                return new SpendingTrendItem
                {
                    Name = kvp.Key,
                    Color = ChartColors[index % ChartColors.Length],
                    MonthlyAmounts = amounts,
                    Trend = CalculateTrend(amounts)
                };
            })
            .ToList();

        return new SpendingTrendsReportViewModel
        {
            Months = monthLabels,
            Items = items,
            GroupByCategory = groupByCategory
        };
    }

    /// <summary>
    /// Simple trend calculation: percentage change from first half average to second half average.
    /// </summary>
    private static decimal CalculateTrend(List<decimal> amounts)
    {
        if (amounts.Count < 2) return 0;

        int mid = amounts.Count / 2;
        var firstHalf = amounts.Take(mid).Where(a => a > 0).ToList();
        var secondHalf = amounts.Skip(mid).Where(a => a > 0).ToList();

        if (firstHalf.Count == 0 || secondHalf.Count == 0) return 0;

        var firstAvg = firstHalf.Average();
        var secondAvg = secondHalf.Average();

        return firstAvg > 0 ? Math.Round((secondAvg - firstAvg) / firstAvg * 100m, 1) : 0;
    }
}
