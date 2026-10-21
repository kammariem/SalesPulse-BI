namespace SalesCubeApi.Models;

public sealed record MetadataSchemaTableDto(
    string Name,
    IReadOnlyList<string> Columns,
    IReadOnlyList<Dictionary<string, object?>> Rows,
    int RowCount,
    string? Error
);
