using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class GeographyAnalyticsService : IGeographyAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;

    public GeographyAnalyticsService(IAnalyticsQueryService queryService)
    {
        _queryService = queryService;
    }

    public Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(GeographyPerformanceRequestDto request, CancellationToken cancellationToken = default)
    {
        var measures = new[]
        {
            request.Measure,
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        }.Distinct().ToArray();

        return _queryService.GetSeriesAsync("Geography Analytics", MdxDefinition.GeographyAxis(request.Level), measures, request, request.Top, request.Measure, cancellationToken);
    }
}
