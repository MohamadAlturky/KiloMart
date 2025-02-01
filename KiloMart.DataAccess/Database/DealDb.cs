using Dapper;
using KiloMart.Domain.DateServices;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<int> InsertDealAsync(IDbConnection connection,
        int product,
        bool isActive,
        float offPercentage,
        DateTime startDate,
        DateTime endDate,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Deal]
                            ([Product], [IsActive], [OffPercentage], [StartDate], [EndDate])
                            VALUES (@Product, @IsActive, @OffPercentage, @StartDate, @EndDate)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Product = product,
            IsActive = isActive,
            OffPercentage = offPercentage,
            StartDate = startDate,
            EndDate = endDate
        }, transaction);
    }

    public static async Task<bool> UpdateDealAsync(IDbConnection connection,
        int id,
        int product,
        bool isActive,
        float offPercentage,
        DateTime startDate,
        DateTime endDate,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Deal]
                                SET 
                                [Product] = @Product,
                                [IsActive] = @IsActive,
                                [OffPercentage] = @OffPercentage,
                                [StartDate] = @StartDate,
                                [EndDate] = @EndDate
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Product = product,
            IsActive = isActive,
            OffPercentage = offPercentage,
            StartDate = startDate,
            EndDate = endDate
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeActivateDealAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Deal]
                                SET 
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            IsActive = false,
            EndDate = SaudiDateTimeHelper.GetCurrentTime()
        }, transaction);

        return updatedRowsCount > 0;
    }
    public static async Task<bool> ActivateDealAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Deal]
                                SET 
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            IsActive = true,
            EndDate = SaudiDateTimeHelper.GetCurrentTime()
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteDealAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Deal]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Deal?> GetDealByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Product], 
                            [IsActive], 
                            [OffPercentage], 
                            [StartDate], 
                            [EndDate]
                            FROM [dbo].[Deal]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Deal>(query, new
        {
            Id = id
        });
    }
    public static async Task<bool> HasActiveDealAsync(int product, DateTime startDate, DateTime endDate, IDbConnection connection)
    {
        const string query = @"
        SELECT TOP(1) 1 AS Status 
        FROM [dbo].[Deal]
        WHERE [IsActive] = 1 
          AND [Product] = @Product
          AND (NOT (@EndDate <= [StartDate] OR @StartDate >= [EndDate]))";

        var result = await connection.QueryFirstOrDefaultAsync<int?>(query, new
        {
            Product = product,
            StartDate = startDate,
            EndDate = endDate
        });

        // If result is not null, it means there is at least one active deal
        return result.HasValue;
    }


    public static async Task<IEnumerable<Deal>> GetAllDealsForProductAsync(IDbConnection connection,
    int product)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Product], 
                            [IsActive], 
                            [OffPercentage], 
                            [StartDate], 
                            [EndDate]
                            FROM [dbo].[Deal] WHERE [Product] = @Product";

        return await connection.QueryAsync<Deal>(query, new { Product = product });
    }

    public static async Task<IEnumerable<Deal>> GetActiveDealsAsync(IDbConnection connection)
    {
        const string query = @"SELECT 
                                    [Id], 
                                    [Product], 
                                    [IsActive], 
                                    [OffPercentage], 
                                    [StartDate], 
                                    [EndDate]
                                   FROM [dbo].[Deal]
                                   WHERE 
                                    GETDATE() BETWEEN [StartDate] AND [EndDate] AND [IsActive] = 1";

        return await connection.QueryAsync<Deal>(query);
    }
    public static async Task<IEnumerable<Deal>> GetActiveDealsByProductAsync(IDbConnection connection, int product)
    {
        const string query = @"SELECT 
                                    [Id], 
                                    [Product], 
                                    [IsActive], 
                                    [OffPercentage], 
                                    [StartDate], 
                                    [EndDate]
                                   FROM [dbo].[Deal]
                                   WHERE 
                                    GETDATE() BETWEEN [StartDate] AND [EndDate] AND [IsActive] = 1 AND [Product] = @Product";

        return await connection.QueryAsync<Deal>(query, new { Product = product });
    }
}

public class Deal
{
    public int Id { get; set; }
    public int Product { get; set; }
    public bool IsActive { get; set; }
    public float OffPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
