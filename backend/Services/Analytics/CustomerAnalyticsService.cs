using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class CustomerAnalyticsService : ICustomerAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;

    public CustomerAnalyticsService(IAnalyticsQueryService queryService)
    {
        _queryService = queryService;
    }

    public Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(CustomerPerformanceRequestDto request, CancellationToken cancellationToken = default)
    {
        var measures = new[]
        {
            request.Measure,
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        }.Distinct().ToArray();

        return _queryService.GetSeriesAsync("Customer Analytics", MdxDefinition.CustomerAxis(request.Level), measures, request, request.Top, request.Measure, cancellationToken);
    }
}
