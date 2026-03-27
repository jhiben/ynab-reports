using System.Globalization;
using YnabReports.Models.Ynab;
using YnabReports.Models.ViewModels;

namespace YnabReports.Services;

public class TargetProportionCalculator
{
    private static readonly string[] ChartColors =
    [
        "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
        "#FF9F40", "#C9CBCF", "#7BC67E", "#E77C8E", "#55BFC7",
        "#DBAD6A", "#6C8EBF", "#D4A5A5", "#9ED2C6", "#FFB6C1",
        "#B0E0E6", "#DDA0DD", "#F0E68C", "#98D8C8", "#F7DC6F"
    ];

    /// <summary>
    /// Normalizes a category's goal target to a monthly amount (in milliunits).
    /// Returns null if the category cannot be normalized (unknown goal type, no target).
    /// </summary>
    public static long? NormalizeToMonthly(Category category)
    {
        if (category.GoalType is null || !category.GoalTarget.HasValue || category.GoalTarget.Value <= 0)
            return null;

        var target = category.GoalTarget.Value;

        return category.GoalType switch
        {
            "MF" or "NEED" or "DEBT" => NormalizeByCadence(target, category.GoalCadence, category.GoalCadenceFrequency),
            "TB" => NormalizeByCadence(target, category.GoalCadence, category.GoalCadenceFrequency),
            "TBD" => NormalizeTbd(target, category.GoalTargetDate ?? category.GoalTargetMonth),
            _ => null
        };
    }

    /// <summary>
    /// Converts a per-period target to a monthly amount based on the goal cadence.
    /// Cadence values (from YNAB API):
    ///   0 = None, 1 = Monthly, 2 = Weekly, 13 = Yearly  (× goal_cadence_frequency)
    ///   3 = Every 2 Months, 4 = Every 3 Months, ..., 12 = Every 11 Months, 14 = Every 2 Years
    /// </summary>
    private static long NormalizeByCadence(long target, int? cadence, int? cadenceFrequency)
    {
        int freq = Math.Max(1, cadenceFrequency ?? 1);

        return cadence switch
        {
            null or 0 => target,
            1 => target / freq,                          // Monthly × frequency
            2 => target * 52 / (12 * freq),              // Weekly × frequency → monthly
            >= 3 and <= 12 => target / (cadence.Value - 1), // Every N months (3→2mo, 4→3mo, …, 12→11mo)
            13 => target / (12 * freq),                  // Yearly × frequency
            14 => target / 24,                           // Every 2 years
            _ => target
        };
    }

    private static long NormalizeTbd(long target, string? goalTargetDate)
    {
        if (goalTargetDate is null)
            return target;

        // YNAB returns goal_target_date as "YYYY-MM-DD"
        if (!DateOnly.TryParseExact(goalTargetDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var targetDate))
            return target;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int monthsRemaining = (targetDate.Year - today.Year) * 12 + (targetDate.Month - today.Month);

        // Past-due or current month: treat as single-month amount
        return monthsRemaining <= 1 ? target : target / monthsRemaining;
    }

    /// <summary>
    /// Builds subcategory-level proportion items from category groups.
    /// Normalizes all targets to monthly amounts before computing proportions.
    /// </summary>
    public List<TargetProportionItem> CalculateSubcategoryProportions(
        List<CategoryGroupWithCategories> groups)
    {
        var items = groups
            .SelectMany(g => g.Categories)
            .Select(c => (c.Name, Monthly: NormalizeToMonthly(c)))
            .Where(x => x.Monthly.HasValue)
            .Select(x => (x.Name, Total: x.Monthly!.Value));

        return BuildItems(items);
    }

    /// <summary>
    /// Builds category-group-level proportion items by summing normalized monthly
    /// subcategory targets per group.
    /// </summary>
    public List<TargetProportionItem> CalculateCategoryGroupProportions(
        List<CategoryGroupWithCategories> groups)
    {
        var groupTotals = groups
            .Select(g => (
                Name: g.Name,
                Total: g.Categories
                    .Select(c => NormalizeToMonthly(c))
                    .Where(m => m.HasValue)
                    .Sum(m => m!.Value)
            ))
            .Where(g => g.Total > 0);

        return BuildItems(groupTotals);
    }

    private static List<TargetProportionItem> BuildItems(IEnumerable<(string Name, long Total)> items)
    {
        var list = items.ToList();

        // YNAB amounts are in milliunits (amount * 1000)
        decimal total = list.Sum(i => i.Total) / 1000m;
        if (total == 0) return [];

        return list
            .OrderByDescending(i => i.Total)
            .Select((item, index) => new TargetProportionItem
            {
                Name = item.Name,
                TargetAmount = item.Total / 1000m,
                Percentage = Math.Round(item.Total / 1000m / total * 100m, 1),
                Color = ChartColors[index % ChartColors.Length]
            })
            .ToList();
    }
}
