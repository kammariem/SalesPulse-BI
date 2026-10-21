using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IKpiAnalyticsService
{
    Task<IReadOnlyList<AnalyticsMetricDto>> GetKpisAsync(AnalyticsFilterDto filters, CancellationToken cancellationToken = default);
}
