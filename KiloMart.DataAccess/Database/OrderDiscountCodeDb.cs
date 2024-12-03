using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[OrderDiscountCode](
// 	[Order] [bigint] NOT NULL,
// 	[DiscountCode] [int] NOT NULL)
/// </summary>

public static partial class Db
{
   public static async Task<bool> CreateOrderDiscountCodeAsync(IDbConnection connection,
       long orderId,
       int discountCodeId,
       IDbTransaction? transaction = null)
   {
       const string query = @"INSERT INTO [dbo].[OrderDiscountCode]
                           ([Order], [DiscountCode])
                           VALUES (@OrderId, @DiscountCodeId)";

       var insertedRowsCount = await connection.ExecuteAsync(query, new
       {
           OrderId = orderId,
           DiscountCodeId = discountCodeId
       }, transaction);

       return insertedRowsCount > 0;
   }

   public static async Task<bool> HasOrderDiscountCodeAsync(long orderId, IDbConnection connection)
   {
       const string query = @"SELECT 1 FROM [dbo].[OrderDiscountCode]
                               WHERE [Order] = @OrderId";

       var result = await connection.ExecuteScalarAsync<int?>(query, new
       {
           OrderId = orderId
       });

       return result.HasValue;
   }

   public static async Task<bool> DeleteOrderDiscountCodeAsync(long orderId, IDbConnection connection, IDbTransaction? transaction = null)
   {
       const string query = @"DELETE FROM [dbo].[OrderDiscountCode]
                               WHERE [Order] = @OrderId";

       var deletedRowsCount = await connection.ExecuteAsync(query, new
       {
           OrderId = orderId
       }, transaction);

       return deletedRowsCount > 0;
   }
}

public class OrderDiscountCode
{
   public long Order { get; set; }
   public int DiscountCode { get; set; }
}
