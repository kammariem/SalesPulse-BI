using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class ProductAnalyticsService : IProductAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;

    public ProductAnalyticsService(IAnalyticsQueryService queryService)
    {
        _queryService = queryService;
    }

    public Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(ProductPerformanceRequestDto request, CancellationToken cancellationToken = default)
    {
        var measures = new[]
        {
            request.Measure,
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        }.Distinct().ToArray();

        return _queryService.GetSeriesAsync("Product Analytics", MdxDefinition.ProductAxis(request.Level), measures, request, request.Top, request.Measure, cancellationToken);
    }
}
