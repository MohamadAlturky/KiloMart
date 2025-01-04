using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string CartSql => @"
    SELECT
        [Id],
        [Product],
        [Quantity],
        [Customer]
    FROM 
        dbo.[Cart]";

    public static DbQuery<CartSqlResponse> CartSqlQuery 
    => new(CartSql);
}

public class CartSqlResponse
{
    public long Id { get; set; }
    public int Product { get; set; }
    public float Quantity { get; set; }
    public int Customer { get; set; }
}
