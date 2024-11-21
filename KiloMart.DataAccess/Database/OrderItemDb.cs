//using Dapper;
//using System.Data;

//namespace KiloMart.DataAccess.Database
//{
//    /// <summary>
//    /// Table Specification
//    //  CREATE TABLE [dbo].[OrderItem](
//    //  [Id] [bigint] IDENTITY(1,1) NOT NULL,
//    //  [Order] [bigint] NOT NULL,
//    //  [ProductOffer] [int] NOT NULL,
//    //  [UnitPrice] [money] NOT NULL,
//    //  [Quantity] [float] NOT NULL)
//    /// </summary>
//    public static partial class Db
//    {
//        public static async Task<long> InsertOrderItemAsync(IDbConnection connection,
//            long orderId,
//            int productOfferId,
//            decimal unitPrice,
//            float quantity,
//            IDbTransaction? transaction = null)
//        {
//            const string query = @"INSERT INTO [dbo].[OrderItem]
//                                ([Order], [ProductOffer], [UnitPrice], [Quantity])
//                                VALUES (@OrderId, @ProductOfferId, @UnitPrice, @Quantity)
//                                SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

//            return await connection.ExecuteScalarAsync<long>(query, new
//            {
//                OrderId = orderId,
//                ProductOfferId = productOfferId,
//                UnitPrice = unitPrice,
//                Quantity = quantity
//            }, transaction);
//        }

//        public static async Task<bool> UpdateOrderItemAsync(IDbConnection connection,
//            long id,
//            long orderId,
//            int productOfferId,
//            decimal unitPrice,
//            float quantity,
//            IDbTransaction? transaction = null)
//        {
//            const string query = @"UPDATE [dbo].[OrderItem]
//                                    SET 
//                                    [Order] = @OrderId,
//                                    [ProductOffer] = @ProductOfferId,
//                                    [UnitPrice] = @UnitPrice,
//                                    [Quantity] = @Quantity
//                                    WHERE [Id] = @Id";

//            var updatedRowsCount = await connection.ExecuteAsync(query, new
//            {
//                Id = id,
//                OrderId = orderId,
//                ProductOfferId = productOfferId,
//                UnitPrice = unitPrice,
//                Quantity = quantity
//            }, transaction);

//            return updatedRowsCount > 0;
//        }

//        public static async Task<bool> DeleteOrderItemAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
//        {
//            const string query = @"DELETE FROM [dbo].[OrderItem]
//                                    WHERE [Id] = @Id";

//            var deletedRowsCount = await connection.ExecuteAsync(query, new
//            {
//                Id = id
//            }, transaction);

//            return deletedRowsCount > 0;
//        }

//        public static async Task<OrderItem?> GetOrderItemByIdAsync(long id, IDbConnection connection)
//        {
//            const string query = @"SELECT 
//                                    [Id], 
//                                    [Order], 
//                                    [ProductOffer], 
//                                    [UnitPrice], 
//                                    [Quantity]
//                                    FROM [dbo].[OrderItem]
//                                    WHERE [Id] = @Id";

//            return await connection.QueryFirstOrDefaultAsync<OrderItem>(query, new
//            {
//                Id = id
//            });
//        }
//    }

//    public class OrderItem
//    {
//        public long Id { get; set; }
//        public long Order { get; set; }
//        public int ProductOffer { get; set; }
//        public decimal UnitPrice { get; set; }
//        public float Quantity { get; set; }
//    }
//}
