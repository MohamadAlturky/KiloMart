using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductOfferSql => @"
    SELECT
        [Id],
        [Product],
        [Price],
        [OffPercentage],
        [FromDate],
        [ToDate],
        [Quantity],
        [Provider],
        [IsActive]
    FROM 
        dbo.[ProductOffer]";

    public static DbQuery<ProductOfferSqlResponse> ProductOfferSqlQuery 
    => new(ProductOfferSql);
}

public class ProductOfferSqlResponse
{
    public int Id { get; set; }
    public int Product { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public double Quantity { get; set; }
    public int Provider { get; set; }
    public bool IsActive { get; set; }
}
