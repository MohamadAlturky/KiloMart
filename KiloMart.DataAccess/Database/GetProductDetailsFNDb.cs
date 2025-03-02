using Dapper;
using KiloMart.Core.Models;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for GetProductDetailsFN
/// </summary>
public static partial class Db
{
    public static async Task<List<ProductDetailWithPricing>> GetBestDeals(
        byte language,
        int totalCount,
        IDbConnection connection)
    {
        const string query = @"
        SELECT TOP(@TotalCount)
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
            po.MinPrice,
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
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
            pd.[ProductIsActive] = 1 
            AND po.Quantity > 0 
            AND pd.[DealOffPercentage] IS NOT NULL
        ORDER BY 
            [DealOffPercentage];";

        var result = await connection.QueryAsync<ProductDetailWithPricing>(query, new
        {
            Language = language,
            TotalCount = totalCount
        });

        return result.ToList();
    }

    public static async Task<IEnumerable<ProductDetail>> GetProductDetailsAsync(byte language, IDbConnection connection)
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
            [DealId],
            [DealEndDate],
            [DealStartDate],
            [DealIsActive],
            [DealOffPercentage]
        FROM dbo.GetProductDetailsFN(@Language)";

        return await connection.QueryAsync<ProductDetail>(query, new
        {
            Language = language
        });
    }

    public static async Task<IEnumerable<ProductDetail>> GetProductDetailsFilteredAsync(
        IDbConnection connection,
        byte language,
        int? productCategoryId = null,
        bool? productIsActive = null,
        bool? dealIsActive = null,
        int pageNumber = 1,
        int pageSize = 10)
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
            [DealId],
            [DealEndDate],
            [DealStartDate],
            [DealIsActive],
            [DealOffPercentage]
        FROM dbo.GetProductDetailsFN(@Language)
        WHERE (@ProductCategoryId IS NULL OR [ProductCategoryId] = @ProductCategoryId)
          AND (@ProductIsActive IS NULL OR [ProductIsActive] = @ProductIsActive)
          AND (@DealIsActive IS NULL OR [DealIsActive] = @DealIsActive)
        ORDER BY [ProductId] DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (pageNumber - 1) * pageSize;

        return await connection.QueryAsync<ProductDetail>(query, new
        {
            Language = language,
            ProductCategoryId = productCategoryId,
            ProductIsActive = productIsActive,
            DealIsActive = dealIsActive,
            Offset = offset,
            PageSize = pageSize
        });
    }
    public static async Task<int?> GetCOUNTProductDetailsFilteredAsync(
        IDbConnection connection,
        byte language,
        int? productCategoryId = null,
        bool? productIsActive = null,
        bool? dealIsActive = null)
    {
        const string query = @"
        SELECT 
            COUNT([ProductId])
        FROM dbo.GetProductDetailsFN(@Language)
        WHERE (@ProductCategoryId IS NULL OR [ProductCategoryId] = @ProductCategoryId)
          AND (@ProductIsActive IS NULL OR [ProductIsActive] = @ProductIsActive)
          AND (@DealIsActive IS NULL OR [DealIsActive] = @DealIsActive)";


        return await connection.QueryFirstOrDefaultAsync<int>(query, new
        {
            Language = language,
            ProductCategoryId = productCategoryId,
            ProductIsActive = productIsActive,
            DealIsActive = dealIsActive
        });
    }

    public static async Task<PaginatedResult<ProductDetailWithPricing>> GetProductDetailsWithPricingAsync(
    byte language,
    int pageNumber,
    int pageSize,
    IDbConnection connection)
    {
        const string query = @"
    SELECT * FROM (
        SELECT 
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
            po.MinPrice,
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
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
        WHERE pd.[ProductIsActive] = 1 AND po.Quantity > 0
    ) AS ProductDetails
    WHERE RowNum BETWEEN @StartRow AND @EndRow;

    SELECT COUNT(*) FROM (
        SELECT 
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
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
        WHERE pd.[ProductIsActive] = 1 AND po.Quantity > 0
    ) AS ProductCount;";

        var startRow = (pageNumber - 1) * pageSize + 1;
        var endRow = startRow + pageSize - 1;

        using (var multi = await connection.QueryMultipleAsync(query, new
        {
            Language = language,
            StartRow = startRow,
            EndRow = endRow
        }))
        {
            var items = multi.Read<ProductDetailWithPricing>().ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ProductDetailWithPricing>
            {
                Items = [.. items],
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
    public static async Task<PaginatedResult<ProductDetailWithPricing>> GetProductDetailsWithPricingByCategoryAsync(
        byte language,
        int pageNumber,
        int pageSize,
        int categoryId,
        IDbConnection connection)
    {
        const string query = @"
    SELECT * FROM (
        SELECT 
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
            po.MinPrice,
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
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
            pd.[ProductIsActive] = 1 AND 
            po.Quantity > 0 AND 
            pd.[ProductCategoryId] = @CategoryId 
    ) AS ProductDetails
    WHERE RowNum BETWEEN @StartRow AND @EndRow;

    SELECT COUNT(*) FROM (
        SELECT 
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
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
            pd.[ProductIsActive] = 1 AND 
            po.Quantity > 0 AND 
            pd.[ProductCategoryId] = @CategoryId
    ) AS ProductCount;";

        var startRow = (pageNumber - 1) * pageSize + 1;
        var endRow = startRow + pageSize - 1;

        using (var multi = await connection.QueryMultipleAsync(query, new
        {
            Language = language,
            StartRow = startRow,
            EndRow = endRow,
            CategoryId = categoryId
        }))
        {
            var items = multi.Read<ProductDetailWithPricing>().ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ProductDetailWithPricing>
            {
                Items = [.. items],
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }


}
public class ProductDetail
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductCategoryId { get; set; }
    public bool ProductCategoryIsActive { get; set; }
    public string ProductCategoryName { get; set; } = null!;
    public int? DealId { get; set; }
    public DateTime? DealEndDate { get; set; }
    public DateTime? DealStartDate { get; set; }
    public bool? DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }
}

public class ProductDetailWithPricing
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductCategoryId { get; set; }
    public bool ProductCategoryIsActive { get; set; }
    public string ProductCategoryName { get; set; } = null!;
    public int? DealId { get; set; }
    public DateTime? DealEndDate { get; set; }
    public DateTime? DealStartDate { get; set; }
    public bool? DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinPrice { get; set; }
}
