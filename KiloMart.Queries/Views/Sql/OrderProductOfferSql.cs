using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderProductOfferSql => @"
    SELECT
        [Id],
        [Order],
        [ProductOffer],
        [UnitPrice],
        [Quantity]
    FROM 
        dbo.[OrderProductOffer]";

    public static DbQuery<OrderProductOfferSqlResponse> OrderProductOfferSqlQuery 
    => new(OrderProductOfferSql);
}

public class OrderProductOfferSqlResponse
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int ProductOffer { get; set; }
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }
}
