using Dapper;
using System.Data;
using System.Text;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for ProviderActivity
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertProviderActivityAsync(IDbConnection connection,
        DateTime date,
        decimal value,
        int Provider,
        byte type,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProviderActivity]
                            ([Date], [Value], [Provider],[Type])
                            VALUES (@Date, @Value, @Provider, @Type)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Date = date,
            Value = value,
            Provider = Provider,
            Type = type
        }, transaction);
    }

    public static async Task<bool> UpdateProviderActivityAsync(IDbConnection connection,
        long id,
        DateTime date,
        float value,
        int Provider,
        byte type,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProviderActivity]
                                SET 
                                [Date] = @Date,
                                [Value] = @Value,
                                [Provider] = @Provider
                                [Type] = @Type
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Date = date,
            Value = value,
            Provider = Provider,
            Type = type
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
                            [Type]
                            FROM [dbo].[ProviderActivity]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderActivity>(query, new
        {
            Id = id
        });
    }

    public static async Task<IEnumerable<ProviderActivity>> GetProviderActivityFilteredAsync(
        IDbConnection connection,
        int provider,
        byte? type = null,
        DateTime? startdate = null,
        DateTime? enddate = null)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("SELECT");
        sqlBuilder.AppendLine("    [Id],");
        sqlBuilder.AppendLine("    [Date],");
        sqlBuilder.AppendLine("    [Value],");
        sqlBuilder.AppendLine("    [Provider],");
        sqlBuilder.AppendLine("    [Type]");
        sqlBuilder.AppendLine("FROM [dbo].[ProviderActivity]");
        sqlBuilder.AppendLine("WHERE Provider = @Provider"); // This is a trick to make adding AND conditions easier

        var parameters = new DynamicParameters();
        parameters.Add("@Provider", provider);

        if (type.HasValue)
        {
            sqlBuilder.AppendLine("AND [Type] = @Type");
            parameters.Add("@Type", type.Value);
        }

        if (startdate.HasValue)
        {
            sqlBuilder.AppendLine("AND [Date] >= @StartDate");
            parameters.Add("@StartDate", startdate.Value);
        }

        if (enddate.HasValue)
        {
            sqlBuilder.AppendLine("AND [Date] <= @EndDate");
            parameters.Add("@EndDate", enddate.Value);
        }

        return await connection.QueryAsync<ProviderActivity>(
            sqlBuilder.ToString(),
            parameters);
    }

    public class BalanceValueResponse
    {
        public decimal ValueSum { get; set; }
    }
    public static async Task<ProviderActivityWallet> GetProviderActivityTotalValueByProviderAsync(
        int provider,
        byte receives,
        byte deductions,
        IDbConnection connection)
    {
        const string query = @"SELECT  
                            SUM([Value]) AS ValueSum
                            FROM [dbo].[ProviderActivity]
                            WHERE [Provider] = @Provider AND Type = @Type";


        var deductionsValue = await connection.QueryFirstOrDefaultAsync<BalanceValueResponse>(query, new
        {
            Provider = provider,
            Type = deductions
        });
        var receivesValue = await connection.QueryFirstOrDefaultAsync<BalanceValueResponse>(query, new
        {
            Provider = provider,
            Type = receives
        });
        
        return new ProviderActivityWallet
        {
            Deductions = deductionsValue?.ValueSum,
            Receives = receivesValue?.ValueSum
        };
    }


    public static async Task<IEnumerable<ProviderActivity>> GetProviderActivitiesByProviderIdAsync(int ProviderId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Provider],
                            [Type]
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
                            [Type]
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
                            [Type]
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
    public byte Type { get; set; }
}
public class ProviderActivityWallet
{
    public decimal? Deductions { get; set; }
    public decimal? Receives { get; set; }
}