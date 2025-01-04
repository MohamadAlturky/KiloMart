using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductCategorySql => @"
    SELECT
        [Id],
        [IsActive],
        [Name]
    FROM 
        dbo.[ProductCategory]";

    public static DbQuery<ProductCategorySqlResponse> ProductCategorySqlQuery 
    => new(ProductCategorySql);
}

public class ProductCategorySqlResponse
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = null!;
}
