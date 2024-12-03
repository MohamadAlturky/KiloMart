using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;
public static partial class Db
{
    public static async Task<bool> UpdateSystemSettingsAsync(IDbConnection connection,
        int id,
        double deliveryOrderFee,
        double providerOrderFee,
        bool cancelOrderWhenNoProviderHasAllProducts,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[SystemSettings]
                                SET 
                                [DeliveryOrderFee] = @DeliveryOrderFee,
                                [ProviderOrderFee] = @ProviderOrderFee,
                                [CancelOrderWhenNoProviderHasAllProducts] = @CancelOrderWhenNoProviderHasAllProducts
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            DeliveryOrderFee = deliveryOrderFee,
            ProviderOrderFee = providerOrderFee,
            CancelOrderWhenNoProviderHasAllProducts = cancelOrderWhenNoProviderHasAllProducts
        }, transaction);

        return updatedRowsCount > 0;
    }


    public static async Task<SystemSettings?> GetSystemSettingsByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [DeliveryOrderFee], 
                            [ProviderOrderFee], 
                            [CancelOrderWhenNoProviderHasAllProducts]
                            FROM [dbo].[SystemSettings]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<SystemSettings>(query, new
        {
            Id = id
        });
    }
}
public class SystemSettings
{
    public int Id { get; set; }
    public double DeliveryOrderFee { get; set; }
    public double ProviderOrderFee { get; set; }
    public bool CancelOrderWhenNoProviderHasAllProducts { get; set; }
}