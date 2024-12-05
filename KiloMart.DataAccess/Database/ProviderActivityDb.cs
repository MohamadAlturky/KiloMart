using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for ProviderActivity
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertProviderActivityAsync(IDbConnection connection,
        DateTime date,
        float value,
        int Provider,
        long order,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProviderActivity]
                            ([Date], [Value], [Provider],[Order])
                            VALUES (@Date, @Value, @Provider, @Order)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Date = date,
            Value = value,
            Provider = Provider,
            Order = order
        }, transaction);
    }

    public static async Task<bool> UpdateProviderActivityAsync(IDbConnection connection,
        long id,
        DateTime date,
        float value,
        int Provider,
        long order,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProviderActivity]
                                SET 
                                [Date] = @Date,
                                [Value] = @Value,
                                [Provider] = @Provider
                                [Order] = @Order
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Date = date,
            Value = value,
            Provider = Provider,
            Order = order
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProviderActivityAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProviderActivity]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProviderActivity?> GetProviderActivityByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Provider],
                            [Order]
                            FROM [dbo].[ProviderActivity]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderActivity>(query, new
        {
            Id = id
        });
    }

    public static async Task<IEnumerable<ProviderActivity>> GetProviderActivitiesByProviderIdAsync(int ProviderId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Provider],
                            [Order]
                            FROM [dbo].[ProviderActivity]
                            WHERE [Provider] = @ProviderId
                            ORDER BY [Id] DESC";

        return await connection.QueryAsync<ProviderActivity>(query, new
        {
            ProviderId = ProviderId
        });
    }
    public static async Task<IEnumerable<ProviderActivity>> GetProviderActivitiesByDateBetweenAndProviderAsync(
    DateTime startDate,
    DateTime endDate,
    int ProviderId,
    IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Provider],
                            [Order]
                            FROM [dbo].[ProviderActivity]
                            WHERE [Date] BETWEEN @StartDate AND @EndDate
                            AND [Provider] = @ProviderId
                            ORDER BY [Id] DESC";

        return await connection.QueryAsync<ProviderActivity>(query, new
        {
            StartDate = startDate,
            EndDate = endDate,
            ProviderId = ProviderId
        });
    }

    public static async Task<IEnumerable<ProviderActivity>> GetProviderActivitiesByDateBiggerAndProviderAsync(
        DateTime date,
        int ProviderId,
        IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Provider],
                            [Order]
                            FROM [dbo].[ProviderActivity]
                            WHERE [Date] > @Date
                            AND [Provider] = @ProviderId
                            ORDER BY [Id] DESC";

        return await connection.QueryAsync<ProviderActivity>(query, new
        {
            Date = date,
            ProviderId = ProviderId
        });
    }
}

public class ProviderActivity
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public float Value { get; set; }
    public int Provider { get; set; }
    public long Order { get; set; }
}