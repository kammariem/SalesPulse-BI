namespace SalesCubeApi.Models.Analytics;

public sealed record MetricValuesDto(
    decimal? TotalAmount = null,
    decimal? Quantity = null,
    decimal? Orders = null,
    decimal? SalesLines = null,
    decimal? AverageBasket = null,
    decimal? AverageQuantityPerOrder = null,
    decimal? AverageUnitPrice = null,
    decimal? RevenuePerActiveCustomer = null,
    decimal? AmountInK = null,
    decimal? PreviousYearRevenue = null,
    decimal? RevenueGrowthPercent = null,
    decimal? YearToDateRevenue = null
);

public sealed record AnalyticsPointDto(
    string Label,
    string? UniqueName,
    MetricValuesDto Metrics
);

public sealed record AnalyticsMetricDto(
    string Key,
    string Caption,
    string UniqueName,
    decimal? Value,
    string? Format
);

public sealed record DashboardOverviewDto(
    string CubeName,
    DateTime GeneratedAtUtc,
    MetricValuesDto Kpis,
    IReadOnlyList<AnalyticsPointDto> TopProducts,
    IReadOnlyList<AnalyticsPointDto> TopCustomers,
    IReadOnlyList<AnalyticsPointDto> TopCountries,
    IReadOnlyList<AnalyticsPointDto> TopEmployees
);

public sealed record AnalyticsSeriesResponseDto(
    string CubeName,
    string AnalyticalArea,
    string Axis,
    string Level,
    string Mdx,
    DateTime GeneratedAtUtc,
    IReadOnlyList<AnalyticsPointDto> Points
);

public sealed record DimensionMemberDto(
    string Label,
    string? UniqueName,
    decimal? TotalAmount
);

public sealed record DimensionMembersResponseDto(
    string CubeName,
    string Dimension,
    string Level,
    DateTime GeneratedAtUtc,
    IReadOnlyList<DimensionMemberDto> Members
);
