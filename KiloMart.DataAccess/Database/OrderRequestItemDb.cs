//using Dapper;
//using System.Data;

//namespace KiloMart.DataAccess.Database;

///// <summary>
///// Dapper-based data access for the OrderRequestItem table
///// </summary>
//public static partial class Db
//{
//    public static async Task<long> InsertOrderRequestItemAsync(IDbConnection connection,
//        int product,
//        double quantity,
//        long orderRequest,
//        IDbTransaction? transaction = null)
//    {
//        const string query = @"INSERT INTO [dbo].[OrderRequestItem]
//                               ([Product], [Quantity], [OrderRequest])
//                               VALUES (@Product, @Quantity, @OrderRequest)
//                               SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

//        return await connection.ExecuteScalarAsync<long>(query, new
//        {
//            Product = product,
//            Quantity = quantity,
//            OrderRequest = orderRequest
//        }, transaction);
//    }

//    public static async Task<bool> UpdateOrderRequestItemAsync(IDbConnection connection,
//        long id,
//        int product,
//        double quantity,
//        long orderRequest,
//        IDbTransaction? transaction = null)
//    {
//        const string query = @"UPDATE [dbo].[OrderRequestItem]
//                               SET 
//                                   [Product] = @Product,
//                                   [Quantity] = @Quantity,
//                                   [OrderRequest] = @OrderRequest
//                               WHERE [Id] = @Id";

//        var updatedRowsCount = await connection.ExecuteAsync(query, new
//        {
//            Id = id,
//            Product = product,
//            Quantity = quantity,
//            OrderRequest = orderRequest
//        }, transaction);

//        return updatedRowsCount > 0;
//    }

//    public static async Task<bool> DeleteOrderRequestItemAsync(IDbConnection connection,
//        long id,
//        IDbTransaction? transaction = null)
//    {
//        const string query = @"DELETE FROM [dbo].[OrderRequestItem]
//                               WHERE [Id] = @Id";

//        var deletedRowsCount = await connection.ExecuteAsync(query, new
//        {
//            Id = id
//        }, transaction);

//        return deletedRowsCount > 0;
//    }

//    public static async Task<OrderRequestItem?> GetOrderRequestItemByIdAsync(long id, IDbConnection connection)
//    {
//        const string query = @"SELECT 
//                                   [Id],
//                                   [Product],
//                                   [Quantity],
//                                   [OrderRequest]
//                               FROM [dbo].[OrderRequestItem]
//                               WHERE [Id] = @Id";

//        return await connection.QueryFirstOrDefaultAsync<OrderRequestItem>(query, new
//        {
//            Id = id
//        });
//    }

//    public static async Task<IEnumerable<OrderRequestItem>> GetOrderRequestItemsByOrderRequestAsync(long orderRequest, IDbConnection connection)
//    {
//        const string query = @"SELECT 
//                                   [Id],
//                                   [Product],
//                                   [Quantity],
//                                   [OrderRequest]
//                               FROM [dbo].[OrderRequestItem]
//                               WHERE [OrderRequest] = @OrderRequest";

//        return await connection.QueryAsync<OrderRequestItem>(query, new
//        {
//            OrderRequest = orderRequest
//        });
//    }
//}

///// <summary>
///// Represents the OrderRequestItem table
///// </summary>
//public class OrderRequestItem
//{
//    public long Id { get; set; }
//    public int Product { get; set; }
//    public double Quantity { get; set; }
//    public long OrderRequest { get; set; }
//}
