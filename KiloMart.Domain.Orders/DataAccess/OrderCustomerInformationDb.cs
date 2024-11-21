using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

/// <summary>
/// CREATE TABLE[dbo].[OrderCustomerInformation]
/// (
///     [Id][bigint] IDENTITY(1,1) NOT NULL,
///     [Order][bigint] NOT NULL,
///     [Customer][int] NOT NULL,
///     [Location][int] NOT NULL
/// );
/// </summary>
public static partial class OrdersDb
{
    public static async Task<long> InsertOrderCustomerInfoAsync(IDbConnection connection,
        long orderId,
        int customerId,
        int locationId,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderCustomerInformation]
                            ([Order], [Customer], [Location])
                            VALUES (@Order, @Customer, @Location)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Order = orderId,
            Customer = customerId,
            Location = locationId
        }, transaction);
    }

    public static async Task<bool> UpdateOrderCustomerInfoAsync(IDbConnection connection,
        long id,
        long orderId,
        int customerId,
        int locationId,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[OrderCustomerInformation]
                                SET 
                                [Order] = @Order,
                                [Customer] = @Customer,
                                [Location] = @Location
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Order = orderId,
            Customer = customerId,
            Location = locationId
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderCustomerInfoAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderCustomerInformation]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<OrderCustomerInformation?> GetOrderCustomerInfoByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [Customer], 
                            [Location]
                            FROM [dbo].[OrderCustomerInformation]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<OrderCustomerInformation>(query, new
        {
            Id = id
        });
    }
}

public class OrderCustomerInformation
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Customer { get; set; }
    public int Location { get; set; }
}