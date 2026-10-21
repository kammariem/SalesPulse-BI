namespace SalesCubeApi.Models;

public sealed record ChartPointDto(
    string Label,
    decimal Value,
    string? Category = null
);
