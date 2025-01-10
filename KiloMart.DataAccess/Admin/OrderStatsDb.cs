using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{

    public static async Task<long> GetOrderCountByStatusAsync(
        IDbConnection connection,
        byte orderStatus)
    {
        const string query = @"SELECT COUNT(o.[Id]) AS TotalCount 
                               FROM dbo.[Order] o 
                               WHERE o.OrderStatus = @OrderStatus";

        return await connection.ExecuteScalarAsync<long>(
            query,
            new { OrderStatus = orderStatus });
    }
    public static async Task<long> GetSystemFeeSumByStatusAsync(
        IDbConnection connection,
        byte orderStatus)
    {
        const string query = @"
                    SELECT SUM(o.[SystemFee]) AS TotalCount from dbo.[Order] o
                        Where o.OrderStatus=  @OrderStatus";

        return await connection.ExecuteScalarAsync<long>(
            query,
            new { OrderStatus = orderStatus });
    }
}