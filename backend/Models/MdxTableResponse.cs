namespace SalesCubeApi.Models;

public sealed record MdxTableResponse(
    IReadOnlyList<string> Columns,
    IReadOnlyList<Dictionary<string, object?>> Rows
);
