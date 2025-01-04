using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProductRequestDataLocalizedSql => @"
    SELECT
        [ProductRequest],
        [Language],
        [Name],
        [Description], 
        [MeasurementUnit]
    FROM 
        dbo.[ProductRequestDataLocalized]";

    public static DbQuery<ProductRequestDataLocalizedSqlResponse> ProductRequestDataLocalizedSqlQuery 
    => new(ProductRequestDataLocalizedSql);
}

public class ProductRequestDataLocalizedSqlResponse
{
    public int ProductRequest { get; set; }
    public byte Language { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string MeasurementUnit { get; set; } = null!;
}
