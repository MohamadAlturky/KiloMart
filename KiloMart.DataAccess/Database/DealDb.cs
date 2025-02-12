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

    public static async Task<IEnumerable<DealWithProductDetails>> GetActiveDealsByProductAsync(IDbConnection connection, byte language)
    {
        const string query = @"
        SELECT 
            d.[Id], 
            d.[Product], 
            d.[IsActive], 
            d.[OffPercentage], 
            d.[StartDate], 
            d.[EndDate],
            pd.[ProductId],
            pd.[ProductImageUrl],
            pd.[ProductIsActive],
            pd.[ProductMeasurementUnit],
            pd.[ProductDescription],
            pd.[ProductName],
            pd.[ProductCategoryId],
            pd.[ProductCategoryIsActive],
            pd.[ProductCategoryName]
        FROM [dbo].[Deal] d
        INNER JOIN 
        dbo.GetProductDetailsFN(@language) pd
        ON pd.ProductId = d.Product
        WHERE 
        GETDATE() BETWEEN d.[StartDate] AND d.[EndDate] 
        AND d.[IsActive] = 1";

        return await connection.QueryAsync<DealWithProductDetails>(query, new { Language = language });
    }

    public static async Task<(IEnumerable<DealWithProductDetails> Deals, int TotalCount)> GetActiveDealsByProductAsync(IDbConnection connection, byte language, int pageNumber, int pageSize)
    {
        const string query = @"
        SELECT 
            d.[Id], 
            d.[Product], 
            d.[IsActive], 
            d.[OffPercentage], 
            d.[StartDate], 
            d.[EndDate],
            pd.[ProductId],
            pd.[ProductImageUrl],
            pd.[ProductIsActive],
            pd.[ProductMeasurementUnit],
            pd.[ProductDescription],
            pd.[ProductName],
            pd.[ProductCategoryId],
            pd.[ProductCategoryIsActive],
            pd.[ProductCategoryName]
        FROM [dbo].[Deal] d
        INNER JOIN 
        dbo.GetProductDetailsFN(@language) pd
        ON pd.ProductId = d.Product
        WHERE 
        GETDATE() BETWEEN d.[StartDate] AND d.[EndDate] 
        ORDER BY d.[Id]
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

        const string countQuery = @"
        SELECT COUNT(*)
        FROM [dbo].[Deal] d
        WHERE 
        GETDATE() BETWEEN d.[StartDate] AND d.[EndDate]";

        int offset = (pageNumber - 1) * pageSize;

        var deals = await connection.QueryAsync<DealWithProductDetails>(query, new
        {
            Language = language,
            Offset = offset,
            PageSize = pageSize
        });

        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Language = language });

        return (deals, totalCount);
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
public class DealWithProductDetails
{
    public int Id { get; set; }
    public int Product { get; set; }
    public bool IsActive { get; set; }
    public float OffPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; }
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; }
    public string ProductDescription { get; set; }
    public string ProductName { get; set; }
    public int ProductCategoryId { get; set; }
    public bool ProductCategoryIsActive { get; set; }
    public string ProductCategoryName { get; set; }
}