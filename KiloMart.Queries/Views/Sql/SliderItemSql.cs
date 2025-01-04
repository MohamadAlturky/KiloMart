using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string SliderItemSql => @"
    SELECT
        [Id],
        [ImageUrl],
        [Target]
    FROM 
        dbo.[SliderItem]";

    public static DbQuery<SliderItemSqlResponse> SliderItemSqlQuery 
    => new(SliderItemSql);
}

public class SliderItemSqlResponse
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int? Target { get; set; }
}
