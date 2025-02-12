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


    // public static async Task<IEnumerable<ProductOfferDetail>> GetProductOffersWithPaginationAsync(
    // int pageNumber,
    // int pageSize,
    // string language,
    // IDbConnection connection)
    // {
    //     const string query = @"
    //     SELECT 
    //         po.[Id],
    //         po.[Price],
    //         po.[OffPercentage],
    //         po.[FromDate],
    //         po.[ToDate],
    //         po.[Quantity],
    //         po.[Provider],
    //         po.[IsActive],
    //         p.Id AS ProviderId,
    //         p.DisplayName AS ProviderDisplayName,
    //         p.IsActive AS ProviderIsActive,
    //         pd.[ProductId],
    //         pd.[ProductImageUrl],
    //         pd.[ProductIsActive],
    //         pd.[ProductMeasurementUnit],
    //         pd.[ProductDescription],
    //         pd.[ProductName],
    //         pd.[ProductCategoryId],
    //         pd.[ProductCategoryIsActive],
    //         pd.[ProductCategoryName],
    //         pd.[DealId],
    //         pd.[DealEndDate],
    //         pd.[DealStartDate],
    //         pd.[DealIsActive],
    //         pd.[DealOffPercentage],
    //         (SELECT COUNT(*) FROM OrderProductOffer ops WHERE ops.ProductOffer = po.[Id]) AS TotalOrders
    //     FROM 
    //         [dbo].[ProductOffer] po
    //     INNER JOIN 
    //         [dbo].[Party] p ON p.Id = po.[Provider]
    //     INNER JOIN 
    //         dbo.GetProductDetailsFN(@Language) pd ON pd.ProductId = po.Product
    //     WHERE 
    //         po.[IsActive] = 1
    //     ORDER BY 
    //         po.[Id]
    //     OFFSET (@PageNumber - 1) * @PageSize ROWS
    //     FETCH NEXT @PageSize ROWS ONLY;";

    //     var parameters = new
    //     {
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         Language = language
    //     };

    //     return await connection.QueryAsync<ProductOfferDetail>(query, parameters);
    // }

    public static async Task<(IEnumerable<ProductOfferDetail> Results, int TotalCount)> GetProductOffersWithPaginationAsync(
        int pageNumber,
        int pageSize,
        byte languageId,
        int? categoryId,
        bool? isActive,
        IDbConnection connection)
    {
        // Query to get the total count of filtered records
        const string countQuery = @"
    SELECT COUNT(*)
    FROM 
        [dbo].[ProductOffer] po
    INNER JOIN 
        [dbo].[Party] p ON p.Id = po.[Provider]
    INNER JOIN 
        dbo.GetProductDetailsFN(@LanguageId) pd ON pd.ProductId = po.Product
    WHERE 
        (@CategoryId IS NULL OR pd.[ProductCategoryId] = @CategoryId)
        AND (@IsActive IS NULL OR po.[IsActive] = @IsActive);";

        // Query to get the paginated results
        const string dataQuery = @"
    SELECT 
        po.[Id],
        po.[Price],
        po.[OffPercentage],
        po.[FromDate],
        po.[ToDate],
        po.[Quantity],
        po.[Provider],
        po.[IsActive],
        p.Id AS ProviderId,
        p.DisplayName AS ProviderDisplayName,
        p.IsActive AS ProviderIsActive,
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
        (SELECT COUNT(*) FROM OrderProductOffer ops WHERE ops.ProductOffer = po.[Id]) AS TotalOrders
    FROM 
        [dbo].[ProductOffer] po
    INNER JOIN 
        [dbo].[Party] p ON p.Id = po.[Provider]
    INNER JOIN 
        dbo.GetProductDetailsFN(@LanguageId) pd ON pd.ProductId = po.Product
    WHERE 
        (@CategoryId IS NULL OR pd.[ProductCategoryId] = @CategoryId)
        AND (@IsActive IS NULL OR po.[IsActive] = @IsActive)
    ORDER BY 
        po.[Id]
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;";

        var parameters = new
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            LanguageId = languageId,
            CategoryId = categoryId,
            IsActive = isActive
        };

        // Execute the count query
        int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

        // Execute the data query
        var results = await connection.QueryAsync<ProductOfferDetail>(dataQuery, parameters);

        // Return the results and total count
        return (results, totalCount);
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


public class ProductOfferDetail
{
    public int Id { get; set; } // ProductOffer ID
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int Quantity { get; set; }
    public int Provider { get; set; }
    public bool IsActive { get; set; }
    public int ProviderId { get; set; }
    public string ProviderDisplayName { get; set; }
    public bool ProviderIsActive { get; set; }
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
    public int TotalOrders { get; set; } // Count of orders for this offer
}