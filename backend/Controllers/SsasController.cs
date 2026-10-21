using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models;
using SalesCubeApi.Services;

namespace SalesCubeApi.Controllers;

[ApiController]
[Route("api/ssas")]
public sealed class SsasController : ControllerBase
{
    private readonly ISsasService _ssasService;

    public SsasController(ISsasService ssasService)
    {
        _ssasService = ssasService;
    }

    [HttpGet("test")]
    [ProducesResponseType(typeof(SsasConnectionStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SsasConnectionStatusDto>> TestConnection(CancellationToken cancellationToken)
    {
        var result = await _ssasService.TestConnectionAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("cubes")]
    [ProducesResponseType(typeof(MdxTableResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MdxTableResponse>> GetCubes(CancellationToken cancellationToken)
    {
        var result = await _ssasService.GetCubesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("measures")]
    [ProducesResponseType(typeof(MdxTableResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MdxTableResponse>> GetMeasures([FromQuery] string? cubeName, CancellationToken cancellationToken)
    {
        var result = await _ssasService.GetMeasuresAsync(cubeName, cancellationToken);
        return Ok(result);
    }

    [HttpGet("dimensions")]
    [ProducesResponseType(typeof(MdxTableResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MdxTableResponse>> GetDimensions([FromQuery] string? cubeName, CancellationToken cancellationToken)
    {
        var result = await _ssasService.GetDimensionsAsync(cubeName, cancellationToken);
        return Ok(result);
    }
}
