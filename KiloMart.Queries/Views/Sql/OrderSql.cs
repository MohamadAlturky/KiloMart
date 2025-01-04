using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string OrderSql => @"
    SELECT
        [Id],
        [OrderStatus],
        [TotalPrice],
        [TransactionId],
        [Date],
        [PaymentType],
        [IsPaid],
        [DeliveryFee],
        [SystemFee],
        [ItemsPrice],
        [SpecialRequest]
    FROM 
        dbo.[Order]";

    public static DbQuery<OrderSqlResponse> OrderSqlQuery 
    => new(OrderSql);
}

public class OrderSqlResponse
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;
    public DateTime Date { get; set; }
    public byte PaymentType { get; set; }
    public bool IsPaid { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal SystemFee { get; set; }
    public decimal ItemsPrice { get; set; }
    public string? SpecialRequest { get; set; }
}
