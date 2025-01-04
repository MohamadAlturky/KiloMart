using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductLocalizedSql => @"
    SELECT
        [Language],
        [Product],
        [MeasurementUnit],
        [Description],
        [Name]
    FROM 
        dbo.[ProductLocalized]";

    public static DbQuery<ProductLocalizedSqlResponse> ProductLocalizedSqlQuery 
    => new(ProductLocalizedSql);
}

public class ProductLocalizedSqlResponse
{
    public byte Language { get; set; }
    public int Product { get; set; }
    public string MeasurementUnit { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Name { get; set; } = null!;
}
