using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IGeographyAnalyticsService
{
    Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(GeographyPerformanceRequestDto request, CancellationToken cancellationToken = default);
}
