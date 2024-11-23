using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

/// <summary>
/// CREATE TABLE [dbo].[OrderProduct](
///     [Id] [int] IDENTITY(1,1) NOT NULL,
///     [Order] [bigint] NOT NULL,
///     [Product] [int] NOT NULL,
///     [Quantity] [float] NOT NULL
/// );
/// </summary>
public static partial class OrdersDb
{
    public static async Task<int> InsertOrderProductAsync(IDbConnection connection,
        long orderId,
        int productId,
        double quantity,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderProduct]
                            ([Order], [Product], [Quantity])
                            VALUES (@Order, @Product, @Quantity)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Order = orderId,
            Product = productId,
            Quantity = quantity
        }, transaction);
    }

    public static async Task<bool> UpdateOrderProductAsync(IDbConnection connection,
        int id,
        long orderId,
        int productId,
        double quantity,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[OrderProduct]
                                SET 
                                [Order] = @Order,
                                [Product] = @Product,
                                [Quantity] = @Quantity
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Order = orderId,
            Product = productId,
            Quantity = quantity
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderProductAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderProduct]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<OrderProduct?> GetOrderProductByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [Product], 
                            [Quantity]
                            FROM [dbo].[OrderProduct]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<OrderProduct>(query, new
        {
            Id = id
        });
    }
    public static async Task<List<OrderProduct>> GetOrderProductByOrderIdAsync(long orderId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [Product], 
                            [Quantity]
                            FROM [dbo].[OrderProduct]
                            WHERE [Order] = @OrderId";

        
        var result = await connection.QueryAsync<OrderProduct>(query, new
        {
            OrderId = orderId
        });
        return result.ToList();
    }
}

public class OrderProduct
{
    public int Id { get; set; }
    public long Order { get; set; }
    public int Product { get; set; }
    public double Quantity { get; set; }
}