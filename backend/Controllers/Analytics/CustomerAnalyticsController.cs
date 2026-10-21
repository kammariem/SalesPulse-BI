using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/customers")]
public sealed class CustomerAnalyticsController : ControllerBase
{
    private readonly ICustomerAnalyticsService _customerService;

    public CustomerAnalyticsController(ICustomerAnalyticsService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Customer performance by Category, Customer Type, Company, Customer, or Discount Percent.
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(AnalyticsSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsSeriesResponseDto>> GetPerformance([FromQuery] CustomerPerformanceRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _customerService.GetPerformanceAsync(request, cancellationToken));
    }
}
