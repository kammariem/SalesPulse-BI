using SalesCubeApi.Constants;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Repositories;

namespace SalesCubeApi.Services.Analytics;

public sealed class DimensionAnalyticsService : IDimensionAnalyticsService
{
    private readonly IAnalyticsRepository _repository;

    public DimensionAnalyticsService(IAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<DimensionMembersResponseDto> GetMembersAsync(DimensionMembersRequestDto request, CancellationToken cancellationToken = default)
    {
        var axis = MdxDefinition.AxisFromDrilldown(request.Dimension, request.Level);
        var safeTop = Math.Clamp(request.Top, 1, 500);
        // Avoid TOPCOUNT on deep user hierarchies; limit the returned members after mapping.
        var mdx = MdxDefinition.BuildAxisQuery(axis, [MeasureKey.TotalAmount], request);
        var table = await _repository.ExecuteMdxAsync(mdx, cancellationToken);
        var members = MdxResultMapper.ToDimensionMembers(table)
            .OrderByDescending(member => member.TotalAmount ?? decimal.MinValue)
            .Take(safeTop)
            .ToList();

        return new DimensionMembersResponseDto(
            CubeName: CubeDefinition.CubeName,
            Dimension: axis.Axis,
            Level: axis.LevelName,
            GeneratedAtUtc: DateTime.UtcNow,
            Members: members
        );
    }
}
