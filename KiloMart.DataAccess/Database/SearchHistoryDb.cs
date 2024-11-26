using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[SearchHistory](
///     [Id] [bigint] IDENTITY(1,1) NOT NULL,
///     [Customer] [int] NOT NULL,
///     [Term] [varchar](50) NOT NULL
/// )
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertSearchHistoryAsync(IDbConnection connection,
        int customer,
        string term,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[SearchHistory]
                            ([Customer], [Term])
                            VALUES (@Customer, @Term)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Customer = customer,
            Term = term
        }, transaction);
    }

    public static async Task<bool> UpdateSearchHistoryAsync(IDbConnection connection,
        long id,
        int customer,
        string term,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[SearchHistory]
                                SET 
                                [Customer] = @Customer,
                                [Term] = @Term
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Customer = customer,
            Term = term
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteSearchHistoryAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[SearchHistory]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<SearchHistory?> GetSearchHistoryByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Customer], 
                            [Term]
                            FROM [dbo].[SearchHistory]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<SearchHistory>(query, new
        {
            Id = id
        });
    }
    public static async Task<IEnumerable<SearchHistory>> GetLastSearchesByCustomerAsync(IDbConnection connection, int customerId, int count)
    {
        const string query = @"SELECT TOP (@Count) 
                            [Id], 
                            [Customer], 
                            [Term]
                          FROM [dbo].[SearchHistory]
                          WHERE [Customer] = @CustomerId
                          ORDER BY [Id] DESC"; // Assuming Id is auto-incremented and represents the order of insertion

        return await connection.QueryAsync<SearchHistory>(query, new
        {
            CustomerId = customerId,
            Count = count
        });
    }
}

public class SearchHistory
{
    public long Id { get; set; }
    public int Customer { get; set; }
    public string Term { get; set; } = null!;
}