namespace SalesCubeApi.Models;

public sealed record KpiSummaryDto(
    decimal TotalSales,
    int TotalOrders,
    int TotalQuantity,
    decimal AverageOrderValue,
    string? TopProduct,
    string? TopCountry,
    DateTime GeneratedAtUtc
);
