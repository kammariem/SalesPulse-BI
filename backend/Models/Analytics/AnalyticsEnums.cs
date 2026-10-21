namespace SalesCubeApi.Models.Analytics;

public enum MeasureKey
{
    TotalAmount,
    Quantity,
    Orders,
    SalesLines,
    AverageBasket,
    AverageQuantityPerOrder,
    AverageUnitPrice,
    RevenuePerActiveCustomer,
    AmountInK,
    PreviousYearRevenue,
    RevenueGrowthPercent,
    YearToDateRevenue
}

public enum TimeGrain
{
    Year,
    Semester,
    Quarter,
    Month,
    Date
}

public enum ProductLevel
{
    Category,
    Brand,
    Product
}

public enum CustomerLevel
{
    Category,
    CustomerType,
    Company,
    Customer,
    DiscountPercent
}

public enum EmployeeLevel
{
    Department,
    JobTitle,
    Employee
}

public enum GeographyLevel
{
    Country,
    City
}

public enum SalesDrilldownAxis
{
    Date,
    Product,
    Customer,
    Employee,
    Geography
}
