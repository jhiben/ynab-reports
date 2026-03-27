using Microsoft.Extensions.Options;
using YnabReports.Configuration;
using YnabReports.Models.Ynab;
using YnabReports.Models.ViewModels;

namespace YnabReports.Services;

public class NeedsWantsCalculator
{
    private readonly IOptionsMonitor<ReportOptions> _options;

    private static readonly Dictionary<string, string> ClassificationColors = new()
    {
        ["Need"] = "#36A2EB",
        ["Want"] = "#FF6384",
        ["Savings"] = "#4BC0C0",
        ["Debt"] = "#FF9F40",
        ["Uncategorized"] = "#C9CBCF"
    };

    public NeedsWantsCalculator(IOptionsMonitor<ReportOptions> options)
    {
        _options = options;
    }

    public bool IsConfigured()
    {
        var classifications = _options.CurrentValue.CategoryClassifications;
        return classifications.Values.Any(list => list.Count > 0);
    }

    public List<ClassificationItem> Calculate(List<CategoryGroupWithCategories> groups)
    {
        var options = _options.CurrentValue;
        var classifications = options.CategoryClassifications;

        // Build reverse lookup: category name → classification
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (classification, categories) in classifications)
        {
            foreach (var cat in categories)
                lookup[cat] = classification;
        }

        // Normalize all categories to monthly amounts and classify them
        var classified = new Dictionary<string, List<(string Name, decimal Amount)>>();

        foreach (var group in groups)
        {
            foreach (var category in group.Categories)
            {
                var monthly = TargetProportionCalculator.NormalizeToMonthly(category);
                if (!monthly.HasValue || monthly.Value <= 0) continue;

                var amount = monthly.Value / 1000m;
                var classification = lookup.GetValueOrDefault(category.Name, "Uncategorized");

                if (!classified.ContainsKey(classification))
                    classified[classification] = [];

                classified[classification].Add((category.Name, amount));
            }
        }

        var total = classified.Values.SelectMany(v => v).Sum(v => v.Amount);
        if (total == 0) return [];

        return classified
            .OrderByDescending(kvp => kvp.Value.Sum(v => v.Amount))
            .Select(kvp =>
            {
                var groupTotal = kvp.Value.Sum(v => v.Amount);
                return new ClassificationItem
                {
                    Classification = kvp.Key,
                    Amount = groupTotal,
                    Percentage = Math.Round(groupTotal / total * 100m, 1),
                    Color = ClassificationColors.GetValueOrDefault(kvp.Key, "#C9CBCF"),
                    Categories = kvp.Value
                        .OrderByDescending(v => v.Amount)
                        .Select(v => new ClassificationCategoryDetail
                        {
                            Name = v.Name,
                            Amount = v.Amount
                        })
                        .ToList()
                };
            })
            .ToList();
    }
}
