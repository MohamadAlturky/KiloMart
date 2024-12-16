using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[FAQ](
///     [Id] [int] IDENTITY(1,1) NOT NULL,
///     [Question] [varchar](200) NOT NULL,
///     [Answer] [varchar](300) NOT NULL,
///     [Language] [tinyint] NOT NULL
/// )
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertFAQAsync(IDbConnection connection,
        string question,
        string answer,
        byte language,
        byte type,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[FAQ]
                            ([Question], [Answer], [Language], [Type])
                            VALUES (@Question, @Answer, @Language, @Type)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Question = question,
            Answer = answer,
            Language = language,
            Type = type
        }, transaction);
    }

    public static async Task<bool> UpdateFAQAsync(IDbConnection connection,
        int id,
        string question,
        string answer,
        byte language,
        byte type,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[FAQ]
                                SET 
                                [Question] = @Question,
                                [Answer] = @Answer,
                                [Type] = @Type,
                                [Language] = @Language
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Question = question,
            Answer = answer,
            Language = language,
            Type = type
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteFAQAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[FAQ]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<FAQ?> GetFAQByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Question], 
                            [Answer], 
                            [Language],
                            [Type]
                            FROM [dbo].[FAQ]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<FAQ>(query, new
        {
            Id = id
        });
    }

    public static async Task<IEnumerable<FAQ>> GetAllFAQsAsync(IDbConnection connection, byte? language = null)
    {
        string query = @"SELECT 
                            [Id], 
                            [Question], 
                            [Answer], 
                            [Language],
                            [Type]
                            FROM [dbo].[FAQ]";

        if (language.HasValue)
        {
            query += " WHERE [Language] = @Language";
            return await connection.QueryAsync<FAQ>(query, new { Language = language.Value });
        }

        return await connection.QueryAsync<FAQ>(query);
    }
    public static async Task<IEnumerable<FAQ>> GetAllFAQsByTypeAsync(
        IDbConnection connection,
        byte type,
        byte language)
    {
        string query = @"SELECT 
                            [Id], 
                            [Question], 
                            [Answer], 
                            [Language],
                            [Type]
                            FROM [dbo].[FAQ]
                            WHERE [Language] = @Language AND [Type] = @Type";
            return await connection.QueryAsync<FAQ>(query, new { Language = language, Type = type });

    }
}

public class FAQ
{
    public int Id { get; set; }
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public byte Language { get; set; }
    public byte Type { get; set; }
}