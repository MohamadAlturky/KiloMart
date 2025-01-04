using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderProviderInformationSql => @"
    SELECT
        [Id],
        [Order],
        [Provider],
        [Location]
    FROM 
        dbo.[OrderProviderInformation]";

    public static DbQuery<OrderProviderInformationSqlResponse> OrderProviderInformationSqlQuery 
    => new(OrderProviderInformationSql);
}

public class OrderProviderInformationSqlResponse
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Provider { get; set; }
    public int Location { get; set; }
}
