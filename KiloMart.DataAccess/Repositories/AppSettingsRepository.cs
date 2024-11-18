using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Repositories;

namespace KiloMart.DataAccess.EFCore.Repositories;

public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly IDbFactory _dbFactory;

    public AppSettingsRepository(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<string?> GetSettingAsync(int key)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT Value FROM AppSetting WHERE [Key] = @Key";
        var value = await connection.QueryFirstOrDefaultAsync<string>(query, new { Key = key });
        return value;
    }

    public async Task UpdateSettingAsync(int key, string value)
    {
        using (var connection = _dbFactory.CreateDbConnection())
        {
            var query = @"
                    IF EXISTS (SELECT 1 FROM AppSetting WHERE [Key] = @Key)
                        UPDATE AppSetting SET Value = @Value WHERE [Key] = @Key
                    ELSE
                        INSERT INTO AppSetting ([Key], Value) VALUES (@Key, @Value)";

            await connection.ExecuteAsync(query, new { Key = key, Value = value });
        }
    }
}