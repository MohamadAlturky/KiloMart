using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string ProviderWalletSql => @"
    SELECT
        [Id],
        [Value],
        [Provider]
    FROM 
        dbo.[ProviderWallet]";

    public static DbQuery<ProviderWalletSqlResponse> ProviderWalletSqlQuery 
    => new(ProviderWalletSql);
}

public class ProviderWalletSqlResponse
{
    public int Id { get; set; }
    public double Value { get; set; }
    public int Provider { get; set; }
}
