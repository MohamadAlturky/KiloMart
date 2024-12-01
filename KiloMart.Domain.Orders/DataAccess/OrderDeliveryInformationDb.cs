using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

public static partial class OrdersDb
{
    public static async Task<long> InsertOrderDeliveryInfoAsync(IDbConnection connection,
        long order,
        int delivery,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderDeliveryInformation]
                            ([Order], [Delivery])
                            VALUES (@Order, @Delivery)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Order = order,
            Delivery = delivery
        }, transaction);
    }

    public static async Task<bool> UpdateOrderDeliveryInfoAsync(IDbConnection connection,
        long id,
        long order,
        int delivery,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[OrderDeliveryInformation]
                                SET 
                                [Order] = @Order,
                                [Delivery] = @Delivery
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Order = order,
            Delivery = delivery
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderDeliveryInfoAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderDeliveryInformation]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<OrderDeliveryInformation?> GetOrderDeliveryInfoByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [Delivery]
                            FROM [dbo].[OrderDeliveryInformation]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<OrderDeliveryInformation>(query, new
        {
            Id = id
        });
    }
}
public class OrderDeliveryInformation
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Delivery { get; set; }
}