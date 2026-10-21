using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public sealed class KpiAnalyticsService : IKpiAnalyticsService
{
    private readonly IAnalyticsQueryService _queryService;
    private readonly ILogger<KpiAnalyticsService> _logger;

    public KpiAnalyticsService(IAnalyticsQueryService queryService, ILogger<KpiAnalyticsService> logger)
    {
        _queryService = queryService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AnalyticsMetricDto>> GetKpisAsync(AnalyticsFilterDto filters, CancellationToken cancellationToken = default)
    {
        var result = new List<AnalyticsMetricDto>();
        var failures = 0;

        foreach (var measure in MdxDefinition.DashboardMeasures)
        {
            decimal? value = null;

            try
            {
                value = await _queryService.GetScalarAsync(measure, filters, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load KPI {Measure} from SSAS.", measure);
                failures++;
            }

            result.Add(new AnalyticsMetricDto(
                Key: measure.ToString(),
                Caption: MdxDefinition.MeasureCaption(measure),
                UniqueName: MdxDefinition.MeasureUniqueName(measure),
                Value: value,
                Format: MdxDefinition.MeasureFormat(measure)
            ));
        }

        if (result.Count > 0 && failures == result.Count)
        {
            throw new InvalidOperationException("All KPI requests to SSAS failed.");
        }

        return result;
    }
}
