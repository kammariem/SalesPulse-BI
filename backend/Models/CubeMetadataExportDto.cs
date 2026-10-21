namespace SalesCubeApi.Models;

public sealed record CubeMetadataExportDto(
    string? RequestedCubeName,
    string? Database,
    DateTime GeneratedAtUtc,
    bool IncludeMembers,
    IReadOnlyList<string> Warnings,
    IReadOnlyDictionary<string, MetadataSchemaTableDto> Rowsets
);
