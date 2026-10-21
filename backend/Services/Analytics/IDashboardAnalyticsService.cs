using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IDashboardAnalyticsService
{
    Task<DashboardOverviewDto> GetOverviewAsync(AnalyticsFilterDto filters, CancellationToken cancellationToken = default);
}
