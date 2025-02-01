using Dapper;
using KiloMart.Domain.DateServices;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[DriverFreeFee](
///     [Id] [int] IDENTITY(1,1) NOT NULL,
///     [StartDate] [datetime] NOT NULL,
///     [EndDate] [datetime] NOT NULL,
///     [IsActive] [bit] NOT NULL)
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertDriverFreeFeeAsync(IDbConnection connection,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[DriverFreeFee]
                            ([StartDate], [EndDate], [IsActive])
                            VALUES (@StartDate, @EndDate, @IsActive)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            StartDate = startDate,
            EndDate = endDate,
            IsActive = isActive
        }, transaction);
    }

    public static async Task<bool> UpdateDriverFreeFeeAsync(IDbConnection connection,
        int id,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DriverFreeFee]
                                SET 
                                [StartDate] = @StartDate,
                                [EndDate] = @EndDate,
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = isActive
        }, transaction);

        return updatedRowsCount > 0;
    }
    public static async Task<bool> DeActivateDriverFreeFeeAsync(IDbConnection connection,
            int id,
            IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DriverFreeFee]
                                SET 
                                [IsActive] = @IsActive,
                                [EndDate] = @EndDate
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            IsActive = false,
            EndDate = SaudiDateTimeHelper.GetCurrentTime()
        }, transaction);

        return updatedRowsCount > 0;
    }
    public static async Task<bool> DeleteDriverFreeFeeAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[DriverFreeFee]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<DriverFreeFee?> GetDriverFreeFeeByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [StartDate], 
                            [EndDate], 
                            [IsActive]
                            FROM [dbo].[DriverFreeFee]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<DriverFreeFee>(query, new
        {
            Id = id
        });
    }
    public static async Task<IEnumerable<DriverFreeFee>> GetAllAsync(IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [StartDate], 
                            [EndDate], 
                            [IsActive]
                            FROM [dbo].[DriverFreeFee]";

        return await connection.QueryAsync<DriverFreeFee>(query);
    }
    public static async Task<DriverFreeFee?> GetActiveDriverFreeFeesAsync(IDbConnection connection)
    {
        const string query = @"SELECT 
                                    [Id], 
                                    [StartDate], 
                                    [EndDate], 
                                    [IsActive]
                                   FROM [dbo].[DriverFreeFee]
                                   WHERE 
                                    GETDATE() BETWEEN [StartDate] AND [EndDate] AND [IsActive] = 1";

        return await connection.QueryFirstOrDefaultAsync<DriverFreeFee>(query);
    }
}

public class DriverFreeFee
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
