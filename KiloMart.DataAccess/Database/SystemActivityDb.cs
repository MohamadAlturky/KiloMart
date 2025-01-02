// using Dapper;
// using System.Data;

// namespace KiloMart.DataAccess.Database;

// /// <summary>
// /// Table Specification for SystemActivity
// /// </summary>
// public static partial class Db
// {
//     public static async Task<long> InsertSystemActivityAsync(IDbConnection connection,
//         DateTime date,
//         decimal value,
//         long order,
//         IDbTransaction? transaction = null)
//     {
//         const string query = @"INSERT INTO [dbo].[SystemActivity]
//                             ([Date], [Value], [Order])
//                             VALUES (@Date, @Value, @Order)
//                             SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

//         return await connection.ExecuteScalarAsync<long>(query, new
//         {
//             Date = date,
//             Value = value,
//             Order = order
//         }, transaction);
//     }

//     public static async Task<bool> UpdateSystemActivityAsync(IDbConnection connection,
//         long id,
//         DateTime date,
//         float value,
//         long order,
//         IDbTransaction? transaction = null)
//     {
//         const string query = @"UPDATE [dbo].[SystemActivity]
//                                 SET 
//                                 [Date] = @Date,
//                                 [Value] = @Value,
//                                 [Order] = @Order
//                                 WHERE [Id] = @Id";

//         var updatedRowsCount = await connection.ExecuteAsync(query, new
//         {
//             Id = id,
//             Date = date,
//             Value = value,
//             Order = order
//         }, transaction);

//         return updatedRowsCount > 0;
//     }

//     public static async Task<bool> DeleteSystemActivityAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
//     {
//         const string query = @"DELETE FROM [dbo].[SystemActivity]
//                                 WHERE [Id] = @Id";

//         var deletedRowsCount = await connection.ExecuteAsync(query, new
//         {
//             Id = id
//         }, transaction);

//         return deletedRowsCount > 0;
//     }

//     public static async Task<SystemActivity?> GetSystemActivityByIdAsync(long id, IDbConnection connection)
//     {
//         const string query = @"SELECT 
//                             [Id], 
//                             [Date], 
//                             [Value], 
//                             [Order]
//                             FROM [dbo].[SystemActivity]
//                             WHERE [Id] = @Id";

//         return await connection.QueryFirstOrDefaultAsync<SystemActivity>(query, new
//         {
//             Id = id
//         });
//     }

//     public static async Task<IEnumerable<SystemActivity>> GetAllSystemActivitiesAsync(IDbConnection connection)
//     {
//         const string query = @"SELECT 
//                             [Id], 
//                             [Date], 
//                             [Value], 
//                             [Order]
//                             FROM [dbo].[SystemActivity]
//                             ORDER BY [Id] DESC";

//         return await connection.QueryAsync<SystemActivity>(query);
//     }
//     public static async Task<float> GetSumOfActivityValuesBetweenDatesAsync(
//         DateTime startDate,
//         DateTime endDate,
//         IDbConnection connection)
//     {
//         const string query = @"SELECT SUM([Value]) 
//                             FROM [dbo].[SystemActivity]
//                             WHERE [Date] BETWEEN @StartDate AND @EndDate";

//         var sum = await connection.ExecuteScalarAsync<float?>(query, new
//         {
//             StartDate = startDate,
//             EndDate = endDate
//         });

//         return sum ?? 0; // Return 0 if the result is null (no records found)
//     }
// }

// public class SystemActivity
// {
//     public long Id { get; set; }
//     public DateTime Date { get; set; }
//     public float Value { get; set; }
//     public long Order { get; set; }
// }