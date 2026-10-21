using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface ICustomerAnalyticsService
{
    Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(CustomerPerformanceRequestDto request, CancellationToken cancellationToken = default);
}
