using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DeliveryActivitySql => @"
    SELECT
        [Id],
        [Date],
        [Value],
        [Type],
        [Delivery]
    FROM 
        dbo.[DeliveryActivity]";

    public static DbQuery<DeliveryActivitySqlResponse> DeliveryActivitySqlQuery 
    => new(DeliveryActivitySql);
}

public class DeliveryActivitySqlResponse
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public byte Type { get; set; }
    public int Delivery { get; set; }
}
