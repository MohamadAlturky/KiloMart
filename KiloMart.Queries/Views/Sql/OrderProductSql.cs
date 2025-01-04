using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderProductSql => @"
    SELECT
        [Id],
        [Order],
        [Product],
        [Quantity]
    FROM 
        dbo.[OrderProduct]";

    public static DbQuery<OrderProductSqlResponse> OrderProductSqlQuery 
    => new(OrderProductSql);
}

public class OrderProductSqlResponse
{
    public int Id { get; set; }
    public long Order { get; set; }
    public int Product { get; set; }
    public float Quantity { get; set; }
}
