using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IAnalyticsQueryService
{
    Task<decimal?> GetScalarAsync(MeasureKey measure, AnalyticsFilterDto? filters = null, CancellationToken cancellationToken = default);

    Task<MetricValuesDto> GetMetricsAsync(IReadOnlyList<MeasureKey> measures, AnalyticsFilterDto? filters = null, CancellationToken cancellationToken = default);

    Task<AnalyticsSeriesResponseDto> GetSeriesAsync(
        string analyticalArea,
        AxisDefinition axis,
        IReadOnlyList<MeasureKey> measures,
        AnalyticsFilterDto? filters = null,
        int? top = null,
        MeasureKey orderByMeasure = MeasureKey.TotalAmount,
        CancellationToken cancellationToken = default);
}
