namespace SalesCubeApi.Models.Analytics;

/// <summary>
/// Filters are based only on dimensions that actually exist in CUBE_EDB.
/// Pass SSAS member unique names returned by /api/dimensions/members.
/// Example: [Dim Product].[Product Hierarchy].[Category].&[Accessories]
/// </summary>
public class AnalyticsFilterDto
{
    public string? YearUniqueName { get; set; }
    public string? SemesterUniqueName { get; set; }
    public string? QuarterUniqueName { get; set; }
    public string? MonthUniqueName { get; set; }
    public string? DateUniqueName { get; set; }

    public string? ProductCategoryUniqueName { get; set; }
    public string? BrandUniqueName { get; set; }
    public string? ProductUniqueName { get; set; }

    public string? CustomerCategoryUniqueName { get; set; }
    public string? CustomerTypeUniqueName { get; set; }
    public string? CompanyUniqueName { get; set; }
    public string? CustomerUniqueName { get; set; }
    public string? DiscountPercentUniqueName { get; set; }

    public string? DepartmentUniqueName { get; set; }
    public string? JobTitleUniqueName { get; set; }
    public string? EmployeeUniqueName { get; set; }

    public string? CountryUniqueName { get; set; }
    public string? CityUniqueName { get; set; }
}
