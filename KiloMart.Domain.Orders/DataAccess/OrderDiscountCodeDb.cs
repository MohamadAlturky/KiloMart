using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

public static partial class OrdersDb
{
    public static async Task<bool> InsertOrderDiscountCodeAsync(IDbConnection connection,
        long orderId,
        int discountCode,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderDiscountCode]
                                   ([Order], [DiscountCode])
                                   VALUES (@Order, @DiscountCode)";

        var affectedRows = await connection.ExecuteAsync(query, new
        {
            Order = orderId,
            DiscountCode = discountCode
        }, transaction);

        return affectedRows > 0;
    }

    public static async Task<bool> DeleteOrderDiscountCodeAsync(IDbConnection connection,
        long orderId,
        IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderDiscountCode]
                                   WHERE [Order] = @Order";

        var affectedRows = await connection.ExecuteAsync(query, new
        {
            Order = orderId
        }, transaction);

        return affectedRows > 0;
    }

    public static async Task<IEnumerable<OrderDiscountCode>> GetDiscountCodesByOrderIdAsync(long orderId, IDbConnection connection)
    {
        const string query = @"SELECT 
                                   [Order], 
                                   [DiscountCode]
                                   FROM [dbo].[OrderDiscountCode]
                                   WHERE [Order] = @Order";

        return await connection.QueryAsync<OrderDiscountCode>(query, new { Order = orderId });
    }
}

public class OrderDiscountCode
{
    public long Order { get; set; }
    public int DiscountCode { get; set; }
}