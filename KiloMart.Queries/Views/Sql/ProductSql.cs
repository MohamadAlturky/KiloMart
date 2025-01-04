using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductSql => @"
    SELECT
        [Id],
        [ImageUrl],
        [ProductCategory],
        [IsActive],
        [MeasurementUnit],
        [Description],
        [Name]
    FROM 
        dbo.[Product]";

    public static DbQuery<ProductSqlResponse> ProductSqlQuery 
    => new(ProductSql);
}

public class ProductSqlResponse
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int ProductCategory { get; set; }
    public bool IsActive { get; set; }
    public string MeasurementUnit { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Name { get; set; } = null!;
}
