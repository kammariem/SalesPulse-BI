using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class EmployeeAnalyticsService : IEmployeeAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;

    public EmployeeAnalyticsService(IAnalyticsQueryService queryService)
    {
        _queryService = queryService;
    }

    public Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(EmployeePerformanceRequestDto request, CancellationToken cancellationToken = default)
    {
        var measures = new[]
        {
            request.Measure,
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        }.Distinct().ToArray();

        return _queryService.GetSeriesAsync("Employee Analytics", MdxDefinition.EmployeeAxis(request.Level), measures, request, request.Top, request.Measure, cancellationToken);
    }
}
