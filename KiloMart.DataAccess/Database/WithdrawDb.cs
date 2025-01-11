using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<long> InsertWithdrawAsync(IDbConnection connection,
        int Party,
        string bankAccountNumber,
        string ibanNumber,
        DateTime date,
        bool done,
        bool accepted,
        bool rejected,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Withdraw]
                            ([Party], [BankAccountNumber], [IBanNumber], [Date], [Done],[Rejected],[Accepted])
                            VALUES (@Party, @BankAccountNumber, @IBanNumber, @Date, @Done,@Rejected,@Accepted)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Party = Party,
            BankAccountNumber = bankAccountNumber,
            IBanNumber = ibanNumber,
            Date = date,
            Done = done,
            Accepted = accepted,
            Rejected = rejected
        }, transaction);
    }

    public static async Task<bool> UpdateWithdrawAsync(IDbConnection connection,
        long id,
        int Party,
        string bankAccountNumber,
        string ibanNumber,
        DateTime date,
        bool done,
                bool accepted,
        bool rejected,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Withdraw]
                                SET 
                                [Party] = @Party,
                                [BankAccountNumber] = @BankAccountNumber,
                                [IBanNumber] = @IBanNumber,
                                [Date] = @Date,
                                [Rejected] = @Rejected,
                                [Accepted] = @Accepted,
                                [Done] = @Done
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Party = Party,
            BankAccountNumber = bankAccountNumber,
            IBanNumber = ibanNumber,
            Date = date,
            Done = done,
            Accepted = accepted,
            Rejected = rejected
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteWithdrawAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Withdraw]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Withdraw?> GetWithdrawByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Party], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done],
                            [Accepted],
                            [Rejected]
                            FROM [dbo].[Withdraw]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Withdraw>(query, new
        {
            Id = id
        });
    }
    /// <summary>
    /// Retrieves a list of Withdraw records filtered by Party.
    /// </summary>
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByPartyAsync(int Party, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Party], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done],
                            [Accepted],
                            [Rejected]
                            FROM [dbo].[Withdraw]
                            WHERE [Party] = @Party";

        return await connection.QueryAsync<Withdraw>(query, new { Party = Party });
    }


    /// <summary>
    /// Retrieves a list of Withdraw records filtered by Done status.
    /// </summary>
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByDoneAsync(bool done, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Party], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done],
                            [Accepted],
                            [Rejected]
                            FROM [dbo].[Withdraw]
                            WHERE [Done] = @Done";

        return await connection.QueryAsync<Withdraw>(query, new { Done = done });
    }
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByRejectedAsync(bool Rejected, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Party], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done],
                            [Accepted],
                            [Rejected]
                            FROM [dbo].[Withdraw]
                            WHERE [Rejected] = @Rejected";

        return await connection.QueryAsync<Withdraw>(query, new { Rejected = Rejected });
    }
    /// <summary>
    /// Retrieves a list of Withdraw records filtered by both Party and Done status.
    /// </summary>
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByPartyAndDoneAsync(int Party, bool done, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Party], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done],
                            [Accepted],
                            [Rejected]
                            FROM [dbo].[Withdraw]
                            WHERE [Party] = @Party AND [Done] = @Done";

        return await connection.QueryAsync<Withdraw>(query, new { Party = Party, Done = done });
    }


    /// <summary>
    /// Retrieves a paginated list of Withdraw records filtered by Party along with the total count.
    /// </summary>
    public static async Task<(IEnumerable<Withdraw> Withdraws, int TotalCount)> GetPaginatedWithdrawsByPartyAsync(int Party, int pageNumber, int pageSize, IDbConnection connection)
    {
        const string withdrawQuery = @"SELECT 
                                        [Id], 
                                        [Party], 
                                        [BankAccountNumber], 
                                        [IBanNumber], 
                                        [Date], 
                                        [Done],
                                        [Accepted],
                                        [Rejected]
                                        FROM [dbo].[Withdraw]
                                        WHERE [Party] = @Party
                                        ORDER BY [Id]  DESC
                                        OFFSET @Offset ROWS
                                        FETCH NEXT @PageSize ROWS ONLY";

        const string countQuery = @"SELECT COUNT(*) FROM [dbo].[Withdraw] WHERE [Party] = @Party";

        var parameters = new
        {
            Party = Party,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        var withdraws = await connection.QueryAsync<Withdraw>(withdrawQuery, parameters);
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Party = Party });

        return (withdraws, totalCount);
    }

    /// <summary>
    /// Retrieves a paginated list of Withdraw records filtered by Done status along with the total count.
    /// </summary>
    public static async Task<(IEnumerable<Withdraw> Withdraws, int TotalCount)> GetPaginatedWithdrawsByDoneAsync(bool done, int pageNumber, int pageSize, IDbConnection connection)
    {
        const string withdrawQuery = @"SELECT 
                                        [Id], 
                                        [Party], 
                                        [BankAccountNumber], 
                                        [IBanNumber], 
                                        [Date], 
                                        [Done],
                                        [Accepted],
                                        [Rejected]
                                        FROM [dbo].[Withdraw]
                                        WHERE [Done] = @Done
                                        ORDER BY [Id]  DESC
                                        OFFSET @Offset ROWS
                                        FETCH NEXT @PageSize ROWS ONLY";

        const string countQuery = @"SELECT COUNT(*) FROM [dbo].[Withdraw] WHERE [Done] = @Done";

        var parameters = new
        {
            Done = done,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        var withdraws = await connection.QueryAsync<Withdraw>(withdrawQuery, parameters);
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Done = done });

        return (withdraws, totalCount);
    }

    /// <summary>
    /// Retrieves a paginated list of Withdraw records filtered by both Party and Done status along with the total count.
    /// </summary>
    public static async Task<(IEnumerable<Withdraw> Withdraws, int TotalCount)> GetPaginatedWithdrawsByPartyAndDoneAsync(int Party, bool done, int pageNumber, int pageSize, IDbConnection connection)
    {
        const string withdrawQuery = @"SELECT 
                                        [Id], 
                                        [Party], 
                                        [BankAccountNumber], 
                                        [IBanNumber], 
                                        [Date], 
                                        [Done],
                                        [Accepted],
                                        [Rejected]
                                        FROM [dbo].[Withdraw]
                                        WHERE [Party] = @Party AND [Done] = @Done
                                        ORDER BY [Id] DESC
                                        OFFSET @Offset ROWS
                                        FETCH NEXT @PageSize ROWS ONLY";

        const string countQuery = @"SELECT COUNT(*) FROM [dbo].[Withdraw] WHERE [Party] = @Party AND [Done] = @Done";

        var parameters = new
        {
            Party = Party,
            Done = done,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        var withdraws = await connection.QueryAsync<Withdraw>(withdrawQuery, parameters);
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Party = Party, Done = done });

        return (withdraws, totalCount);
    }
}


public class Withdraw
{
    public long Id { get; set; }
    public int Party { get; set; }
    public string BankAccountNumber { get; set; } = null!;
    public string IBanNumber { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Done { get; set; }
    public bool Accepted { get; set; }
    public bool Rejected { get; set; }
}