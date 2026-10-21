using SalesCubeApi.Constants;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Repositories;

namespace SalesCubeApi.Services.Analytics;

public sealed class AnalyticsQueryService : IAnalyticsQueryService
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<AnalyticsQueryService> _logger;

    public AnalyticsQueryService(IAnalyticsRepository repository, ILogger<AnalyticsQueryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<decimal?> GetScalarAsync(MeasureKey measure, AnalyticsFilterDto? filters = null, CancellationToken cancellationToken = default)
    {
        var mdx = MdxDefinition.BuildScalarQuery(measure, filters);
        var table = await _repository.ExecuteMdxAsync(mdx, cancellationToken);
        return MdxResultMapper.ExtractFirstDecimal(table);
    }

    public async Task<MetricValuesDto> GetMetricsAsync(IReadOnlyList<MeasureKey> measures, AnalyticsFilterDto? filters = null, CancellationToken cancellationToken = default)
    {
        var values = new Dictionary<MeasureKey, decimal?>();

        // Optional calculated measures may be absent in some cube deployments.
        // Keep successful measures, but never claim a fully failed cube is successful.
        Exception? firstFailure = null;
        var failures = 0;
        foreach (var measure in measures.Distinct())
        {
            try
            {
                values[measure] = await GetScalarAsync(measure, filters, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SSAS measure {Measure} could not be read.", measure);
                firstFailure ??= ex;
                failures++;
                values[measure] = null;
            }
        }

        if (values.Count > 0 && failures == values.Count)
        {
            throw new InvalidOperationException("No BI measures could be loaded from SSAS.", firstFailure);
        }

        return MdxResultMapper.ToMetricValues(values);
    }

    public async Task<AnalyticsSeriesResponseDto> GetSeriesAsync(
        string analyticalArea,
        AxisDefinition axis,
        IReadOnlyList<MeasureKey> measures,
        AnalyticsFilterDto? filters = null,
        int? top = null,
        MeasureKey orderByMeasure = MeasureKey.TotalAmount,
        CancellationToken cancellationToken = default)
    {
        var safeTop = top.HasValue ? Math.Clamp(top.Value, 1, 500) : (int?)null;
        var mdx = MdxDefinition.BuildAxisQuery(axis, measures, filters, safeTop, orderByMeasure);

        // Query failures must reach the global exception handler and the frontend error banner.
        var table = await _repository.ExecuteMdxAsync(mdx, cancellationToken);
        var points = MdxResultMapper.ToAnalyticsPoints(table, measures);

        if (safeTop.HasValue)
        {
            points = points
                .OrderByDescending(point => MdxResultMapper.GetMetricValue(point, orderByMeasure) ?? decimal.MinValue)
                .Take(safeTop.Value)
                .ToList();
        }

        return new AnalyticsSeriesResponseDto(
            CubeName: CubeDefinition.CubeName,
            AnalyticalArea: analyticalArea,
            Axis: axis.Axis,
            Level: axis.LevelName,
            Mdx: mdx,
            GeneratedAtUtc: DateTime.UtcNow,
            Points: points
        );
    }
}
