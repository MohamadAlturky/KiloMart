using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DeliveryWalletSql => @"
    SELECT
        [Id],
        [Value],
        [Delivery]
    FROM 
        dbo.[DeliveryWallet]";

    public static DbQuery<DeliveryWalletSqlResponse> DeliveryWalletSqlQuery 
    => new(DeliveryWalletSql);
}

public class DeliveryWalletSqlResponse
{
    public int Id { get; set; }
    public double Value { get; set; }
    public int Delivery { get; set; }
}
