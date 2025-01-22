using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for Favorite Products
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertFavoriteProductAsync(IDbConnection connection,
        int customer,
        int product,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[FavoriteProducts]
                            ([Customer], [Product])
                            VALUES (@Customer, @Product)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Customer = customer,
            Product = product
        }, transaction);
    }

    public static async Task<bool> DeleteFavoriteProductAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[FavoriteProducts]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<IEnumerable<FavoriteProduct>> GetFavoriteProductsByCustomerIdAsync(int customerId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Customer], 
                            [Product]
                            FROM [dbo].[FavoriteProducts]
                            WHERE [Customer] = @CustomerId";

        return await connection.QueryAsync<FavoriteProduct>(query, new
        {
            CustomerId = customerId
        });
    }
    public static async Task<FavoriteProduct?> GetFavoriteProductsByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Customer], 
                            [Product]
                            FROM [dbo].[FavoriteProducts]
                            WHERE [Id] = @id";

        return await connection.QueryFirstOrDefaultAsync<FavoriteProduct>(query, new
        {
            id = id
        });
    }
    public static async Task<FavoriteProduct?> GetFavoriteProductsByCustomerAndProductAsync(int customer, int product, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Customer], 
                            [Product]
                            FROM [dbo].[FavoriteProducts]
                            WHERE [Product] = @Product AND [Customer] = @Customer";

        return await connection.QueryFirstOrDefaultAsync<FavoriteProduct>(query, new
        {
            Customer = customer,
            Product = product
        });
    }
    public static async Task<IEnumerable<FavoriteProductDetails>> GetFavoriteProductDetailsByCustomerIdAsync(int customerId, byte language, IDbConnection connection)
    {
        const string query = @"
        SELECT 
            [Id] AS FavoriteId,
            p.ProductId,
            p.ProductDescription,
            p.ProductImageUrl,
            p.ProductIsActive,
            p.ProductMeasurementUnit,
            p.ProductName,
            p.ProductProductCategory,
            c.ProductCategoryName
        FROM [dbo].[FavoriteProducts] 
        INNER JOIN GetProductDetailsByLanguageFN(@language) p ON p.ProductId = FavoriteProducts.Product
        INNER JOIN GetProductCategoryDetailsByLanguageFN(@language) c ON c.ProductCategoryId = p.ProductProductCategory 
        WHERE [Customer] = @customer";

        return await connection.QueryAsync<FavoriteProductDetails>(query, new
        {
            customer = customerId,
            language = language
        });
    }



    public static async Task<IEnumerable<FavoriteProductWithPricing>> GetFavoriteProductsByCustomerWithInfoAndPricingAsync(
    int customerId,
    byte language,
    decimal distanceInKm,
    decimal? longitude,
    decimal? latitude,
    IDbConnection connection)
    {
        const string query = @"
    SELECT 
        f.Id as FavoriteProductId,
        f.Product as FavoriteProduct,
        p.[ProductId],
        p.[ProductImageUrl],
        p.[ProductIsActive],
        p.[ProductMeasurementUnit],
        p.[ProductDescription],
        p.[ProductName],
        p.[ProductCategoryId],
        p.[ProductCategoryIsActive],
        p.[ProductCategoryName],
        p.[DealId],
        p.[DealEndDate],
        p.[DealStartDate],
        p.[DealIsActive],
        p.[DealOffPercentage],
        p.[InCart],
        p.[InFavorite],
        p.MaxPrice, 
        p.MinPrice
    FROM 
    (
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
            po.MinPrice
        FROM 
            dbo.GetProductDetailsWithInFavoriteAndInCartFN(@language, @customer) pd
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
                    (dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @DistanceInKm OR (@Longitude IS NULL OR @Latitude IS NULL))
            WHERE 
                po.[IsActive] = 1
            GROUP BY 
                [Product]
        ) po ON pd.[ProductId] = po.[Product]
        WHERE 
            pd.[ProductIsActive] = 1
    ) p
    INNER JOIN [dbo].[FavoriteProducts] f ON p.ProductId = f.Product
    WHERE f.Customer = @customer";

        return await connection.QueryAsync<FavoriteProductWithPricing>(query, new
        {
            Customer = customerId,
            Language = language,
            DistanceInKm = distanceInKm,
            Longitude = longitude,
            Latitude = latitude
        });
    }

}

public class FavoriteProduct
{
    public long Id { get; set; }
    public int Customer { get; set; }
    public int Product { get; set; }
}
public class FavoriteProductDetails
{
    public long FavoriteId { get; set; }
    public int ProductId { get; set; }
    public string ProductDescription { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductProductCategory { get; set; }
    public string ProductCategoryName { get; set; } = null!;
}



public class FavoriteProductWithPricing
{
    public long FavoriteProductId { get; set; }        // f.Id as FavoriteProductId
    public int FavoriteProduct { get; set; }            // f.Product as FavoriteProduct

    public int ProductId { get; set; }                  // p.ProductId
    public string ProductImageUrl { get; set; }         // p.ProductImageUrl
    public bool ProductIsActive { get; set; }           // p.ProductIsActive
    public string ProductMeasurementUnit { get; set; }  // p.ProductMeasurementUnit
    public string ProductDescription { get; set; }      // p.ProductDescription
    public string ProductName { get; set; }             // p.ProductName
    public int ProductCategoryId { get; set; }          // p.ProductCategoryId
    public bool ProductCategoryIsActive { get; set; }   // p.ProductCategoryIsActive
    public string ProductCategoryName { get; set; }     // p.ProductCategoryName

    public int? DealId { get; set; }                    // p.DealId (nullable if there are cases without a deal)
    public DateTime? DealEndDate { get; set; }          // p.DealEndDate (nullable)
    public DateTime? DealStartDate { get; set; }        // p.DealStartDate (nullable)
    public bool DealIsActive { get; set; }              // p.DealIsActive
    public decimal? DealOffPercentage { get; set; }      // p.DealOffPercentage (nullable)

    public bool InCart { get; set; }                    // p.InCart
    public bool InFavorite { get; set; }                 // p.InFavorite

    public decimal MaxPrice { get; set; }               // MaxPrice
    public decimal MinPrice { get; set; }               // MinPrice
}
