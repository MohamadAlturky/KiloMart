using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;
public static partial class Db
{
    // Insert a new session record
    public static async Task<long> InsertSessionAsync(
        IDbConnection connection,
        string token,
        int userId,
        DateTime expireDate,
        DateTime creationDate,
        string code,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Sessions]
                                ([Token], [UserId], [ExpireDate], [CreationDate], [Code])
                                VALUES (@Token, @UserId, @ExpireDate, @CreationDate, @Code)
                                SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Token = token,
            UserId = userId,
            ExpireDate = expireDate,
            CreationDate = creationDate,
            Code = code
        }, transaction);
    }

    // Update an existing session record by Id
    public static async Task<bool> UpdateSessionAsync(
        IDbConnection connection,
        long id,
        string? token = null,
        DateTime? expireDate = null,
        string? code = null,
        IDbTransaction? transaction = null)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        var updateColumns = new System.Text.StringBuilder();

        if (token != null)
        {
            parameters.Add("@Token", token);
            updateColumns.AppendLine("[Token] = @Token,");
        }
        if (expireDate.HasValue)
        {
            parameters.Add("@ExpireDate", expireDate.Value);
            updateColumns.AppendLine("[ExpireDate] = @ExpireDate,");
        }
        if (code != null)
        {
            parameters.Add("@Code", code);
            updateColumns.AppendLine("[Code] = @Code,");
        }

        if (updateColumns.Length == 0)
            return true; // Nothing to update

        updateColumns.Length -= 1; // Remove the trailing comma

        const string query = @"UPDATE [dbo].[Sessions]
                                SET {0}
                                WHERE [Id] = @Id";

        var fullQuery = string.Format(query, updateColumns.ToString());

        var updatedRowsCount = await connection.ExecuteAsync(fullQuery, parameters, transaction);

        return updatedRowsCount > 0;
    }

    // Delete a session record by Id
    public static async Task<bool> DeleteSessionAsync(
        IDbConnection connection,
        long id,
        IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Sessions]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    // Retrieve a session by Id
    public static async Task<Sessions?> GetSessionByIdAsync(
        IDbConnection connection,
        long id,
        IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Sessions>(query, new
        {
            Id = id
        }, transaction);
    }
    public static async Task<Sessions?> GetSessionByCodeAsync(
        IDbConnection connection,
        string code,
        IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                WHERE [Code] = @Code";

        return await connection.QueryFirstOrDefaultAsync<Sessions>(query, new
        {
            Code = code
        }, transaction);
    }


    // Retrieve sessions by UserId
    public static async Task<IEnumerable<Sessions>> GetSessionsByUserIdAsync(
        IDbConnection connection,
        int userId,
        IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                WHERE [UserId] = @UserId";

        return await connection.QueryAsync<Sessions>(query, new
        {
            UserId = userId
        }, transaction);
    }

    public static async Task<IEnumerable<Sessions>> GetActiveSessionsByUserIdAsync(
        IDbConnection connection,
        int userId,
        IDbTransaction? transaction = null)
    {
        DateTime now = DateTime.Now;
        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                WHERE [UserId] = @UserId AND [ExpireDate] > @Now";

        return await connection.QueryAsync<Sessions>(query, new
        {
            UserId = userId,
            Now = now
        }, transaction);
    }


    public static async Task<IEnumerable<Sessions>> GetActiveSessionsByUserIdAsync(
        IDbConnection connection,
        int userId,
        string code,
        IDbTransaction? transaction = null)
    {
        DateTime now = DateTime.Now;
        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                WHERE [UserId] = @UserId AND [ExpireDate] > @Now AND [Code] = @Code";

        return await connection.QueryAsync<Sessions>(query, new
        {
            UserId = userId,
            Now = now,
            Code = code
        }, transaction);
    }

    // Retrieve a session by Token
    public static async Task<Sessions?> GetSessionsByTokenAsync(
        IDbConnection connection,
        string token,
        IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                WHERE [Token] = @Token";

        return await connection.QueryFirstOrDefaultAsync<Sessions>(query, new
        {
            Token = token
        }, transaction);
    }

    // Retrieve sessions with optional filters and pagination
    public static async Task<IEnumerable<Sessions>> GetSessionsAsync(
        IDbConnection connection,
        int? userId = null,
        string? token = null,
        DateTime? expireDateFrom = null,
        DateTime? expireDateTo = null,
        DateTime? creationDateFrom = null,
        DateTime? creationDateTo = null,
        string? code = null,
        int pageNumber = 1,
        int pageSize = 10,
        IDbTransaction? transaction = null)
    {
        var parameters = new DynamicParameters();

        var whereClauses = new System.Text.StringBuilder();

        if (userId.HasValue)
        {
            whereClauses.AppendLine("[UserId] = @UserId");
            parameters.Add("@UserId", userId.Value);
        }
        if (!string.IsNullOrEmpty(token))
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[Token] = @Token");
            parameters.Add("@Token", token);
        }
        if (expireDateFrom.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[ExpireDate] >= @ExpireDateFrom");
            parameters.Add("@ExpireDateFrom", expireDateFrom.Value);
        }
        if (expireDateTo.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[ExpireDate] <= @ExpireDateTo");
            parameters.Add("@ExpireDateTo", expireDateTo.Value);
        }
        if (creationDateFrom.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[CreationDate] >= @CreationDateFrom");
            parameters.Add("@CreationDateFrom", creationDateFrom.Value);
        }
        if (creationDateTo.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[CreationDate] <= @CreationDateTo");
            parameters.Add("@CreationDateTo", creationDateTo.Value);
        }
        if (!string.IsNullOrEmpty(code))
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[Code] = @Code");
            parameters.Add("@Code", code);
        }

        var whereClause = whereClauses.Length > 0 ? " WHERE " + whereClauses.ToString() : "";

        const string query = @"SELECT 
                                [Id], 
                                [Token], 
                                [UserId], 
                                [ExpireDate], 
                                [CreationDate], 
                                [Code]
                                FROM [dbo].[Sessions]
                                {0}
                                ORDER BY [Id] DESC
                                OFFSET @Offset ROWS
                                FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (pageNumber - 1) * pageSize;
        parameters.Add("@Offset", offset);
        parameters.Add("@PageSize", pageSize);

        var fullQuery = string.Format(query, whereClause);

        return await connection.QueryAsync<Sessions>(fullQuery, parameters, transaction);
    }

    // Get count of sessions based on filters
    public static async Task<long> GetSessionsCountAsync(
        IDbConnection connection,
        int? userId = null,
        string? token = null,
        DateTime? expireDateFrom = null,
        DateTime? expireDateTo = null,
        DateTime? creationDateFrom = null,
        DateTime? creationDateTo = null,
        string? code = null,
        IDbTransaction? transaction = null)
    {
        var parameters = new DynamicParameters();

        var whereClauses = new System.Text.StringBuilder();

        if (userId.HasValue)
        {
            whereClauses.AppendLine("[UserId] = @UserId");
            parameters.Add("@UserId", userId.Value);
        }
        if (!string.IsNullOrEmpty(token))
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[Token] = @Token");
            parameters.Add("@Token", token);
        }
        if (expireDateFrom.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[ExpireDate] >= @ExpireDateFrom");
            parameters.Add("@ExpireDateFrom", expireDateFrom.Value);
        }
        if (expireDateTo.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[ExpireDate] <= @ExpireDateTo");
            parameters.Add("@ExpireDateTo", expireDateTo.Value);
        }
        if (creationDateFrom.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[CreationDate] >= @CreationDateFrom");
            parameters.Add("@CreationDateFrom", creationDateFrom.Value);
        }
        if (creationDateTo.HasValue)
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[CreationDate] <= @CreationDateTo");
            parameters.Add("@CreationDateTo", creationDateTo.Value);
        }
        if (!string.IsNullOrEmpty(code))
        {
            if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
            whereClauses.AppendLine("[Code] = @Code");
            parameters.Add("@Code", code);
        }

        var whereClause = whereClauses.Length > 0 ? " WHERE " + whereClauses.ToString() : "";

        const string query = @"SELECT COUNT([Id])
                                FROM [dbo].[Sessions]
                                {0};";

        var fullQuery = string.Format(query, whereClause);

        return await connection.QueryFirstOrDefaultAsync<long>(fullQuery, parameters, transaction);
    }
}


public class Sessions
{
    public long Id { get; set; }
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime CreationDate { get; set; }
    public string Code { get; set; } = null!;
}