using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class SalesAnalyticsService : ISalesAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;

    public SalesAnalyticsService(IAnalyticsQueryService queryService)
    {
        _queryService = queryService;
    }

    public Task<AnalyticsSeriesResponseDto> GetTrendAsync(SalesTrendRequestDto request, CancellationToken cancellationToken = default)
    {
        var axis = MdxDefinition.TimeAxis(request.Grain);
        // Keep trend query stable: use only base aggregations for charts.
        // Calculated time measures can be exposed as KPI cards, but they can fail if the date context is not compatible.
        var measures = new[]
        {
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        };

        return _queryService.GetSeriesAsync("Sales Trend", axis, measures, request, null, MeasureKey.TotalAmount, cancellationToken);
    }

    public Task<AnalyticsSeriesResponseDto> DrillDownAsync(SalesDrilldownRequestDto request, CancellationToken cancellationToken = default)
    {
        var axis = MdxDefinition.AxisFromDrilldown(request.Axis, request.Level);
        var measures = new[]
        {
            request.Measure,
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        }.Distinct().ToArray();

        return _queryService.GetSeriesAsync("Sales Drilldown", axis, measures, request, request.Top, request.Measure, cancellationToken);
    }
}
