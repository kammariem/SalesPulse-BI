using Microsoft.AspNetCore.Mvc;

namespace SalesCubeApi.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "OK",
            api = "Sales Cube API",
            timeUtc = DateTime.UtcNow
        });
    }
}
