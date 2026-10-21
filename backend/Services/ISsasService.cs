using SalesCubeApi.Models;

namespace SalesCubeApi.Services;

public interface ISsasService
{
    Task<SsasConnectionStatusDto> TestConnectionAsync(CancellationToken cancellationToken = default);

    Task<decimal> ExecuteScalarDecimalAsync(string mdx, CancellationToken cancellationToken = default);

    Task<string?> ExecuteFirstLabelAsync(string mdx, CancellationToken cancellationToken = default);

    Task<MdxTableResponse> ExecuteTableAsync(string mdx, CancellationToken cancellationToken = default);

    Task<MdxTableResponse> GetCubesAsync(CancellationToken cancellationToken = default);

    Task<MdxTableResponse> GetMeasuresAsync(string? cubeName, CancellationToken cancellationToken = default);

    Task<MdxTableResponse> GetDimensionsAsync(string? cubeName, CancellationToken cancellationToken = default);

    Task<CubeMetadataExportDto> GetCubeMetadataAsync(string? cubeName, bool includeMembers = false, CancellationToken cancellationToken = default);
}
