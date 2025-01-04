using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderActivitySql => @"
    SELECT
        [Id],
        [Order],
        [Date],
        [OrderActivityType],
        [OperatedBy]
    FROM 
        dbo.[OrderActivity]";

    public static DbQuery<OrderActivitySqlResponse> OrderActivitySqlQuery 
    => new(OrderActivitySql);
}

public class OrderActivitySqlResponse
{
    public long Id { get; set; }
    public long Order { get; set; }
    public DateTime Date { get; set; }
    public byte OrderActivityType { get; set; }
    public int OperatedBy { get; set; }
}
