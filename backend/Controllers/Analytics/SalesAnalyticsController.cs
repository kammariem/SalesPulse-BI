using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/sales")]
public sealed class SalesAnalyticsController : ControllerBase
{
    private readonly ISalesAnalyticsService _salesService;

    public SalesAnalyticsController(ISalesAnalyticsService salesService)
    {
        _salesService = salesService;
    }

    /// <summary>
    /// Time-series analysis using Dim Date hierarchy: Year > Semester > Quarter > Month > Date.
    /// </summary>
    [HttpGet("trend")]
    [ProducesResponseType(typeof(AnalyticsSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsSeriesResponseDto>> GetTrend([FromQuery] SalesTrendRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _salesService.GetTrendAsync(request, cancellationToken));
    }

    /// <summary>
    /// Generic OLAP drill-down across Date, Product, Customer, Employee, and Geography dimensions.
    /// </summary>
    [HttpGet("drilldown")]
    [ProducesResponseType(typeof(AnalyticsSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsSeriesResponseDto>> DrillDown([FromQuery] SalesDrilldownRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _salesService.DrillDownAsync(request, cancellationToken));
    }
}
