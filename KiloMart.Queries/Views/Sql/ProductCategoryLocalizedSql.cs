using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductCategoryLocalizedSql => @"
    SELECT
        [Name],
        [Language],
        [ProductCategory]
    FROM 
        dbo.[ProductCategoryLocalized]";

    public static DbQuery<ProductCategoryLocalizedSqlResponse> ProductCategoryLocalizedSqlQuery 
    => new(ProductCategoryLocalizedSql);
}

public class ProductCategoryLocalizedSqlResponse
{
    public string Name { get; set; } = null!;
    public byte Language { get; set; }
    public int ProductCategory { get; set; }
}
