using Microsoft.Extensions.Options;
using YnabReports.Configuration;
using YnabReports.Models.Ynab;
using YnabReports.Models.ViewModels;

namespace YnabReports.Services;

public class ActualVsTargetCalculator
{
    private readonly IOptionsMonitor<ReportOptions> _options;

    private static readonly string[] ChartColors =
    [
        "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
        "#FF9F40", "#C9CBCF", "#7BC67E", "#E77C8E", "#55BFC7",
        "#DBAD6A", "#6C8EBF", "#D4A5A5", "#9ED2C6", "#FFB6C1",
        "#B0E0E6", "#DDA0DD", "#F0E68C", "#98D8C8", "#F7DC6F"
    ];

    public ActualVsTargetCalculator(IOptionsMonitor<ReportOptions> options)
    {
        _options = options;
    }

    /// <summary>
    /// Compares monthly targets to actual spending for a given month's category data.
    /// Categories come from the month detail endpoint with populated Activity fields.
    /// Groups data is used for target normalization (goal metadata).
    /// </summary>
    public List<ActualVsTargetItem> Calculate(
        List<CategoryGroupWithCategories> groups,
        BudgetMonthDetail monthDetail)
    {
        var options = _options.CurrentValue;
        var excludedGroups = new HashSet<string>(options.ExcludedCategoryGroups, StringComparer.OrdinalIgnoreCase);
        var excludedCategories = new HashSet<string>(options.ExcludedCategories, StringComparer.OrdinalIgnoreCase);

        // Build a lookup from category ID to its normalized monthly target
        var targetLookup = new Dictionary<string, long>();
        foreach (var group in groups)
        {
            if (excludedGroups.Contains(group.Name)) continue;
            foreach (var cat in group.Categories)
            {
                if (excludedCategories.Contains(cat.Name)) continue;
                var monthly = TargetProportionCalculator.NormalizeToMonthly(cat);
                if (monthly.HasValue && monthly.Value > 0)
                    targetLookup[cat.Id] = monthly.Value;
            }
        }

        // Match month categories with targets
        var items = monthDetail.Categories
            .Where(c => !excludedCategories.Contains(c.Name))
            .Where(c => targetLookup.ContainsKey(c.Id) || c.Activity != 0)
            .Select(c =>
            {
                var target = targetLookup.GetValueOrDefault(c.Id, 0) / 1000m;
                // Activity is negative for outflows in YNAB
                var actual = Math.Abs(c.Activity) / 1000m;
                return (c.Name, Target: target, Actual: actual);
            })
            .Where(x => x.Target > 0 || x.Actual > 0)
            .OrderByDescending(x => Math.Max(x.Target, x.Actual))
            .ToList();

        return items
            .Select((item, index) => new ActualVsTargetItem
            {
                Name = item.Name,
                TargetAmount = item.Target,
                ActualAmount = item.Actual,
                Color = ChartColors[index % ChartColors.Length]
            })
            .ToList();
    }
}
