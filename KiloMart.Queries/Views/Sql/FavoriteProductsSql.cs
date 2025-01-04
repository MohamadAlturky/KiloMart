using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string FavoriteProductsSql => @"
    SELECT
        [Id],
        [Customer],
        [Product]
    FROM 
        dbo.[FavoriteProducts]";

    public static DbQuery<FavoriteProductsSqlResponse> FavoriteProductsSqlQuery 
    => new(FavoriteProductsSql);
}

public class FavoriteProductsSqlResponse
{
    public long Id { get; set; }
    public int Customer { get; set; }
    public int Product { get; set; }
}
