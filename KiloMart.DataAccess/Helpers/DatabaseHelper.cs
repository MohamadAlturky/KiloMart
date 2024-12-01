using System.Data;
using Dapper;

public static class DatabaseHelper
{
    public static async Task<List<DatabaseSettingsTable>> SelectFromTable(string tableName, IDbConnection connection)
    {
        string sqlQuery = $"SELECT * FROM {tableName};";

        var result = await connection.QueryAsync<DatabaseSettingsTable>(sqlQuery);
        return result.AsList();
    }
}

public class DatabaseSettingsTable
{
    public byte Id {get;set;} 
    public string Name { get; set;} = null!;
}