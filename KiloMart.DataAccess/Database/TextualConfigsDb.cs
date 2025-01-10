using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{

    // Update an existing textual config
    public static async Task<bool> UpdateTextualConfigAsync(IDbConnection connection,
        TextualConfigKey key,
        string value,
        byte language,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[TextualConfigs]
                                SET 
                                [Value] = @Value
                                WHERE [Key] = @Key AND [Language] = @Language";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Key = (int)key,
            Value = value,
            Language = language
        }, transaction);

        return updatedRowsCount > 0;
    }


    // Get a textual config by key and language
    public static async Task<TextualConfig?> GetTextualConfigByKeyAndLanguageAsync(IDbConnection connection, TextualConfigKey key, byte language)
    {
        const string query = @"SELECT 
                            [Key], 
                            [Value], 
                            [Language]
                            FROM [dbo].[TextualConfigs]
                            WHERE [Key] = @Key AND [Language] = @Language";

        return await connection.QueryFirstOrDefaultAsync<TextualConfig>(query, new
        {
            Key = (int)key,
            Language = language
        });
    }
}

// TextualConfig class to represent the table
public class TextualConfig
{
    public int Key { get; set; }
    public string Value { get; set; } = null!;
    public byte Language { get; set; }
}

public enum TextualConfigKey
{
    TermsAndConditions = 1,
    AboutApp = 2
}