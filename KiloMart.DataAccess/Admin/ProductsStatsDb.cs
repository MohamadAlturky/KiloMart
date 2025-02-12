using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{
    public static async Task<(ProductDetail ProductDetail, int OrderCount)> GetProductDetailsAndOrderCountAsync(int productId, byte language, IDbConnection connection)
    {
        const string productDetailQuery = @"
        SELECT TOP(1)
                pd.[ProductId],
                pd.[ProductImageUrl],
                pd.[ProductIsActive],
                pd.[ProductMeasurementUnit],
                pd.[ProductDescription],
                pd.[ProductName],
                pd.[ProductCategoryId],
                pd.[ProductCategoryIsActive],
                pd.[ProductCategoryName],
                pd.[DealId],
                pd.[DealEndDate],
                pd.[DealStartDate],
                pd.[DealIsActive],
                pd.[DealOffPercentage],
                po.MaxPrice, 
                po.MinPrice
            FROM 
                dbo.GetProductDetailsFN(@Language) pd
            INNER JOIN (
                SELECT 
                    [Product], 
                    MAX([Price]) AS MaxPrice, 
                    MIN([Price]) AS MinPrice,
                    SUM(Quantity) AS Quantity
                FROM 
                    [ProductOffer]
                WHERE 
                    [IsActive] = 1
                GROUP BY 
                    [Product]
            ) po ON pd.[ProductId] = po.[Product]
            WHERE 
                pd.[ProductId] = @Product";

        const string orderCountQuery = @"
        SELECT COUNT(*) OrderCount 
        FROM OrderProduct 
        WHERE [Product] = @Product";

        var productDetail = await connection.QueryFirstOrDefaultAsync<ProductDetail>(productDetailQuery, new
        {
            Product = productId,
            Language = language
        });

        var orderCount = await connection.QueryFirstOrDefaultAsync<OrderCountData>(orderCountQuery, new
        {
            Product = productId
        });

        return (productDetail, orderCount?.OrderCount ?? 0);
    }
}
public class ProductDetail
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
    public int DealId { get; set; }
    public DateTime DealEndDate { get; set; }
    public DateTime DealStartDate { get; set; }
    public bool DealIsActive { get; set; }
    public decimal DealOffPercentage { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinPrice { get; set; }
}

public class OrderCountData
{
    public int OrderCount { get; set; }
}