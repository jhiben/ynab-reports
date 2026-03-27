using Microsoft.Extensions.Options;
using YnabReports.Configuration;
using YnabReports.Models.Ynab;
using YnabReports.Models.ViewModels;

namespace YnabReports.Services;

public class ValuesAlignmentCalculator
{
    private readonly IOptionsMonitor<ReportOptions> _options;

    public ValuesAlignmentCalculator(IOptionsMonitor<ReportOptions> options)
    {
        _options = options;
    }

    public bool IsConfigured()
    {
        return _options.CurrentValue.Values.Any(v => v.Categories.Count > 0);
    }

    public ValuesAlignmentReportViewModel Calculate(List<CategoryGroupWithCategories> groups)
    {
        var options = _options.CurrentValue;
        var values = options.Values;

        // Normalize all categories to monthly targets
        var categoryTargets = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in groups)
        {
            foreach (var cat in group.Categories)
            {
                var monthly = TargetProportionCalculator.NormalizeToMonthly(cat);
                if (monthly.HasValue && monthly.Value > 0)
                    categoryTargets[cat.Name] = monthly.Value / 1000m;
            }
        }

        var totalSpending = categoryTargets.Values.Sum();
        if (totalSpending == 0 || values.Count == 0)
            return new ValuesAlignmentReportViewModel { IsConfigured = values.Count > 0 };

        // Calculate spending per value
        var totalImportance = values.Sum(v => v.Importance);
        var mappedTotal = 0m;
        var items = new List<ValueAlignmentItem>();

        foreach (var value in values)
        {
            var spending = value.Categories
                .Where(c => categoryTargets.ContainsKey(c))
                .Sum(c => categoryTargets[c]);

            mappedTotal += spending;

            var spendingPercent = Math.Round(spending / totalSpending * 100m, 1);
            var importancePercent = totalImportance > 0
                ? Math.Round((decimal)value.Importance / totalImportance * 100m, 1)
                : 0;

            items.Add(new ValueAlignmentItem
            {
                ValueName = value.Name,
                Importance = value.Importance,
                SpendingAmount = spending,
                SpendingPercent = spendingPercent,
                ImportancePercent = importancePercent,
                LinkedCategories = value.Categories
            });
        }

        // Alignment score: 100 minus average absolute gap
        var avgGap = items.Count > 0
            ? items.Average(i => Math.Abs(i.Gap))
            : 0;
        var alignmentScore = Math.Max(0, Math.Round(100m - avgGap * 2, 0));

        return new ValuesAlignmentReportViewModel
        {
            Items = items,
            AlignmentScore = alignmentScore,
            TotalSpending = totalSpending,
            TotalMappedSpending = mappedTotal,
            UnmappedSpending = totalSpending - mappedTotal,
            IsConfigured = true
        };
    }
}
