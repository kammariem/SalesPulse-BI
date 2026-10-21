using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardAnalyticsService _dashboardService;

    public DashboardController(IDashboardAnalyticsService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Executive overview dashboard driven by CUBE_EDB measures and dimensions.
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(DashboardOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardOverviewDto>> GetOverview([FromQuery] AnalyticsFilterDto filters, CancellationToken cancellationToken)
    {
        return Ok(await _dashboardService.GetOverviewAsync(filters, cancellationToken));
    }

    /// <summary>
    /// Backward-compatible alias for older frontend code.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardOverviewDto>> GetSummary([FromQuery] AnalyticsFilterDto filters, CancellationToken cancellationToken)
    {
        return Ok(await _dashboardService.GetOverviewAsync(filters, cancellationToken));
    }
}
