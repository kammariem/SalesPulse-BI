namespace SalesCubeApi.Constants;

public static class CubeDefinition
{
    public const string CubeName = "CUBE_EDB";

    public static class Measures
    {
        public const string Quantity = "[Measures].[Order Quantity]";
        public const string TotalAmount = "[Measures].[Total Amount]";
        public const string OrderCount = "[Measures].[Nombre totale de commandes]";
        public const string SalesLineCount = "[Measures].[Nombre de lignes de vente]";
        public const string AverageBasket = "[Measures].[Panier moyen]";
        public const string AverageQuantityPerOrder = "[Measures].[Quantité moyenne par commande]";
        public const string AverageUnitPrice = "[Measures].[Prix moyen unitaire]";
        public const string RevenuePerActiveCustomer = "[Measures].[CA par client actif]";
        public const string AmountInK = "[Measures].[Montant total en K]";
        public const string PreviousYearRevenue = "[Measures].[CA mois précédent]";
        public const string RevenueGrowthPercent = "[Measures].[Evolution CA %]";
        public const string YearToDateRevenue = "[Measures].[CA cumule annee]";
        public const string TotalQuantitySold = "[Measures].[Quantité totale vendue]";
        public const string CustomerCount = "[Measures].[Customer Count]";
        public const string AverageRevenuePerOrder = "[Measures].[CA moyen par commande]";
        public const string RevenueContribution = "[Measures].[Contribution CA %]";
        public const string MonthlyRevenueGrowth = "[Measures].[Evolution CA mois %]";
        public const string TopProduct = "[Measures].[Produit le plus vendu]";
        public const string TopCountry = "[Measures].[Top Country]";
        public const string TopCity = "[Measures].[Top City]";
        public const string TopCountryCity = "[Measures].[Top Country City]";
    }

    public static class Date
    {
        public const string Dimension = "[Dim Date]";
        public const string Hierarchy = "[Dim Date].[HiérarchieDate]";
        public const string Year = "[Dim Date].[HiérarchieDate].[Year]";
        public const string Semester = "[Dim Date].[HiérarchieDate].[Semester]";
        public const string Quarter = "[Dim Date].[HiérarchieDate].[Quarter]";
        public const string Month = "[Dim Date].[HiérarchieDate].[Month]";
        public const string DateLevel = "[Dim Date].[HiérarchieDate].[Date]";
        public const string DayOfWeek = "[Dim Date].[Day Of Week Name].[Day Of Week Name]";
    }

    public static class Product
    {
        public const string Dimension = "[Dim Product]";
        public const string Hierarchy = "[Dim Product].[Product Hierarchy]";
        public const string Category = "[Dim Product].[Product Hierarchy].[Category]";
        public const string Brand = "[Dim Product].[Product Hierarchy].[Brand]";
        public const string ProductLevel = "[Dim Product].[Product Hierarchy].[Product]";
        public const string ListPrice = "[Dim Product].[List Price].[List Price]";
    }

    public static class Customer
    {
        public const string Dimension = "[Dim Customer]";
        public const string Hierarchy = "[Dim Customer].[Customer Hierarchy]";
        public const string Category = "[Dim Customer].[Customer Hierarchy].[Category]";
        public const string CustomerType = "[Dim Customer].[Customer Hierarchy].[Customer Type]";
        public const string Company = "[Dim Customer].[Customer Hierarchy].[Company]";
        public const string CustomerLevel = "[Dim Customer].[Customer Hierarchy].[Customer]";
        public const string DiscountPercent = "[Dim Customer].[Discount Percent].[Discount Percent]";
    }

    public static class Employee
    {
        public const string Dimension = "[Dim Employee]";
        public const string Hierarchy = "[Dim Employee].[Employee Hierarchy]";
        public const string Department = "[Dim Employee].[Employee Hierarchy].[Department]";
        public const string JobTitle = "[Dim Employee].[Employee Hierarchy].[Job Title]";
        public const string EmployeeLevel = "[Dim Employee].[Employee Hierarchy].[Employee]";
    }

    public static class Geography
    {
        public const string Dimension = "[Dim Geography]";
        public const string Hierarchy = "[Dim Geography].[Geography Hierarchy]";
        public const string Country = "[Dim Geography].[Geography Hierarchy].[Country]";
        public const string City = "[Dim Geography].[Geography Hierarchy].[City]";
    }
}
