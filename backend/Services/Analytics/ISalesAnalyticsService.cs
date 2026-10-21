using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface ISalesAnalyticsService
{
    Task<AnalyticsSeriesResponseDto> GetTrendAsync(SalesTrendRequestDto request, CancellationToken cancellationToken = default);
    Task<AnalyticsSeriesResponseDto> DrillDownAsync(SalesDrilldownRequestDto request, CancellationToken cancellationToken = default);
}
