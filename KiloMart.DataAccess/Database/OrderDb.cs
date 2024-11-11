using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[Order](
// 	[Id] [bigint] IDENTITY(1,1) NOT NULL,
// 	[OrderStatus] [tinyint] NOT NULL,
// 	[TotalPrice] [money] NOT NULL,
// 	[TransactionId] [varchar](50) NOT NULL,
// 	[CustomerLocation] [int] NOT NULL,
// 	[ProviderLocation] [int] NOT NULL,
// 	[Customer] [int] NOT NULL,
// 	[Provider] [int] NOT NULL)
/// </summary>
///
public static partial class Db
{
    public static async Task<long> InsertOrderAsync(IDbConnection connection,
        byte orderStatus,
        decimal totalPrice,
        string transactionId,
        int customerLocation,
        int providerLocation,
        int customer,
        int provider,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Order]
                            ([OrderStatus], [TotalPrice], [TransactionId], [CustomerLocation], [ProviderLocation], [Customer], [Provider])
                            VALUES (@OrderStatus, @TotalPrice, @TransactionId, @CustomerLocation, @ProviderLocation, @Customer, @Provider)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            OrderStatus = orderStatus,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            CustomerLocation = customerLocation,
            ProviderLocation = providerLocation,
            Customer = customer,
            Provider = provider
        }, transaction);
    }

    public static async Task<bool> UpdateOrderAsync(IDbConnection connection,
        long id,
        byte orderStatus,
        decimal totalPrice,
        string transactionId,
        int customerLocation,
        int providerLocation,
        int customer,
        int provider,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Order]
                                SET 
                                [OrderStatus] = @OrderStatus,
                                [TotalPrice] = @TotalPrice,
                                [TransactionId] = @TransactionId,
                                [CustomerLocation] = @CustomerLocation,
                                [ProviderLocation] = @ProviderLocation,
                                [Customer] = @Customer,
                                [Provider] = @Provider
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            OrderStatus = orderStatus,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            CustomerLocation = customerLocation,
            ProviderLocation = providerLocation,
            Customer = customer,
            Provider = provider
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Order]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Order?> GetOrderByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [OrderStatus], 
                            [TotalPrice], 
                            [TransactionId], 
                            [CustomerLocation], 
                            [ProviderLocation], 
                            [Customer], 
                            [Provider]
                            FROM [dbo].[Order]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Order>(query, new
        {
            Id = id
        });
    }
}

public class Order
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;
    public int CustomerLocation { get; set; }
    public int ProviderLocation { get; set; }
    public int Customer { get; set; }
    public int Provider { get; set; }
}
