using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SalesCubeApi.Models;
using SalesCubeApi.Services;

namespace SalesCubeApi.Controllers;

[ApiController]
[Route("api/metadata")]
public sealed class MetadataController : ControllerBase
{
    private readonly ISsasService _ssasService;

    public MetadataController(ISsasService ssasService)
    {
        _ssasService = ssasService;
    }

    [HttpGet("cube")]
    [ProducesResponseType(typeof(CubeMetadataExportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CubeMetadataExportDto>> GetCubeMetadata(
        [FromQuery] string? cubeName,
        [FromQuery] bool includeMembers = false,
        CancellationToken cancellationToken = default)
    {
        var metadata = await _ssasService.GetCubeMetadataAsync(cubeName, includeMembers, cancellationToken);
        return Ok(metadata);
    }

    [HttpGet("download")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadCubeMetadata(
        [FromQuery] string? cubeName,
        [FromQuery] bool includeMembers = false,
        CancellationToken cancellationToken = default)
    {
        var metadata = await _ssasService.GetCubeMetadataAsync(cubeName, includeMembers, cancellationToken);

        var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        var safeCubeName = string.IsNullOrWhiteSpace(cubeName)
            ? "all-cubes"
            : string.Join("_", cubeName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

        var fileName = $"ssas-metadata-{safeCubeName}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
        return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
    }
}
