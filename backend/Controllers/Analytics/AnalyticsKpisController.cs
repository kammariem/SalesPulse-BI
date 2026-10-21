using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/analytics")]
public sealed class AnalyticsKpisController : ControllerBase
{
    private readonly IKpiAnalyticsService _kpiService;

    public AnalyticsKpisController(IKpiAnalyticsService kpiService)
    {
        _kpiService = kpiService;
    }

    /// <summary>
    /// Returns business KPIs exposed by the cube measures.
    /// The cube metadata does not define SSAS KPI objects, so this endpoint exposes KPI-like calculated measures.
    /// </summary>
    [HttpGet("kpis")]
    [ProducesResponseType(typeof(IReadOnlyList<AnalyticsMetricDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AnalyticsMetricDto>>> GetKpis([FromQuery] AnalyticsFilterDto filters, CancellationToken cancellationToken)
    {
        return Ok(await _kpiService.GetKpisAsync(filters, cancellationToken));
    }
}
