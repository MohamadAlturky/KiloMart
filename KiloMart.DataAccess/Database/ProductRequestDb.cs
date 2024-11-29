using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Database;



/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[ProductRequest](
// 	[Id] [int] IDENTITY(1,1) NOT NULL,
// 	[Provider] [int] NOT NULL,
// 	[Date] [datetime] NOT NULL,
// 	[ImageUrl] [varchar](300) NOT NULL,
// 	[ProductCategory] [int] NOT NULL,
// 	[Price] [money] NOT NULL,
// 	[OffPercentage] [decimal](10, 5) NOT NULL,
// 	[Quantity] [decimal] NOT NULL,
// 	[Status] [tinyint] NOT NULL)
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertProductRequestAsync(IDbConnection connection,
        int provider,
        DateTime date,
        string imageUrl,
        int productCategory,
        decimal price,
        decimal offPercentage,
        decimal quantity,
        byte status,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProductRequest]
                    ([Provider], [Date], [ImageUrl], [ProductCategory], [Price], [OffPercentage], [Quantity], [Status])
                    VALUES (@Provider, @Date, @ImageUrl, @ProductCategory, @Price, @OffPercentage, @Quantity, @Status)
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Provider = provider,
            Date = date,
            ImageUrl = imageUrl,
            ProductCategory = productCategory,
            Price = price,
            OffPercentage = offPercentage,
            Quantity = quantity,
            Status = status
        }, transaction);
    }

    public static async Task<bool> UpdateProductRequestAsync(IDbConnection connection,
        int id,
        int provider,
        DateTime date,
        string imageUrl,
        int productCategory,
        decimal price,
        decimal offPercentage,
        decimal quantity,
        byte status,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProductRequest]
                        SET 
                        [Provider] = @Provider,
                        [Date] = @Date,
                        [ImageUrl] = @ImageUrl,
                        [ProductCategory] = @ProductCategory,
                        [Price] = @Price,
                        [OffPercentage] = @OffPercentage,
                        [Quantity] = @Quantity,
                        [Status] = @Status
                        WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Provider = provider,
            Date = date,
            ImageUrl = imageUrl,
            ProductCategory = productCategory,
            Price = price,
            OffPercentage = offPercentage,
            Quantity = quantity,
            Status = status
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProductRequestAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProductRequest]
                            WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProductRequest?> GetProductRequestByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Provider], 
                            [Date], 
                            [ImageUrl], 
                            [ProductCategory], 
                            [Price], 
                            [OffPercentage], 
                            [Quantity], 
                            [Status]
                            FROM [dbo].[ProductRequest]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProductRequest>(query, new
        {
            Id = id
        });
    }
}

public class ProductRequest
{
    public int Id { get; set; }
    public int Provider { get; set; }
    public DateTime Date { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int ProductCategory { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public decimal Quantity { get; set; }
    public byte Status { get; set; }
}
