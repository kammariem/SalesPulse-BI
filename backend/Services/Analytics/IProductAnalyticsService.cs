using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IProductAnalyticsService
{
    Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(ProductPerformanceRequestDto request, CancellationToken cancellationToken = default);
}
