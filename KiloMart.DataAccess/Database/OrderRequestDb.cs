//using Dapper;
//using System.Data;

//namespace KiloMart.DataAccess.Database;

///// <summary>
///// Dapper-based data access for the OrderRequest table
///// </summary>
//public static partial class Db
//{
//    public static async Task<long> InsertOrderRequestAsync(IDbConnection connection,
//        int customer,
//        DateTime date,
//        byte orderRequestStatus,
//        IDbTransaction? transaction = null)
//    {
//        const string query = @"INSERT INTO [dbo].[OrderRequest]
//                               ([Customer], [Date], [OrderRequestStatus])
//                               VALUES (@Customer, @Date, @OrderRequestStatus)
//                               SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

//        return await connection.ExecuteScalarAsync<long>(query, new
//        {
//            Customer = customer,
//            Date = date,
//            OrderRequestStatus = orderRequestStatus
//        }, transaction);
//    }

//    public static async Task<bool> UpdateOrderRequestAsync(IDbConnection connection,
//        long id,
//        int customer,
//        DateTime date,
//        byte orderRequestStatus,
//        IDbTransaction? transaction = null)
//    {
//        const string query = @"UPDATE [dbo].[OrderRequest]
//                               SET 
//                                   [Customer] = @Customer,
//                                   [Date] = @Date,
//                                   [OrderRequestStatus] = @OrderRequestStatus
//                               WHERE [Id] = @Id";

//        var updatedRowsCount = await connection.ExecuteAsync(query, new
//        {
//            Id = id,
//            Customer = customer,
//            Date = date,
//            OrderRequestStatus = orderRequestStatus
//        }, transaction);

//        return updatedRowsCount > 0;
//    }

//    public static async Task<bool> DeleteOrderRequestAsync(IDbConnection connection,
//        long id,
//        IDbTransaction? transaction = null)
//    {
//        const string query = @"DELETE FROM [dbo].[OrderRequest]
//                               WHERE [Id] = @Id";

//        var deletedRowsCount = await connection.ExecuteAsync(query, new
//        {
//            Id = id
//        }, transaction);

//        return deletedRowsCount > 0;
//    }

//    public static async Task<OrderRequest?> GetOrderRequestByIdAsync(long id, IDbConnection connection)
//    {
//        const string query = @"SELECT 
//                                   [Id],
//                                   [Customer],
//                                   [Date],
//                                   [OrderRequestStatus]
//                               FROM [dbo].[OrderRequest]
//                               WHERE [Id] = @Id";

//        return await connection.QueryFirstOrDefaultAsync<OrderRequest>(query, new
//        {
//            Id = id
//        });
//    }

//    public static async Task<IEnumerable<OrderRequest>> GetOrderRequestsByCustomerAsync(int customer, IDbConnection connection)
//    {
//        const string query = @"SELECT 
//                                   [Id],
//                                   [Customer],
//                                   [Date],
//                                   [OrderRequestStatus]
//                               FROM [dbo].[OrderRequest]
//                               WHERE [Customer] = @Customer";

//        return await connection.QueryAsync<OrderRequest>(query, new
//        {
//            Customer = customer
//        });
//    }
//}

///// <summary>
///// Represents the OrderRequest table
///// </summary>
//public class OrderRequest
//{
//    public long Id { get; set; }
//    public int Customer { get; set; }
//    public DateTime Date { get; set; }
//    public byte OrderRequestStatus { get; set; }
//}
