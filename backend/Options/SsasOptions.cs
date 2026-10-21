namespace SalesCubeApi.Options;

public sealed class SsasOptions
{
    public const string SectionName = "Ssas";

    public string ConnectionString { get; set; } = string.Empty;

    public int CommandTimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Allows POST /api/mdx/query only in Development.
    /// Keep it false in production because raw MDX can expose sensitive cube data.
    /// </summary>
    public bool AllowRawMdxInDevelopment { get; set; } = false;
}
