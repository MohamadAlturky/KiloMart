using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<long> InsertWithdrawAsync(IDbConnection connection,
        int delivery,
        string bankAccountNumber,
        string ibanNumber,
        DateTime date,
        bool done,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Withdraw]
                            ([Delivery], [BankAccountNumber], [IBanNumber], [Date], [Done])
                            VALUES (@Delivery, @BankAccountNumber, @IBanNumber, @Date, @Done)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Delivery = delivery,
            BankAccountNumber = bankAccountNumber,
            IBanNumber = ibanNumber,
            Date = date,
            Done = done
        }, transaction);
    }

    public static async Task<bool> UpdateWithdrawAsync(IDbConnection connection,
        long id, 
        int delivery, 
        string bankAccountNumber, 
        string ibanNumber, 
        DateTime date,
        bool done, 
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Withdraw]
                                SET 
                                [Delivery] = @Delivery,
                                [BankAccountNumber] = @BankAccountNumber,
                                [IBanNumber] = @IBanNumber,
                                [Date] = @Date,
                                [Done] = @Done
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Delivery = delivery,
            BankAccountNumber = bankAccountNumber,
            IBanNumber = ibanNumber,
            Date = date,
            Done = done
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
                            [Delivery], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done]
                            FROM [dbo].[Withdraw]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Withdraw>(query, new
        {
            Id = id
        });
    }
       /// <summary>
    /// Retrieves a list of Withdraw records filtered by Delivery.
    /// </summary>
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByDeliveryAsync(int delivery, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Delivery], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done]
                            FROM [dbo].[Withdraw]
                            WHERE [Delivery] = @Delivery";

        return await connection.QueryAsync<Withdraw>(query, new { Delivery = delivery });
    }

    /// <summary>
    /// Retrieves a list of Withdraw records filtered by Done status.
    /// </summary>
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByDoneAsync(bool done, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Delivery], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done]
                            FROM [dbo].[Withdraw]
                            WHERE [Done] = @Done";

        return await connection.QueryAsync<Withdraw>(query, new { Done = done });
    }

    /// <summary>
    /// Retrieves a list of Withdraw records filtered by both Delivery and Done status.
    /// </summary>
    public static async Task<IEnumerable<Withdraw>> GetWithdrawsByDeliveryAndDoneAsync(int delivery, bool done, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Delivery], 
                            [BankAccountNumber], 
                            [IBanNumber], 
                            [Date], 
                            [Done]
                            FROM [dbo].[Withdraw]
                            WHERE [Delivery] = @Delivery AND [Done] = @Done";

        return await connection.QueryAsync<Withdraw>(query, new { Delivery = delivery, Done = done });
    }


    /// <summary>
    /// Retrieves a paginated list of Withdraw records filtered by Delivery along with the total count.
    /// </summary>
    public static async Task<(IEnumerable<Withdraw> Withdraws, int TotalCount)> GetPaginatedWithdrawsByDeliveryAsync(int delivery, int pageNumber, int pageSize, IDbConnection connection)
    {
        const string withdrawQuery = @"SELECT 
                                        [Id], 
                                        [Delivery], 
                                        [BankAccountNumber], 
                                        [IBanNumber], 
                                        [Date], 
                                        [Done]
                                        FROM [dbo].[Withdraw]
                                        WHERE [Delivery] = @Delivery
                                        ORDER BY [Id]  DESC
                                        OFFSET @Offset ROWS
                                        FETCH NEXT @PageSize ROWS ONLY";

        const string countQuery = @"SELECT COUNT(*) FROM [dbo].[Withdraw] WHERE [Delivery] = @Delivery";

        var parameters = new
        {
            Delivery = delivery,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        var withdraws = await connection.QueryAsync<Withdraw>(withdrawQuery, parameters);
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Delivery = delivery });

        return (withdraws, totalCount);
    }

    /// <summary>
    /// Retrieves a paginated list of Withdraw records filtered by Done status along with the total count.
    /// </summary>
    public static async Task<(IEnumerable<Withdraw> Withdraws, int TotalCount)> GetPaginatedWithdrawsByDoneAsync(bool done, int pageNumber, int pageSize, IDbConnection connection)
    {
        const string withdrawQuery = @"SELECT 
                                        [Id], 
                                        [Delivery], 
                                        [BankAccountNumber], 
                                        [IBanNumber], 
                                        [Date], 
                                        [Done]
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
    /// Retrieves a paginated list of Withdraw records filtered by both Delivery and Done status along with the total count.
    /// </summary>
    public static async Task<(IEnumerable<Withdraw> Withdraws, int TotalCount)> GetPaginatedWithdrawsByDeliveryAndDoneAsync(int delivery, bool done, int pageNumber, int pageSize, IDbConnection connection)
    {
        const string withdrawQuery = @"SELECT 
                                        [Id], 
                                        [Delivery], 
                                        [BankAccountNumber], 
                                        [IBanNumber], 
                                        [Date], 
                                        [Done]
                                        FROM [dbo].[Withdraw]
                                        WHERE [Delivery] = @Delivery AND [Done] = @Done
                                        ORDER BY [Id] DESC
                                        OFFSET @Offset ROWS
                                        FETCH NEXT @PageSize ROWS ONLY";

        const string countQuery = @"SELECT COUNT(*) FROM [dbo].[Withdraw] WHERE [Delivery] = @Delivery AND [Done] = @Done";

        var parameters = new
        {
            Delivery = delivery,
            Done = done,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        var withdraws = await connection.QueryAsync<Withdraw>(withdrawQuery, parameters);
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Delivery = delivery, Done = done });

        return (withdraws, totalCount);
    }
}


public class Withdraw
{
    public long Id { get; set; }
    public int Delivery { get; set; }
    public string BankAccountNumber { get; set; } = null!;
    public string IBanNumber { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Done { get; set; }
}