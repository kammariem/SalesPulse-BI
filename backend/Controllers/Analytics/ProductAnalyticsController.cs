using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models.Analytics;
using SalesCubeApi.Services.Analytics;

namespace SalesCubeApi.Controllers.Analytics;

[ApiController]
[Route("api/products")]
public sealed class ProductAnalyticsController : ControllerBase
{
    private readonly IProductAnalyticsService _productService;

    public ProductAnalyticsController(IProductAnalyticsService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Product performance by Category, Brand, or Product.
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(AnalyticsSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AnalyticsSeriesResponseDto>> GetPerformance([FromQuery] ProductPerformanceRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetPerformanceAsync(request, cancellationToken));
    }
}
