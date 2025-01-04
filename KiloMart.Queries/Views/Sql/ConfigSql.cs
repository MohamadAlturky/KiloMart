using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ConfigSql => @"
    SELECT
        [Key],
        [Value]
    FROM 
        dbo.[Configs]";

    public static DbQuery<ConfigSqlResponse> ConfigSqlQuery 
    => new(ConfigSql);
}

public class ConfigSqlResponse
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
