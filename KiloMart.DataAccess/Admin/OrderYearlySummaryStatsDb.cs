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

    public static async Task<List<OrderDailySummary>> GetOrderSummaryDailyAsync(
              IDbConnection connection,
              bool isPaid,
              byte paymentType,
              int orderYear,
              int orderMonth,
              IDbTransaction? transaction = null)
    {
        const string query = @"
                SELECT 
                    p.OrderYear, 
                    p.OrderMonth, 
                    p.OrderDay,
                    SUM(DeliveryFee) AS DeliveryFeeSum,
                    SUM(SystemFee) AS SystemFeeSum,
                    SUM(ItemsPrice) AS ItemsPriceSum,
                    SUM(TotalPrice) AS TotalPriceSum
                FROM
                (
                    SELECT 
                        YEAR([Date]) AS OrderYear, 
                        MONTH([Date]) AS OrderMonth, 
                        DAY([Date]) AS OrderDay,
                        IsPaid, 
                        PaymentType, 
                        DeliveryFee, 
                        SystemFee, 
                        ItemsPrice, 
                        TotalPrice
                    FROM [dbo].[Order]
                ) p
                WHERE p.IsPaid = @IsPaid 
                    AND p.PaymentType = @PaymentType 
                    AND p.OrderYear = @OrderYear
                    AND p.OrderMonth = @OrderMonth
                GROUP BY p.OrderYear, p.OrderMonth, p.OrderDay";

        var parameters = new
        {
            IsPaid = isPaid,
            PaymentType = paymentType,
            OrderYear = orderYear,
            OrderMonth = orderMonth
        };

        var result = await connection.QueryAsync<OrderDailySummary>(query, parameters, transaction);

        return result.ToList();
    }

    public static async Task<List<OrderYearlySummary>> GetOrderSummaryAsync(
              IDbConnection connection,
              bool isPaid,
              byte paymentType,
              int orderYear,
              int orderMonth,
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
                        AND p.OrderMonth = @OrderMonth
                    GROUP BY p.OrderYear, p.OrderMonth";

        var parameters = new
        {
            IsPaid = isPaid,
            PaymentType = paymentType,
            OrderYear = orderYear,
            OrderMonth = orderMonth
        };

        var result = await connection.QueryAsync<OrderYearlySummary>(query, parameters, transaction);

        return result.ToList();
    }
    public static async Task<List<OrderCountMonthlySummary>> GetOrderCountSummaryAsync(
        IDbConnection connection,
        bool isPaid,
        byte paymentType,
        int orderYear,
        int orderStatus,
        IDbTransaction? transaction = null)
    {
        const string query = @"
        SELECT 
            p.OrderYear, 
            p.OrderMonth, 
            COUNT(*) AS TotalCount
        FROM
        (
            SELECT YEAR([Date]) AS OrderYear, 
                   MONTH([Date]) AS OrderMonth, 
                   IsPaid, 
                   PaymentType, 
                   OrderStatus
            FROM [dbo].[Order]
        ) p
        WHERE 
            (p.IsPaid = @IsPaid)
            AND (p.PaymentType = @PaymentType) 
            AND (p.OrderYear = @OrderYear)
            AND (p.OrderStatus = @OrderStatus)
        GROUP BY p.OrderYear, p.OrderMonth";

        var parameters = new
        {
            IsPaid = isPaid,
            PaymentType = paymentType,
            OrderYear = orderYear,
            OrderStatus = orderStatus
        };

        var result = await connection.QueryAsync<OrderCountMonthlySummary>(query, parameters, transaction);

        return result.ToList();
    }
    public static async Task<List<OrderCountMonthlySummary>> GetOrderCountSummaryAsync(
        IDbConnection connection,
        bool isPaid,
        byte paymentType,
        int orderYear,
        int orderMonth, // Added parameter for month
        int orderStatus,
        IDbTransaction? transaction = null)
    {
        const string query = @"
    SELECT 
        p.OrderYear, 
        p.OrderMonth, 
        p.OrderDay, 
        COUNT(*) AS TotalCount
    FROM
    (
        SELECT YEAR([Date]) AS OrderYear, 
               MONTH([Date]) AS OrderMonth, 
               DAY([Date]) AS OrderDay,  -- Extracting day
               IsPaid, 
               PaymentType, 
               OrderStatus
        FROM [dbo].[Order]
    ) p
    WHERE 
        (p.IsPaid = @IsPaid)
        AND (p.PaymentType = @PaymentType) 
        AND (p.OrderYear = @OrderYear)
        AND (p.OrderMonth = @OrderMonth)  -- Filtering by month
        AND (p.OrderStatus = @OrderStatus)
    GROUP BY p.OrderYear, p.OrderMonth, p.OrderDay";  // Grouping by year, month, and day

        var parameters = new
        {
            IsPaid = isPaid,
            PaymentType = paymentType,
            OrderYear = orderYear,
            OrderMonth = orderMonth,  // Added month parameter
            OrderStatus = orderStatus
        };

        var result = await connection.QueryAsync<OrderCountMonthlySummary>(query, parameters, transaction);

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

public class OrderDailySummary
{
    public int OrderYear { get; set; }
    public int OrderMonth { get; set; }
    public int OrderDay { get; set; }
    public decimal DeliveryFeeSum { get; set; }
    public decimal SystemFeeSum { get; set; }
    public decimal ItemsPriceSum { get; set; }
    public decimal TotalPriceSum { get; set; }
}

public class OrderCountMonthlySummary
{

    public int OrderYear { get; set; }
    public int OrderMonth { get; set; }
    public int OrderDay { get; set; }
    public int TotalCount { get; set; }
}
