using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{
    public static async Task<List<OrderYearlySummary>> GetOrderSummaryAsync(
              IDbConnection connection,
              bool isPaid,
              byte paymentType,
              int orderYear,
              IDbTransaction? transaction = null)
    {
        const string query = @"
                    SELECT 
                        p.OrderYear, 
                        p.OrderMonth, 
                        SUM(DeliveryFee) AS DeliveryFeeSum,
                        SUM(SystemFee) AS SystemFeeSum,
                        SUM(ItemsPrice) AS ItemsPriceSum,
                        SUM(TotalPrice) AS TotalPriceSum
                    FROM
                    (
                        SELECT YEAR([Date]) AS OrderYear, MONTH([Date]) AS OrderMonth, IsPaid, PaymentType, DeliveryFee, SystemFee, ItemsPrice, TotalPrice
                        FROM [dbo].[Order]
                    ) p
                    WHERE p.IsPaid = @IsPaid 
                        AND p.PaymentType = @PaymentType 
                        AND p.OrderYear = @OrderYear
                    GROUP BY p.OrderYear, p.OrderMonth";

        var parameters = new
        {
            IsPaid = isPaid,
            PaymentType = paymentType,
            OrderYear = orderYear
        };

        var result = await connection.QueryAsync<OrderYearlySummary>(query, parameters, transaction);

        return result.ToList();
    }
}

public class OrderYearlySummary
{
    public int OrderYear { get; set; }
    public int OrderMonth { get; set; }
    public decimal DeliveryFeeSum { get; set; }
    public decimal SystemFeeSum { get; set; }
    public decimal ItemsPriceSum { get; set; }
    public decimal TotalPriceSum { get; set; }
}