using System.Globalization;
using System.Text.RegularExpressions;
using SalesCubeApi.Constants;
using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed record AxisDefinition(string Axis, string LevelName, string LevelUniqueName);

public static class MdxDefinition
{
    private static readonly Regex SafeUniqueNameRegex = new(@"^[\p{L}0-9_ .:\[\]\.&\-',%/()]+$", RegexOptions.Compiled);

    public static readonly IReadOnlyList<MeasureKey> DashboardMeasures =
    [
        MeasureKey.TotalAmount,
        MeasureKey.Quantity,
        MeasureKey.Orders,
        MeasureKey.SalesLines,
        MeasureKey.AverageBasket,
        MeasureKey.AverageQuantityPerOrder,
        MeasureKey.AverageUnitPrice,
        MeasureKey.PreviousYearRevenue,
        MeasureKey.RevenueGrowthPercent,
        MeasureKey.YearToDateRevenue
    ];

    public static string MeasureUniqueName(MeasureKey measure) => measure switch
    {
        MeasureKey.TotalAmount => CubeDefinition.Measures.TotalAmount,
        MeasureKey.Quantity => CubeDefinition.Measures.Quantity,
        MeasureKey.Orders => CubeDefinition.Measures.OrderCount,
        MeasureKey.SalesLines => CubeDefinition.Measures.SalesLineCount,
        MeasureKey.AverageBasket => CubeDefinition.Measures.AverageBasket,
        MeasureKey.AverageQuantityPerOrder => CubeDefinition.Measures.AverageQuantityPerOrder,
        MeasureKey.AverageUnitPrice => CubeDefinition.Measures.AverageUnitPrice,
        MeasureKey.RevenuePerActiveCustomer => CubeDefinition.Measures.RevenuePerActiveCustomer,
        MeasureKey.AmountInK => CubeDefinition.Measures.AmountInK,
        MeasureKey.PreviousYearRevenue => CubeDefinition.Measures.PreviousYearRevenue,
        MeasureKey.RevenueGrowthPercent => CubeDefinition.Measures.RevenueGrowthPercent,
        MeasureKey.YearToDateRevenue => CubeDefinition.Measures.YearToDateRevenue,
        _ => CubeDefinition.Measures.TotalAmount
    };

    public static string MeasureCaption(MeasureKey measure) => measure switch
    {
        MeasureKey.TotalAmount => "Total Amount",
        MeasureKey.Quantity => "Order Quantity",
        MeasureKey.Orders => "Nombre totale de commandes",
        MeasureKey.SalesLines => "Nombre de lignes de vente",
        MeasureKey.AverageBasket => "Panier moyen",
        MeasureKey.AverageQuantityPerOrder => "Quantité moyenne par commande",
        MeasureKey.AverageUnitPrice => "Prix moyen unitaire",
        MeasureKey.RevenuePerActiveCustomer => "CA par client actif",
        MeasureKey.AmountInK => "Montant total en K",
        MeasureKey.PreviousYearRevenue => "CA mois précédent",
        MeasureKey.RevenueGrowthPercent => "Evolution CA %",
        MeasureKey.YearToDateRevenue => "CA cumule annee",
        _ => measure.ToString()
    };

    public static string? MeasureFormat(MeasureKey measure) => measure switch
    {
        MeasureKey.RevenueGrowthPercent => "percent",
        MeasureKey.TotalAmount or MeasureKey.AverageBasket or MeasureKey.AverageUnitPrice or MeasureKey.PreviousYearRevenue or MeasureKey.YearToDateRevenue or MeasureKey.AmountInK => "number",
        _ => null
    };

    public static AxisDefinition TimeAxis(TimeGrain grain) => grain switch
    {
        TimeGrain.Year => new("Date", "Year", CubeDefinition.Date.Year),
        TimeGrain.Semester => new("Date", "Semester", CubeDefinition.Date.Semester),
        TimeGrain.Quarter => new("Date", "Quarter", CubeDefinition.Date.Quarter),
        TimeGrain.Month => new("Date", "Month", CubeDefinition.Date.Month),
        TimeGrain.Date => new("Date", "Date", CubeDefinition.Date.DateLevel),
        _ => new("Date", "Month", CubeDefinition.Date.Month)
    };

    public static AxisDefinition ProductAxis(ProductLevel level) => level switch
    {
        ProductLevel.Category => new("Product", "Category", CubeDefinition.Product.Category),
        ProductLevel.Brand => new("Product", "Brand", CubeDefinition.Product.Brand),
        ProductLevel.Product => new("Product", "Product", CubeDefinition.Product.ProductLevel),
        _ => new("Product", "Category", CubeDefinition.Product.Category)
    };

    public static AxisDefinition CustomerAxis(CustomerLevel level) => level switch
    {
        CustomerLevel.Category => new("Customer", "Category", CubeDefinition.Customer.Category),
        CustomerLevel.CustomerType => new("Customer", "Customer Type", CubeDefinition.Customer.CustomerType),
        CustomerLevel.Company => new("Customer", "Company", CubeDefinition.Customer.Company),
        CustomerLevel.Customer => new("Customer", "Customer", CubeDefinition.Customer.CustomerLevel),
        CustomerLevel.DiscountPercent => new("Customer", "Discount Percent", CubeDefinition.Customer.DiscountPercent),
        _ => new("Customer", "Category", CubeDefinition.Customer.Category)
    };

    public static AxisDefinition EmployeeAxis(EmployeeLevel level) => level switch
    {
        EmployeeLevel.Department => new("Employee", "Department", CubeDefinition.Employee.Department),
        EmployeeLevel.JobTitle => new("Employee", "Job Title", CubeDefinition.Employee.JobTitle),
        EmployeeLevel.Employee => new("Employee", "Employee", CubeDefinition.Employee.EmployeeLevel),
        _ => new("Employee", "Department", CubeDefinition.Employee.Department)
    };

    public static AxisDefinition GeographyAxis(GeographyLevel level) => level switch
    {
        GeographyLevel.Country => new("Geography", "Country", CubeDefinition.Geography.Country),
        GeographyLevel.City => new("Geography", "City", CubeDefinition.Geography.City),
        _ => new("Geography", "Country", CubeDefinition.Geography.Country)
    };

    public static AxisDefinition AxisFromDrilldown(SalesDrilldownAxis axis, string? level)
    {
        return axis switch
        {
            SalesDrilldownAxis.Date => TimeAxis(ParseOrDefault(level, TimeGrain.Month)),
            SalesDrilldownAxis.Product => ProductAxis(ParseOrDefault(level, ProductLevel.Category)),
            SalesDrilldownAxis.Customer => CustomerAxis(ParseOrDefault(level, CustomerLevel.Category)),
            SalesDrilldownAxis.Employee => EmployeeAxis(ParseOrDefault(level, EmployeeLevel.Department)),
            SalesDrilldownAxis.Geography => GeographyAxis(ParseOrDefault(level, GeographyLevel.Country)),
            _ => ProductAxis(ProductLevel.Category)
        };
    }

    public static IReadOnlyList<string> BuildFilterMembers(AnalyticsFilterDto? filters)
    {
        if (filters is null)
        {
            return [];
        }

        var result = new List<string>();

        AddMostGranular(result,
            (filters.DateUniqueName, CubeDefinition.Date.Dimension),
            (filters.MonthUniqueName, CubeDefinition.Date.Dimension),
            (filters.QuarterUniqueName, CubeDefinition.Date.Dimension),
            (filters.SemesterUniqueName, CubeDefinition.Date.Dimension),
            (filters.YearUniqueName, CubeDefinition.Date.Dimension));

        AddMostGranular(result,
            (filters.ProductUniqueName, CubeDefinition.Product.Dimension),
            (filters.BrandUniqueName, CubeDefinition.Product.Dimension),
            (filters.ProductCategoryUniqueName, CubeDefinition.Product.Dimension));

        AddMostGranular(result,
            (filters.CustomerUniqueName, CubeDefinition.Customer.Dimension),
            (filters.CompanyUniqueName, CubeDefinition.Customer.Dimension),
            (filters.CustomerTypeUniqueName, CubeDefinition.Customer.Dimension),
            (filters.CustomerCategoryUniqueName, CubeDefinition.Customer.Dimension),
            (filters.DiscountPercentUniqueName, CubeDefinition.Customer.Dimension));

        AddMostGranular(result,
            (filters.EmployeeUniqueName, CubeDefinition.Employee.Dimension),
            (filters.JobTitleUniqueName, CubeDefinition.Employee.Dimension),
            (filters.DepartmentUniqueName, CubeDefinition.Employee.Dimension));

        AddMostGranular(result,
            (filters.CityUniqueName, CubeDefinition.Geography.Dimension),
            (filters.CountryUniqueName, CubeDefinition.Geography.Dimension));

        return result;
    }

    public static string BuildScalarQuery(MeasureKey measure, AnalyticsFilterDto? filters = null)
    {
        return $"SELECT {{ {MeasureUniqueName(measure)} }} ON COLUMNS FROM [{CubeDefinition.CubeName}] {BuildWhereClause(filters)}".Trim();
    }

    public static string BuildMetricsQuery(IReadOnlyList<MeasureKey> measures, AnalyticsFilterDto? filters = null)
    {
        var measuresMdx = string.Join(", ", measures.Select(MeasureUniqueName));
        return $"SELECT {{ {measuresMdx} }} ON COLUMNS FROM [{CubeDefinition.CubeName}] {BuildWhereClause(filters)}".Trim();
    }

    public static string BuildAxisQuery(
        AxisDefinition axis,
        IReadOnlyList<MeasureKey> measures,
        AnalyticsFilterDto? filters = null,
        int? top = null,
        MeasureKey orderByMeasure = MeasureKey.TotalAmount)
    {
        var measuresMdx = string.Join(", ", measures.Distinct().Select(MeasureUniqueName));

        // Important: do not apply TopCount inside MDX.
        // Some SSAS multidimensional user hierarchies return empty data or fail when TopCount is applied directly on a deep level.
        // We request the level members from the cube, then sort and take Top N in C#.
        var rowSet = $"{axis.LevelUniqueName}.Members";

        return $@"
SELECT NON EMPTY {{ {measuresMdx} }} ON COLUMNS,
       NON EMPTY {rowSet}
       DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
FROM [{CubeDefinition.CubeName}]
{BuildWhereClause(filters)}".Trim();
    }

    public static string BuildWhereClause(AnalyticsFilterDto? filters)
    {
        var members = BuildFilterMembers(filters);
        return members.Count == 0 ? string.Empty : $"WHERE ({string.Join(", ", members)})";
    }

    private static void AddMostGranular(List<string> result, params (string? member, string expectedDimension)[] candidates)
    {
        foreach (var (member, expectedDimension) in candidates)
        {
            if (string.IsNullOrWhiteSpace(member))
            {
                continue;
            }

            var safe = TryValidateMemberUniqueName(member, expectedDimension);
            if (safe is null)
            {
                throw new ArgumentException($"Invalid member unique name for dimension {expectedDimension}.");
            }

            result.Add(safe);
            return;
        }
    }

    private static string? TryValidateMemberUniqueName(string memberUniqueName, string expectedDimension)
    {
        var value = memberUniqueName.Trim();

        if (value.Length == 0 || value.Length > 512)
        {
            return null;
        }

        if (value.Contains("&[]", StringComparison.OrdinalIgnoreCase) || value.EndsWith(".&[]", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!value.StartsWith(expectedDimension, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!SafeUniqueNameRegex.IsMatch(value))
        {
            return null;
        }

        return value;
    }

    private static TEnum ParseOrDefault<TEnum>(string? value, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        var normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty);
        return Enum.TryParse<TEnum>(normalized, ignoreCase: true, out var result) ? result : defaultValue;
    }
}
