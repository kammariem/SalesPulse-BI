namespace SalesCubeApi.Options;

public sealed class MdxQueriesOptions
{
    public const string SectionName = "MdxQueries";

    public string TotalSales { get; set; } = string.Empty;
    public string TotalOrders { get; set; } = string.Empty;
    public string TotalQuantity { get; set; } = string.Empty;
    public string AverageOrderValue { get; set; } = string.Empty;
    public string TopProduct { get; set; } = string.Empty;
    public string TopCountry { get; set; } = string.Empty;
    public string SalesByMonth { get; set; } = string.Empty;
    public string SalesByProduct { get; set; } = string.Empty;
    public string SalesByCountry { get; set; } = string.Empty;
    public string SalesByEmployee { get; set; } = string.Empty;
}
