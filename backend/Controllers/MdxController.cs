using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SalesCubeApi.Models;
using SalesCubeApi.Options;
using SalesCubeApi.Services;

namespace SalesCubeApi.Controllers;

[ApiController]
[Route("api/mdx")]
public sealed class MdxController : ControllerBase
{
    private readonly ISsasService _ssasService;
    private readonly SsasOptions _options;
    private readonly IWebHostEnvironment _environment;

    public MdxController(ISsasService ssasService, IOptions<SsasOptions> options, IWebHostEnvironment environment)
    {
        _ssasService = ssasService;
        _options = options.Value;
        _environment = environment;
    }

    [HttpPost("query")]
    [ProducesResponseType(typeof(MdxTableResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MdxTableResponse>> ExecuteQuery([FromBody] MdxQueryRequest request, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment() || !_options.AllowRawMdxInDevelopment)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "L'exécution MDX brute est autorisée uniquement en mode Development et si AllowRawMdxInDevelopment=true.");
        }

        var query = request.Query.Trim();
        if (!query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Pour la sécurité, seules les requêtes MDX SELECT sont acceptées ici.");
        }

        var result = await _ssasService.ExecuteTableAsync(query, cancellationToken);
        return Ok(result);
    }
}
