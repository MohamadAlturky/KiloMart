using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Admin;
public static partial class Stats
{
    public static async Task<IEnumerable<ToSellingProviderProductDetailDto>> ToSelling(
        IDbConnection connection,
        byte language,
        int count,
        DateTime dateUpper,
        DateTime dateLower)
    {
        const string query = @"
        SELECT 
            [ProductId],
            [ProductImageUrl],
            [ProductIsActive],
            [ProductMeasurementUnit],
            [ProductDescription],
            [ProductName],
            [ProductCategoryId],
            [ProductCategoryIsActive],
            [ProductCategoryName],
            DealId,
            DealEndDate,
            DealStartDate,
            DealIsActive,
            DealOffPercentage,
            p.ProductOffer,
            p.Sum,
            p.UnitPrice AS PriceWhenSelled
        FROM [dbo].[GetProductDetailsWithInFavoriteAndInCartFN](@Language, 0) pd
        INNER JOIN [dbo].ProductOffer po 
            ON pd.[ProductId] = po.[Product] 
            AND po.Provider = 4
        INNER JOIN 
            (SELECT TOP(@Count)
                opof.ProductOffer, 
                opof.UnitPrice, 
                SUM(opof.Quantity) AS Sum
            FROM dbo.[OrderProductOffer] opof
            INNER JOIN dbo.[Order] o
                ON opof.[Order] = o.Id 
                AND o.OrderStatus = 5 
                AND o.[Date] <= @DateUpper 
                AND o.[Date] >= @DateLower
            GROUP BY opof.ProductOffer, opof.UnitPrice) p 
            ON p.ProductOffer = po.Id
        WHERE pd.[ProductIsActive] = 1 
            AND po.Quantity > 0
            ORDER BY p.Sum DESC
    ";

        return await connection.QueryAsync<ToSellingProviderProductDetailDto>(query, new
        {
            Language = language,
            Count = count,
            DateUpper = dateUpper,
            DateLower = dateLower
        });
    }

    public static async Task<AggregatedOrderMetrics?> GetOrderMetricsAsync(
        IDbConnection connection,
        int provider,
        DateTime dateUpper,
        DateTime dateLower)
    {
        const string query = @"
        SELECT 
            AVG(o.[TotalPrice]) AS TotalPriceAvg,
            AVG(o.[ItemsPrice]) AS ItemsPriceAvg
        FROM [dbo].[Order] o
        INNER JOIN [dbo].[OrderProviderInformation] opi 
            ON opi.[Order] = o.Id
        WHERE opi.[Provider] = @Provider 
            AND o.OrderStatus = 5
            AND o.[Date] <= @DateUpper 
            AND o.[Date] >= @DateLower";

        return await connection.QueryFirstOrDefaultAsync<AggregatedOrderMetrics>(query, new
        {
            Provider = provider,
            DateUpper = dateUpper,
            DateLower = dateLower
        });
    }


    public static async Task<int> GetCompletedOrdersCountAsync(
        IDbConnection connection,
        int provider,
        DateTime dateUpper,
        DateTime dateLower)
    {
        const string query = @"
        SELECT COUNT(o.[Id]) AS CountId
        FROM [dbo].[Order] o
        INNER JOIN [dbo].[OrderProviderInformation] opi 
            ON opi.[Order] = o.Id
        WHERE opi.[Provider] = @Provider 
            AND o.OrderStatus = 5
            AND o.[Date] <= @DateUpper 
            AND o.[Date] >= @DateLower";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Provider = provider,
            DateUpper = dateUpper,
            DateLower = dateLower
        });
    }

    public static async Task<IEnumerable<MonthlyOrderMetrics>> GetMonthlyOrderMetricsAsync(
        IDbConnection connection,
        int provider,
        DateTime dateUpper,
        DateTime dateLower)
    {
        const string query = @"
        SELECT 
            AVG(o.[TotalPrice]) AS TotalPriceAvg,
            AVG(o.[ItemsPrice]) AS ItemsPriceAvg,
            MONTH(o.Date) AS Month,
            YEAR(o.Date) AS Year
        FROM [dbo].[Order] o
        INNER JOIN [dbo].[OrderProviderInformation] opi 
            ON opi.[Order] = o.Id
        WHERE opi.[Provider] = @Provider 
            AND o.OrderStatus = 5
            AND o.[Date] <= @DateUpper 
            AND o.[Date] >= @DateLower
        GROUP BY o.[ItemsPrice], o.[ItemsPrice], MONTH(o.Date), YEAR(o.Date)";

        return await connection.QueryAsync<MonthlyOrderMetrics>(query, new
        {
            Provider = provider,
            DateUpper = dateUpper,
            DateLower = dateLower
        });
    }
}

public class ToSellingProviderProductDetailDto
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; }
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; }
    public string ProductDescription { get; set; }
    public string ProductName { get; set; }
    public int ProductCategoryId { get; set; }
    public bool ProductCategoryIsActive { get; set; }
    public string ProductCategoryName { get; set; }
    public int? DealId { get; set; }
    public DateTime? DealEndDate { get; set; }
    public DateTime? DealStartDate { get; set; }
    public bool? DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }
    public int ProductOffer { get; set; }
    public decimal Sum { get; set; }
    public decimal PriceWhenSelled { get; set; }
}



public class AggregatedOrderMetrics
{
    public decimal TotalPriceAvg { get; set; }
    public decimal ItemsPriceAvg { get; set; }
}

public class OrderCountResult
{
    public int CountId { get; set; }
}
public class MonthlyOrderMetrics
{
    public decimal TotalPriceAvg { get; set; }
    public decimal ItemsPriceAvg { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
