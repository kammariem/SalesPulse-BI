using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IEmployeeAnalyticsService
{
    Task<AnalyticsSeriesResponseDto> GetPerformanceAsync(EmployeePerformanceRequestDto request, CancellationToken cancellationToken = default);
}
