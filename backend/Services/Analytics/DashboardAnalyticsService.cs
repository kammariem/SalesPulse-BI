using SalesCubeApi.Constants;
using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class DashboardAnalyticsService : IDashboardAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;

    public DashboardAnalyticsService(IAnalyticsQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync(AnalyticsFilterDto filters, CancellationToken cancellationToken = default)
    {
        var kpis = await _queryService.GetMetricsAsync(MdxDefinition.DashboardMeasures, filters, cancellationToken);

        // Use only stable base measures for dimensional charts.
        // Calculated measures are still attempted in KPI cards but they are isolated measure by measure.
        var coreMeasures = new[]
        {
            MeasureKey.TotalAmount,
            MeasureKey.Quantity
        };

        var topProducts = await _queryService.GetSeriesAsync("Dashboard", MdxDefinition.ProductAxis(ProductLevel.Product), coreMeasures, filters, 5, MeasureKey.TotalAmount, cancellationToken);
        var topCustomers = await _queryService.GetSeriesAsync("Dashboard", MdxDefinition.CustomerAxis(CustomerLevel.Customer), coreMeasures, filters, 5, MeasureKey.TotalAmount, cancellationToken);
        var topCountries = await _queryService.GetSeriesAsync("Dashboard", MdxDefinition.GeographyAxis(GeographyLevel.Country), coreMeasures, filters, 5, MeasureKey.TotalAmount, cancellationToken);
        var topEmployees = await _queryService.GetSeriesAsync("Dashboard", MdxDefinition.EmployeeAxis(EmployeeLevel.Employee), coreMeasures, filters, 5, MeasureKey.TotalAmount, cancellationToken);

        return new DashboardOverviewDto(
            CubeName: CubeDefinition.CubeName,
            GeneratedAtUtc: DateTime.UtcNow,
            Kpis: kpis,
            TopProducts: topProducts.Points,
            TopCustomers: topCustomers.Points,
            TopCountries: topCountries.Points,
            TopEmployees: topEmployees.Points
        );
    }
}
