using Dapper;
using KiloMart.Core.Models;
using KiloMart.Requests.Queries;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for GetProductDetailsFN
/// </summary>
public static partial class Db
{
    public static async Task<List<ProductDetailWithPricingWithInFavoriteAndOnCart>> GetBestDealsWithInFavoriteAndOnCartWithInLocationCircle(
        byte language,
        int totalCount,
        int customer,
        decimal distanceInKm,
        decimal longitude,
        decimal latitude,
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
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
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
            Customer = customer,
            DistanceInKm = distanceInKm,
            Longitude = longitude,
            Latitude = latitude
        });

        return result.ToList();
    }

    public static async Task<IEnumerable<ProductDetailWithInFavoriteAndOnCart>> GetTopSellingProductDetailsWithInLocationCircleAsync(
        byte language,
        int customer,
        int count,
        decimal distanceInKm,
        decimal longitude,
        decimal latitude,
        IDbConnection connection)
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
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
            GROUP BY 
                [Product]

        ) po ON pd.[ProductId] = po.[Product]
        WHERE pd.[ProductIsActive] = 1 AND po.Quantity > 0";

        return await connection.QueryAsync<ProductDetailWithInFavoriteAndOnCart>(query, new
        {
            Language = language,
            Customer = customer,
            Count = count,
            DistanceInKm = distanceInKm,
            Longitude = longitude,
            Latitude = latitude
        });
    }

    public static async Task<PaginatedResult<ProductDetailWithPricingWithInFavoriteAndOnCart>> GetProductDetailsWithPricingWithInFavoriteAndOnCartWithInLocationCircleAsync(
        byte language,
        int pageNumber,
        int pageSize,
        int customer,
        decimal distanceInKm,
        decimal longitude,
        decimal latitude,
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
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
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
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
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
            EndRow = endRow,
            DistanceInKm = distanceInKm,
            Longitude = longitude,
            Latitude = latitude
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

    public static async Task<PaginatedResult<ProductDetailWithPricingWithInFavoriteAndOnCart>> GetProductDetailsWithPricingByCategoryWithInFavoriteAndOnCartWithInLocationCircleAsync(
        byte language,
        int pageNumber,
        int pageSize,
        int categoryId,
        int customer,
        decimal distanceInKm,
        decimal longitude,
        decimal latitude,
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
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
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
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
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
            CategoryId = categoryId,
            DistanceInKm = distanceInKm,
            Longitude = longitude,
            Latitude = latitude
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

    public static async Task<IEnumerable<ProductDetailWithPricingWithInFavoriteAndOnCart>> SearchProductDetailsForCustomerWithInLocationCircleAsync
    (
        int top,
        byte language,
        int customer,
        string searchTerm,
        decimal distanceInKm,
        decimal longitude,
        decimal latitude,
        IDbConnection connection)
    {
        const string query = @"
        SELECT TOP(@Top)
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
            po.MinPrice
        FROM 
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@Language, @Customer) pd
        INNER JOIN (
            
            
            SELECT 
                [Product], 
                MAX([Price]) AS MaxPrice, 
                MIN([Price]) AS MinPrice,
                SUM(Quantity) AS Quantity
            FROM 
                [ProductOffer] po
                INNER JOIN [Location] l 
                ON 
                    l.[Party] = po.[Provider] AND
                    l.IsActive = 1 AND
                    dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm
            WHERE 
                po.[IsActive] = 1
            GROUP BY 
                [Product]

        ) po ON pd.[ProductId] = po.[Product]
        WHERE 
            pd.[ProductIsActive] = 1 AND 
            po.Quantity > 0 AND 
            pd.ProductName LIKE '%' + @SearchTerm + '%'";

        return await connection.QueryAsync<ProductDetailWithPricingWithInFavoriteAndOnCart>(query, new
        {
            Language = language,
            Customer = customer,
            SearchTerm = searchTerm,
            Top = top,
            DistanceInKm = distanceInKm,
            Longitude = longitude,
            Latitude = latitude
        });
    }
}
