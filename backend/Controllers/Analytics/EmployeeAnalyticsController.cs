using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/employees")]
public sealed class EmployeeAnalyticsController : ControllerBase
{
    private readonly IEmployeeAnalyticsService _employeeService;

    public EmployeeAnalyticsController(IEmployeeAnalyticsService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Employee performance by Department, Job Title, or Employee.
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(AnalyticsSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsSeriesResponseDto>> GetPerformance([FromQuery] EmployeePerformanceRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _employeeService.GetPerformanceAsync(request, cancellationToken));
    }
}
