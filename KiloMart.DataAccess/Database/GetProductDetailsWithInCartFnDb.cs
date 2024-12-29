using Dapper;
using KiloMart.Core.Models;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for GetProductDetailsFN
/// </summary>
public static partial class Db
{
    public static async Task<List<ProductDetailWithPricingWithInFavoriteAndOnCart>> GetBestDealsWithInFavoriteAndOnCart(
        byte language,
        int totalCount,
        int customer,
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
            pd.[InCart],
            pd.[InFavorite],
            po.MaxPrice, 
            po.MinPrice,
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
        FROM 
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer) pd
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

        var result = await connection.QueryAsync<ProductDetailWithPricingWithInFavoriteAndOnCart>(query, new
        {
            Language = language,
            TotalCount = totalCount,
            Customer = customer
        });

        return result.ToList();
    }

    public static async Task<IEnumerable<ProductDetailWithInFavoriteAndOnCart>> GetProductDetailsWithInFavoriteAndOnCartAsync(byte language, int customer, IDbConnection connection)
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
            [DealOffPercentage],
            [InCart],
            [InFavorite]
        FROM dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer)";

        return await connection.QueryAsync<ProductDetailWithInFavoriteAndOnCart>(query, new
        {
            Language = language,
            Customer = customer
        });
    }
    public static async Task<IEnumerable<ProductDetailWithInFavoriteAndOnCart>> GetTopSellingProductDetailsAsync(byte language, int customer, int count, IDbConnection connection)
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
            InCart,
            InFavorite, 
			po.MaxPrice AS MaxPrice, 
            po.MinPrice AS MinPrice 
        FROM [dbo].[GetProductDetailsWithInFavoriteAndInCartFN](@Language, @Customer) pd
        INNER JOIN 
        (SELECT TOP(@Count)
            Product, COUNT(Product) Count
            FROM dbo.[OrderProduct]
            GROUP BY Product) p ON p.Product = pd.ProductId
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
        WHERE pd.[ProductIsActive] = 1 AND po.Quantity > 0";

        return await connection.QueryAsync<ProductDetailWithInFavoriteAndOnCart>(query, new
        {
            Language = language,
            Customer = customer,
            Count = count
        });
    }

    public static async Task<IEnumerable<ProductDetailWithInFavoriteAndOnCart>> GetProductDetailsFilteredWithInFavoriteAndOnCartAsync(
        IDbConnection connection,
        byte language,
        int customer,
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
            [DealOffPercentage],
            [InCart],
            [InFavorite]
        FROM dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer)
        WHERE (@ProductCategoryId IS NULL OR [ProductCategoryId] = @ProductCategoryId)
          AND (@ProductIsActive IS NULL OR [ProductIsActive] = @ProductIsActive)
          AND (@DealIsActive IS NULL OR [DealIsActive] = @DealIsActive)
        ORDER BY [ProductId] DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (pageNumber - 1) * pageSize;

        return await connection.QueryAsync<ProductDetailWithInFavoriteAndOnCart>(query, new
        {
            Language = language,
            Customer = customer,
            ProductCategoryId = productCategoryId,
            ProductIsActive = productIsActive,
            DealIsActive = dealIsActive,
            Offset = offset,
            PageSize = pageSize
        });
    }

    public static async Task<int?> GetCOUNTProductDetailsFilteredWithInFavoriteAndOnCartAsync(
        IDbConnection connection,
        byte language,
        int customer,
        int? productCategoryId = null,
        bool? productIsActive = null,
        bool? dealIsActive = null)
    {
        const string query = @"
        SELECT 
            COUNT([ProductId])
        FROM dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer)
        WHERE (@ProductCategoryId IS NULL OR [ProductCategoryId] = @ProductCategoryId)
          AND (@ProductIsActive IS NULL OR [ProductIsActive] = @ProductIsActive)
          AND (@DealIsActive IS NULL OR [DealIsActive] = @DealIsActive)";

        return await connection.QueryFirstOrDefaultAsync<int>(query, new
        {
            Language = language,
            Customer = customer,
            ProductCategoryId = productCategoryId,
            ProductIsActive = productIsActive,
            DealIsActive = dealIsActive
        });
    }

    public static async Task<PaginatedResult<ProductDetailWithPricingWithInFavoriteAndOnCart>> GetProductDetailsWithPricingWithInFavoriteAndOnCartAsync(
        byte language,
        int pageNumber,
        int pageSize,
        int customer,
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
            pd.[InCart],
            pd.[InFavorite],
            po.MaxPrice, 
            po.MinPrice,
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
        FROM 
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer) pd
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
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer) pd
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
            Customer = customer,
            StartRow = startRow,
            EndRow = endRow
        }))
        {
            var items = multi.Read<ProductDetailWithPricingWithInFavoriteAndOnCart>().ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ProductDetailWithPricingWithInFavoriteAndOnCart>
            {
                Items = [.. items],
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }

    public static async Task<PaginatedResult<ProductDetailWithPricingWithInFavoriteAndOnCart>> GetProductDetailsWithPricingByCategoryWithInFavoriteAndOnCartAsync(
        byte language,
        int pageNumber,
        int pageSize,
        int categoryId,
        int customer,
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
            pd.[InCart],
            pd.[InFavorite],
            po.MaxPrice, 
            po.MinPrice,
            ROW_NUMBER() OVER (ORDER BY pd.[ProductId]) AS RowNum
        FROM 
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer) pd
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
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer) pd
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
            Customer = customer,
            StartRow = startRow,
            EndRow = endRow,
            CategoryId = categoryId
        }))
        {
            var items = multi.Read<ProductDetailWithPricingWithInFavoriteAndOnCart>().ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ProductDetailWithPricingWithInFavoriteAndOnCart>
            {
                Items = [.. items],
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}

public class ProductDetailWithInFavoriteAndOnCart
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
    public bool InCart { get; set; }
    public bool InFavorite { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinPrice { get; set; }
}

public class ProductDetailWithPricingWithInFavoriteAndOnCart
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
    public bool InCart { get; set; }
    public bool InFavorite { get; set; }
}
