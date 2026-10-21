using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/geography")]
public sealed class GeographyAnalyticsController : ControllerBase
{
    private readonly IGeographyAnalyticsService _geographyService;

    public GeographyAnalyticsController(IGeographyAnalyticsService geographyService)
    {
        _geographyService = geographyService;
    }

    /// <summary>
    /// Geography performance by Country or City.
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(AnalyticsSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsSeriesResponseDto>> GetPerformance([FromQuery] GeographyPerformanceRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _geographyService.GetPerformanceAsync(request, cancellationToken));
    }
}
