using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductRequestSql => @"
    SELECT
        [Id],
        [Provider],
        [Date],
        [ImageUrl],
        [ProductCategory],
        [Price],
        [OffPercentage],
        [Quantity],
        [Status]
    FROM 
        dbo.[ProductRequest]";

    public static DbQuery<ProductRequestSqlResponse> ProductRequestSqlQuery 
    => new(ProductRequestSql);
}

public class ProductRequestSqlResponse
{
    public int Id { get; set; }
    public int Provider { get; set; }
    public DateTime Date { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int ProductCategory { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public double Quantity { get; set; }
    public byte Status { get; set; }
}
