using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/dimensions")]
public sealed class DimensionsAnalyticsController : ControllerBase
{
    private readonly IDimensionAnalyticsService _dimensionService;

    public DimensionsAnalyticsController(IDimensionAnalyticsService dimensionService)
    {
        _dimensionService = dimensionService;
    }

    /// <summary>
    /// Returns filterable members from a cube dimension level with MEMBER_UNIQUE_NAME values.
    /// Use these unique names as filters in analytical endpoints.
    /// </summary>
    [HttpGet("members")]
    [ProducesResponseType(typeof(DimensionMembersResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DimensionMembersResponseDto>> GetMembers([FromQuery] DimensionMembersRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _dimensionService.GetMembersAsync(request, cancellationToken));
    }
}
