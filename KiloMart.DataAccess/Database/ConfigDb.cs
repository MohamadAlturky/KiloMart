using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[Configs](
///     [Key] [varchar](100) NOT NULL,
///     [Value] [varchar](100) NOT NULL
/// )
/// </summary>
public static partial class Db
{
    public static async Task<bool> InsertConfigAsync(IDbConnection connection,
        string key,
        string value,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Configs]
                            ([Key], [Value])
                            VALUES (@Key, @Value)";

        var affectedRows = await connection.ExecuteAsync(query, new
        {
            Key = key,
            Value = value
        }, transaction);

        return affectedRows > 0;
    }

    public static async Task<bool> UpdateConfigAsync(IDbConnection connection,
        string key,
        string value,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Configs]
                                SET [Value] = @Value
                                WHERE [Key] = @Key";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Key = key,
            Value = value
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteConfigAsync(IDbConnection connection, string key, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Configs]
                                WHERE [Key] = @Key";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Key = key
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Config?> GetConfigByKeyAsync(string key, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Key], 
                            [Value]
                            FROM [dbo].[Configs]
                            WHERE [Key] = @Key";

        return await connection.QueryFirstOrDefaultAsync<Config>(query, new
        {
            Key = key
        });
    }

    public static async Task<IEnumerable<Config>> GetAllConfigsAsync(IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Key], 
                            [Value]
                            FROM [dbo].[Configs]";

        return await connection.QueryAsync<Config>(query);
    }
}

public class Config
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}