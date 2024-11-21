//using Dapper;
//using System.Data;

//namespace KiloMart.DataAccess.Database
//{
//    /// <summary>
//    /// Table Specification
//    /// 
//    /// CREATE TABLE [dbo].[OrderItemDiscountCode](
//    ///     [OrderItem] [bigint] NOT NULL,
//    ///     [DiscountCode] [int] NOT NULL
//    /// )
//    /// </summary>
//    /// 
//    public static partial class OrderItemDiscountCodeDb
//    {
//        public static async Task<int> InsertOrderItemDiscountCodeAsync(IDbConnection connection,
//            long orderItem,
//            int discountCode,
//            IDbTransaction? transaction = null)
//        {
//            const string query = @"INSERT INTO [dbo].[OrderItemDiscountCode]
//                                ([OrderItem], [DiscountCode])
//                                VALUES (@OrderItem, @DiscountCode)";

//            return await connection.ExecuteAsync(query, new
//            {
//                OrderItem = orderItem,
//                DiscountCode = discountCode
//            }, transaction);
//        }

//        public static async Task<bool> DeleteOrderItemDiscountCodeAsync(IDbConnection connection,
//            long orderItem,
//            int discountCode,
//            IDbTransaction? transaction = null)
//        {
//            const string query = @"DELETE FROM [dbo].[OrderItemDiscountCode]
//                                    WHERE [OrderItem] = @OrderItem AND [DiscountCode] = @DiscountCode";

//            var deletedRowsCount = await connection.ExecuteAsync(query, new
//            {
//                OrderItem = orderItem,
//                DiscountCode = discountCode
//            }, transaction);

//            return deletedRowsCount > 0;
//        }

//        public static async Task<List<OrderItemDiscountCode>?> GetOrderItemDiscountCodesAsync(IDbConnection connection)
//        {
//            const string query = @"SELECT 
//                                [OrderItem], 
//                                [DiscountCode]
//                                FROM [dbo].[OrderItemDiscountCode]";
//            var result =  await connection.QueryAsync<OrderItemDiscountCode>(query);
//            return result.ToList();
//        }

//        public static async Task<OrderItemDiscountCode?> GetOrderItemDiscountCodeAsync(IDbConnection connection,
//            long orderItem,
//            int discountCode)
//        {
//            const string query = @"SELECT 
//                                [OrderItem], 
//                                [DiscountCode]
//                                FROM [dbo].[OrderItemDiscountCode]
//                                WHERE [OrderItem] = @OrderItem AND [DiscountCode] = @DiscountCode";

//            return await connection.QueryFirstOrDefaultAsync<OrderItemDiscountCode>(query, new
//            {
//                OrderItem = orderItem,
//                DiscountCode = discountCode
//            });
//        }
//    }

//    public class OrderItemDiscountCode
//    {
//        public long OrderItem { get; set; }
//        public int DiscountCode { get; set; }
//    }
//}
