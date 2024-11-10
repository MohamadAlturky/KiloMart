using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.Models;

namespace KiloMart.Domain.Orders.DataAccess;
public static class OrderActivityRepository
{
    public static async Task InsertOrderActivity(OrderActivity model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        INSERT INTO [dbo].[OrderActivity] ([Order], [Date], [OrderActivityType], [OperatedBy])
            VALUES (@Order, @Date, @OrderActivityType, @OperatedBy)";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task<List<OrderActivity>> GetOrderActivitiesByOrder(long orderId, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[OrderActivity] WHERE [Order] = @Order";
        
        var result = await connection.QueryAsync<OrderActivity>(query, new { Order = orderId });
        return result.ToList();
    }
}

