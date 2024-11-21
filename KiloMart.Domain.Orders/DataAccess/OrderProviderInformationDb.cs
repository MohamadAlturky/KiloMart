using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

/// <summary>
/// CREATE TABLE [dbo].[OrderProviderInformation]
/// (
///     [Id] [bigint] IDENTITY(1,1) NOT NULL,
///     [Order] [bigint] NOT NULL,
///     [Provider] [int] NOT NULL,
///     [Location] [int] NOT NULL
/// );
/// </summary>
public static partial class OrdersDb
{
    public static async Task<long> InsertOrderProviderInfoAsync(IDbConnection connection,
        long orderId,
        int providerId,
        int locationId,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderProviderInformation]
                            ([Order], [Provider], [Location])
                            VALUES (@Order, @Provider, @Location)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Order = orderId,
            Provider = providerId,
            Location = locationId
        }, transaction);
    }

    public static async Task<bool> UpdateOrderProviderInfoAsync(IDbConnection connection,
        long id,
        long orderId,
        int providerId,
        int locationId,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[OrderProviderInformation]
                                SET 
                                [Order] = @Order,
                                [Provider] = @Provider,
                                [Location] = @Location
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Order = orderId,
            Provider = providerId,
            Location = locationId
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderProviderInfoAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderProviderInformation]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<OrderProviderInformation?> GetOrderProviderInfoByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [Provider], 
                            [Location]
                            FROM [dbo].[OrderProviderInformation]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<OrderProviderInformation>(query, new
        {
            Id = id
        });
    }
}

public class OrderProviderInformation
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Provider { get; set; }
    public int Location { get; set; }
}