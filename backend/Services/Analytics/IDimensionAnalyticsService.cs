using SalesCubeApi.Models.Analytics;

namespace SalesCubeApi.Services.Analytics;

public interface IDimensionAnalyticsService
{
    Task<DimensionMembersResponseDto> GetMembersAsync(DimensionMembersRequestDto request, CancellationToken cancellationToken = default);
}
