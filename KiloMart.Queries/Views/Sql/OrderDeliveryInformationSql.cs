using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderDeliveryInformationSql => @"
    SELECT
        [Id],
        [Order],
        [Delivery]
    FROM 
        dbo.[OrderDeliveryInformation]";

    public static DbQuery<OrderDeliveryInformationSqlResponse> OrderDeliveryInformationSqlQuery 
    => new(OrderDeliveryInformationSql);
}

public class OrderDeliveryInformationSqlResponse
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Delivery { get; set; }
}
