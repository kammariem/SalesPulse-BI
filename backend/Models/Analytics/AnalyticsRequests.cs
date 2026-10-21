namespace SalesCubeApi.Models.Analytics;

public sealed class SalesTrendRequestDto : AnalyticsFilterDto
{
    public TimeGrain Grain { get; set; } = TimeGrain.Month;
}

public sealed class SalesDrilldownRequestDto : AnalyticsFilterDto
{
    public SalesDrilldownAxis Axis { get; set; } = SalesDrilldownAxis.Product;
    public string? Level { get; set; }
    public MeasureKey Measure { get; set; } = MeasureKey.TotalAmount;
    public int Top { get; set; } = 20;
}

public sealed class ProductPerformanceRequestDto : AnalyticsFilterDto
{
    public ProductLevel Level { get; set; } = ProductLevel.Category;
    public MeasureKey Measure { get; set; } = MeasureKey.TotalAmount;
    public int Top { get; set; } = 10;
}

public sealed class CustomerPerformanceRequestDto : AnalyticsFilterDto
{
    public CustomerLevel Level { get; set; } = CustomerLevel.Category;
    public MeasureKey Measure { get; set; } = MeasureKey.TotalAmount;
    public int Top { get; set; } = 10;
}

public sealed class EmployeePerformanceRequestDto : AnalyticsFilterDto
{
    public EmployeeLevel Level { get; set; } = EmployeeLevel.Department;
    public MeasureKey Measure { get; set; } = MeasureKey.TotalAmount;
    public int Top { get; set; } = 10;
}

public sealed class GeographyPerformanceRequestDto : AnalyticsFilterDto
{
    public GeographyLevel Level { get; set; } = GeographyLevel.Country;
    public MeasureKey Measure { get; set; } = MeasureKey.TotalAmount;
    public int Top { get; set; } = 25;
}

public sealed class DimensionMembersRequestDto : AnalyticsFilterDto
{
    public SalesDrilldownAxis Dimension { get; set; }
    public string? Level { get; set; }
    public int Top { get; set; } = 200;
}
