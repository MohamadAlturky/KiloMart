using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for Cart
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertCartAsync(IDbConnection connection,
        int product,
        decimal quantity,
        int customer,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Cart]
                            ([Product], [Quantity], [Customer])
                            VALUES (@Product, @Quantity, @Customer)
                             SELECT CAST(SCOPE_IDENTITY() AS INT)";

        var id = await connection.ExecuteScalarAsync<long>(query, new
        {
            Product = product,
            Quantity = quantity,
            Customer = customer
        }, transaction);

        return id; // Return the ID of the inserted cart item
    }

    public static async Task<bool> UpdateCartAsync(IDbConnection connection,
        long id,
        int product,
        decimal quantity,
        int customer,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Cart]
                                SET 
                                [Product] = @Product,
                                [Quantity] = @Quantity,
                                [Customer] = @Customer
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Product = product,
            Quantity = quantity,
            Customer = customer
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteCartAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Cart]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Cart?> GetCartByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Product], 
                            [Quantity], 
                            [Customer]
                            FROM [dbo].[Cart]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Cart>(query, new
        {
            Id = id
        });
    }
    public static async Task<IEnumerable<Cart>> GetCartsByCustomerAsync(int customerId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Product], 
                            [Quantity], 
                            [Customer]
                            FROM [dbo].[Cart]
                            WHERE [Customer] = @CustomerId";

        return await connection.QueryAsync<Cart>(query, new
        {
            CustomerId = customerId
        });
    }
    public static async Task<IEnumerable<CartItemWithProductWithPricing>> GetCartsByCustomerWithProductsInfoAndPricingAsync(int customerId, byte language, IDbConnection connection)
    {
        const string query = @"
        SELECT 
            c.Id as CartItemId,
            c.Product as CartItemProduct,
            c.Quantity as CartItemQuantity,
            c.Customer as CartItemCustomer,
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
                    [ProductOffer]
                WHERE 
                    [IsActive] = 1
                GROUP BY 
                    [Product]
            ) po ON pd.[ProductId] = po.[Product]
            WHERE 
                pd.[ProductIsActive] = 1
        ) p
        INNER JOIN [dbo].[Cart] c ON p.ProductId = c.Product
        WHERE c.[Customer] = @customer";

        return await connection.QueryAsync<CartItemWithProductWithPricing>(query, new
        {
            Customer = customerId,
            Language = language
        });
    }

    public static async Task<IEnumerable<CartItemWithProduct>> GetCartsByCustomerWithProductsInfoAsync(int customerId, byte language, IDbConnection connection)
    {
        const string query = @"SELECT 
                    c.Id as CartItemId,
                    c.Product as CartItemProduct,
                    c.Quantity as CartItemQuantity,
                    c.Customer as CartItemCustomer,
                    p.ProductImageUrl as ProductImageUrl,
                    p.ProductProductCategory as ProductCategoryId,
                    p.ProductMeasurementUnit as ProductMeasurementUnit,
                    p.ProductDescription as ProductDescription,
                    p.ProductName as ProductName,
                    p.ProductIsActive as ProductIsActive
                    FROM [dbo].[Cart] c
                    INNER JOIN
                    dbo.GetProductDetailsByLanguageFN(@Language) p
                    ON p.ProductId = c.Product
                    WHERE [Customer] = @CustomerId";

        return await connection.QueryAsync<CartItemWithProduct>(query, new
        {
            CustomerId = customerId,
            Language = language
        });
    }

    public static async Task<bool> DeleteAllCartsByCustomerAsync(IDbConnection connection, int customerId, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Cart]
                                WHERE [Customer] = @CustomerId";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            CustomerId = customerId
        }, transaction);

        return deletedRowsCount > 0;
    }
}

public class Cart
{
    public long Id { get; set; }
    public int Product { get; set; }
    public decimal Quantity { get; set; }
    public int Customer { get; set; }
}
public class CartItemWithProduct
{
    public long CartItemId { get; set; }
    public int CartItemProduct { get; set; }
    public decimal CartItemQuantity { get; set; }
    public int CartItemCustomer { get; set; }
    public string ProductImageUrl { get; set; }
    public int ProductCategoryId { get; set; }
    public string ProductMeasurementUnit { get; set; }
    public string ProductDescription { get; set; }
    public string ProductName { get; set; }
    public bool ProductIsActive { get; set; }
}

public class CartItemWithProductWithPricing
{
    public int CartItemId { get; set; }              // c.Id as CartItemId
    public int CartItemProduct { get; set; }          // c.Product as CartItemProduct
    public int CartItemQuantity { get; set; }                  // c.Quantity as CartItemQuantity
    public int CartItemCustomer { get; set; }                  // c.Customer as CartItemCustomer

    public int ProductId { get; set; }                 // p.ProductId
    public string ProductImageUrl { get; set; }        // p.ProductImageUrl
    public bool ProductIsActive { get; set; }          // p.ProductIsActive
    public string ProductMeasurementUnit { get; set; } // p.ProductMeasurementUnit
    public string ProductDescription { get; set; }     // p.ProductDescription
    public string ProductName { get; set; }            // p.ProductName
    public int ProductCategoryId { get; set; }         // p.ProductCategoryId
    public bool ProductCategoryIsActive { get; set; }  // p.ProductCategoryIsActive
    public string ProductCategoryName { get; set; }    // p.ProductCategoryName

    public int? DealId { get; set; }                   // p.DealId (nullable if there are cases without a deal)
    public DateTime? DealEndDate { get; set; }         // p.DealEndDate (nullable)
    public DateTime? DealStartDate { get; set; }       // p.DealStartDate (nullable)
    public bool DealIsActive { get; set; }             // p.DealIsActive
    public decimal? DealOffPercentage { get; set; }     // p.DealOffPercentage (nullable)
    
    public bool InCart { get; set; }                   // p.InCart
    public bool InFavorite { get; set; }                // p.InFavorite

    public decimal MaxPrice { get; set; }              // MaxPrice
    public decimal MinPrice { get; set; }              // MinPrice
}
