using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderCustomerInformationSql => @"
    SELECT
        [Id],
        [Order],
        [Customer],
        [Location]
    FROM 
        dbo.[OrderCustomerInformation]";

    public static DbQuery<OrderCustomerInformationSqlResponse> OrderCustomerInformationSqlQuery 
    => new(OrderCustomerInformationSql);
}

public class OrderCustomerInformationSqlResponse
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Customer { get; set; }
    public int Location { get; set; }
}
