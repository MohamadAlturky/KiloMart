using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

/// <summary>
/// CREATE TABLE [dbo].[OrderActivity](
///     [Id] [bigint] IDENTITY(1,1) NOT NULL,
///     [Order] [bigint] NOT NULL,
///     [Date] [datetime] NOT NULL,
///     [OrderActivityType] [tinyint] NOT NULL,
///     [OperatedBy] [int] NOT NULL
/// );
/// </summary>
public static partial class OrdersDb
{
    public static async Task<long> InsertOrderActivityAsync(IDbConnection connection,
        long orderId,
        DateTime date,
        byte orderActivityType,
        int operatedBy,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderActivity]
                            ([Order], [Date], [OrderActivityType], [OperatedBy])
                            VALUES (@Order, @Date, @OrderActivityType, @OperatedBy)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Order = orderId,
            Date = date,
            OrderActivityType = orderActivityType,
            OperatedBy = operatedBy
        }, transaction);
    }

    public static async Task<bool> UpdateOrderActivityAsync(IDbConnection connection,
        long id,
        long orderId,
        DateTime date,
        byte orderActivityType,
        int operatedBy,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[OrderActivity]
                                SET 
                                [Order] = @Order,
                                [Date] = @Date,
                                [OrderActivityType] = @OrderActivityType,
                                [OperatedBy] = @OperatedBy
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Order = orderId,
            Date = date,
            OrderActivityType = orderActivityType,
            OperatedBy = operatedBy
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderActivityAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderActivity]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<OrderActivity?> GetOrderActivityByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [Date], 
                            [OrderActivityType], 
                            [OperatedBy]
                            FROM [dbo].[OrderActivity]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<OrderActivity>(query, new
        {
            Id = id
        });
    }
}

public class OrderActivity
{
    public long Id { get; set; }
    public long Order { get; set; }
    public DateTime Date { get; set; }
    public byte OrderActivityType { get; set; }
    public int OperatedBy { get; set; }
}