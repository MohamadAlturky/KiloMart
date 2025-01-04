using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string SearchHistorySql => @"
    SELECT
        [Id],
        [Customer],
        [Term]
    FROM 
        dbo.[SearchHistory]";

    public static DbQuery<SearchHistorySqlResponse> SearchHistorySqlQuery 
    => new(SearchHistorySql);
}

public class SearchHistorySqlResponse
{
    public long Id { get; set; }
    public int Customer { get; set; }
    public string Term { get; set; } = null!;
}
