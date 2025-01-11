using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{
    public static async Task<long> GetCanceledOrderCountAsync(
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        const string query = @"
        SELECT COUNT(o.[Id]) AS TotalCount
        FROM dbo.[Order] o
        INNER JOIN dbo.[OrderActivity] oa ON oa.[Order] = o.[Id]
        WHERE oa.[OrderActivityType] = 2"; // Cancel by customer

        // Execute the query and retrieve the count
        var totalCount = await connection.ExecuteScalarAsync<long>(query, transaction: transaction);

        return totalCount;
    }

    public static async Task<long> GetCompletedOrderCountAsync(
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        const string query = @"
        SELECT COUNT(o.[Id])
        FROM dbo.[Order] o
        WHERE o.[OrderStatus] = 5"; // Completed orders

        // Execute the query and retrieve the count
        var completedOrderCount = await connection.ExecuteScalarAsync<long>(query, transaction: transaction);

        return completedOrderCount;
    }

    public static async Task<long> GetTotalOrderCountAsync(
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        const string query = @"
        SELECT COUNT(o.[Id])
        FROM dbo.[Order] o"; // Total orders

        // Execute the query and retrieve the count
        var totalOrderCount = await connection.ExecuteScalarAsync<long>(query, transaction: transaction);

        return totalOrderCount;
    }
    public static async Task<long> ThereIsNoProviderAcceptedItAsync(
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        const string query = @"
        SELECT COUNT(o.[Id])
        FROM dbo.[Order] o
        LEFT JOIN dbo.[OrderProviderInformation] opi ON opi.[Order] = o.[Id]
        WHERE opi.[Provider] IS NULL"; // No provider accepted

        // Execute the query and retrieve the count
        var noProviderCount = await connection.ExecuteScalarAsync<long>(query, transaction: transaction);

        return noProviderCount;
    }

    public static async Task<long> ThereIsNoDeliveryAcceptedItAsync(
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        const string query = @"
        SELECT COUNT(o.[Id])
        FROM dbo.[Order] o
        LEFT JOIN dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.[Id]
        WHERE odi.[Delivery] IS NULL"; // No delivery accepted

        // Execute the query and retrieve the count
        var noDeliveryCount = await connection.ExecuteScalarAsync<long>(query, transaction: transaction);

        return noDeliveryCount;
    }

}