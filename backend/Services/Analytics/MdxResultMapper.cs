using System.Globalization;
using SalesCubeApi.Models;
using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public static class MdxResultMapper
{
    public static decimal? ExtractFirstDecimal(MdxTableResponse table)
    {
        foreach (var row in table.Rows)
        {
            foreach (var value in row.Values)
            {
                if (TryToDecimal(value, out var number))
                {
                    return number;
                }
            }
        }

        return null;
    }

    public static MetricValuesDto ToMetricValues(IReadOnlyDictionary<MeasureKey, decimal?> values)
    {
        values.TryGetValue(MeasureKey.TotalAmount, out var totalAmount);
        values.TryGetValue(MeasureKey.Quantity, out var quantity);
        values.TryGetValue(MeasureKey.Orders, out var orders);
        values.TryGetValue(MeasureKey.SalesLines, out var salesLines);
        values.TryGetValue(MeasureKey.AverageBasket, out var averageBasket);
        values.TryGetValue(MeasureKey.AverageQuantityPerOrder, out var averageQuantityPerOrder);
        values.TryGetValue(MeasureKey.AverageUnitPrice, out var averageUnitPrice);
        values.TryGetValue(MeasureKey.RevenuePerActiveCustomer, out var revenuePerActiveCustomer);
        values.TryGetValue(MeasureKey.AmountInK, out var amountInK);
        values.TryGetValue(MeasureKey.PreviousYearRevenue, out var previousYearRevenue);
        values.TryGetValue(MeasureKey.RevenueGrowthPercent, out var revenueGrowthPercent);
        values.TryGetValue(MeasureKey.YearToDateRevenue, out var yearToDateRevenue);

        return new MetricValuesDto(
            TotalAmount: totalAmount,
            Quantity: quantity,
            Orders: orders,
            SalesLines: salesLines,
            AverageBasket: averageBasket,
            AverageQuantityPerOrder: averageQuantityPerOrder,
            AverageUnitPrice: averageUnitPrice,
            RevenuePerActiveCustomer: revenuePerActiveCustomer,
            AmountInK: amountInK,
            PreviousYearRevenue: previousYearRevenue,
            RevenueGrowthPercent: revenueGrowthPercent,
            YearToDateRevenue: yearToDateRevenue
        );
    }

    public static IReadOnlyList<AnalyticsPointDto> ToAnalyticsPoints(MdxTableResponse table, IReadOnlyList<MeasureKey> measures)
    {
        var points = new List<AnalyticsPointDto>();

        foreach (var row in table.Rows)
        {
            var label = ExtractDeepestLabel(row);
            if (string.IsNullOrWhiteSpace(label))
            {
                continue;
            }

            var uniqueName = ExtractDeepestUniqueName(row);
            var values = ExtractMeasureValues(row, measures);

            points.Add(new AnalyticsPointDto(label!, uniqueName, ToMetricValues(values)));
        }

        return points;
    }

    public static IReadOnlyList<DimensionMemberDto> ToDimensionMembers(MdxTableResponse table)
    {
        var points = ToAnalyticsPoints(table, [MeasureKey.TotalAmount]);
        return points
            .Select(point => new DimensionMemberDto(point.Label, point.UniqueName, point.Metrics.TotalAmount))
            .ToList();
    }

    public static decimal? GetMetricValue(AnalyticsPointDto point, MeasureKey measure)
    {
        return measure switch
        {
            MeasureKey.TotalAmount => point.Metrics.TotalAmount,
            MeasureKey.Quantity => point.Metrics.Quantity,
            MeasureKey.Orders => point.Metrics.Orders,
            MeasureKey.SalesLines => point.Metrics.SalesLines,
            MeasureKey.AverageBasket => point.Metrics.AverageBasket,
            MeasureKey.AverageQuantityPerOrder => point.Metrics.AverageQuantityPerOrder,
            MeasureKey.AverageUnitPrice => point.Metrics.AverageUnitPrice,
            MeasureKey.RevenuePerActiveCustomer => point.Metrics.RevenuePerActiveCustomer,
            MeasureKey.AmountInK => point.Metrics.AmountInK,
            MeasureKey.PreviousYearRevenue => point.Metrics.PreviousYearRevenue,
            MeasureKey.RevenueGrowthPercent => point.Metrics.RevenueGrowthPercent,
            MeasureKey.YearToDateRevenue => point.Metrics.YearToDateRevenue,
            _ => point.Metrics.TotalAmount
        };
    }

    private static Dictionary<MeasureKey, decimal?> ExtractMeasureValues(Dictionary<string, object?> row, IReadOnlyList<MeasureKey> measures)
    {
        var values = new Dictionary<MeasureKey, decimal?>();

        foreach (var measure in measures)
        {
            var uniqueName = MdxDefinition.MeasureUniqueName(measure);
            var caption = MdxDefinition.MeasureCaption(measure);

            var cell = row.FirstOrDefault(x =>
                string.Equals(x.Key, uniqueName, StringComparison.OrdinalIgnoreCase)
                || x.Key.EndsWith(uniqueName, StringComparison.OrdinalIgnoreCase)
                || x.Key.Contains(caption, StringComparison.OrdinalIgnoreCase));

            if (cell.Key is not null && TryToDecimal(cell.Value, out var number))
            {
                values[measure] = number;
            }
            else
            {
                values[measure] = null;
            }
        }

        // Fallback for older ADOMD readers that return anonymous columns.
        // We keep only numeric cells after all member captions/unique names.
        if (values.Values.All(v => v is null))
        {
            var numericValues = row
                .Where(cell => !cell.Key.Contains("MEMBER_CAPTION", StringComparison.OrdinalIgnoreCase)
                               && !cell.Key.Contains("MEMBER_UNIQUE_NAME", StringComparison.OrdinalIgnoreCase))
                .Select(cell => cell.Value)
                .Where(value => TryToDecimal(value, out _))
                .Select(value =>
                {
                    TryToDecimal(value, out var number);
                    return (decimal?)number;
                })
                .ToList();

            for (var i = 0; i < measures.Count; i++)
            {
                values[measures[i]] = i < numericValues.Count ? numericValues[i] : null;
            }
        }

        return values;
    }

    private static string? ExtractDeepestLabel(Dictionary<string, object?> row)
    {
        // A user hierarchy row can contain Category, Brand and Product captions.
        // For chart labels we need the deepest visible member, not the first parent.
        var captionCells = row
            .Where(cell => cell.Key.Contains("MEMBER_CAPTION", StringComparison.OrdinalIgnoreCase))
            .Select(cell => cell.Value?.ToString())
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        if (captionCells.Count > 0)
        {
            return captionCells.Last();
        }

        foreach (var value in row.Values)
        {
            if (value is null || TryToDecimal(value, out _))
            {
                continue;
            }

            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            if (text.StartsWith("[", StringComparison.OrdinalIgnoreCase) && text.Contains("]", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return text;
        }

        return null;
    }

    private static string? ExtractDeepestUniqueName(Dictionary<string, object?> row)
    {
        var uniqueNameCells = row
            .Where(cell => cell.Key.Contains("MEMBER_UNIQUE_NAME", StringComparison.OrdinalIgnoreCase))
            .Select(cell => cell.Value?.ToString())
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        if (uniqueNameCells.Count > 0)
        {
            return uniqueNameCells.Last();
        }

        foreach (var value in row.Values)
        {
            var text = value?.ToString();
            if (!string.IsNullOrWhiteSpace(text) && text.StartsWith("[", StringComparison.OrdinalIgnoreCase) && text.Contains("]", StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }
        }

        return null;
    }

    private static bool TryToDecimal(object? value, out decimal result)
    {
        result = 0m;

        switch (value)
        {
            case null:
                return false;
            case decimal d:
                result = d;
                return true;
            case double d:
                result = Convert.ToDecimal(d, CultureInfo.InvariantCulture);
                return true;
            case float f:
                result = Convert.ToDecimal(f, CultureInfo.InvariantCulture);
                return true;
            case int i:
                result = i;
                return true;
            case long l:
                result = l;
                return true;
            case short s:
                result = s;
                return true;
            default:
                return decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                    || decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }
    }
}
