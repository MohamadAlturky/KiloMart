using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<int> InsertMobileAppConfigurationAsync(IDbConnection connection,
        int id,
        float customerAppMinimumBuildNumberAndroid,
        float customerAppMinimumBuildNumberIos,
        string customerAppUrlAndroid,
        string customerAppUrlIos,
        float providerAppMinimumBuildNumberAndroid,
        float providerAppMinimumBuildNumberIos,
        string providerAppUrlAndroid,
        string providerAppUrlIos,
        float deliveryAppMinimumBuildNumberAndroid,
        float deliveryAppMinimumBuildNumberIos,
        string deliveryAppUrlAndroid,
        string deliveryAppUrlIos,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[MobileAppConfiguration] (
                                [Id],
                                [CustomerAppMinimumBuildNumberAndroid],
                                [CustomerAppMinimumBuildNumberIos],
                                [CustomerAppUrlAndroid],
                                [CustomerAppUrlIos],
                                [ProviderAppMinimumBuildNumberAndroid],
                                [ProviderAppMinimumBuildNumberIos],
                                [ProviderAppUrlAndroid],
                                [ProviderAppUrlIos],
                                [DeliveryAppMinimumBuildNumberAndroid],
                                [DeliveryAppMinimumBuildNumberIos],
                                [DeliveryAppUrlAndroid],
                                [DeliveryAppUrlIos])
                            VALUES (
                                @Id,
                                @CustomerAppMinimumBuildNumberAndroid,
                                @CustomerAppMinimumBuildNumberIos,
                                @CustomerAppUrlAndroid,
                                @CustomerAppUrlIos,
                                @ProviderAppMinimumBuildNumberAndroid,
                                @ProviderAppMinimumBuildNumberIos,
                                @ProviderAppUrlAndroid,
                                @ProviderAppUrlIos,
                                @DeliveryAppMinimumBuildNumberAndroid,
                                @DeliveryAppMinimumBuildNumberIos,
                                @DeliveryAppUrlAndroid,
                                @DeliveryAppUrlIos)
                            SELECT @Id";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Id = id,
            CustomerAppMinimumBuildNumberAndroid = customerAppMinimumBuildNumberAndroid,
            CustomerAppMinimumBuildNumberIos = customerAppMinimumBuildNumberIos,
            CustomerAppUrlAndroid = customerAppUrlAndroid,
            CustomerAppUrlIos = customerAppUrlIos,
            ProviderAppMinimumBuildNumberAndroid = providerAppMinimumBuildNumberAndroid,
            ProviderAppMinimumBuildNumberIos = providerAppMinimumBuildNumberIos,
            ProviderAppUrlAndroid = providerAppUrlAndroid,
            ProviderAppUrlIos = providerAppUrlIos,
            DeliveryAppMinimumBuildNumberAndroid = deliveryAppMinimumBuildNumberAndroid,
            DeliveryAppMinimumBuildNumberIos = deliveryAppMinimumBuildNumberIos,
            DeliveryAppUrlAndroid = deliveryAppUrlAndroid,
            DeliveryAppUrlIos = deliveryAppUrlIos
        }, transaction);
    }

    public static async Task<bool> UpdateMobileAppConfigurationAsync(IDbConnection connection,
        int id,
        float customerAppMinimumBuildNumberAndroid,
        float customerAppMinimumBuildNumberIos,
        string customerAppUrlAndroid,
        string customerAppUrlIos,
        float providerAppMinimumBuildNumberAndroid,
        float providerAppMinimumBuildNumberIos,
        string providerAppUrlAndroid,
        string providerAppUrlIos,
        float deliveryAppMinimumBuildNumberAndroid,
        float deliveryAppMinimumBuildNumberIos,
        string deliveryAppUrlAndroid,
        string deliveryAppUrlIos,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[MobileAppConfiguration]
                                SET 
                                [CustomerAppMinimumBuildNumberAndroid] = @CustomerAppMinimumBuildNumberAndroid,
                                [CustomerAppMinimumBuildNumberIos] = @CustomerAppMinimumBuildNumberIos,
                                [CustomerAppUrlAndroid] = @CustomerAppUrlAndroid,
                                [CustomerAppUrlIos] = @CustomerAppUrlIos,
                                [ProviderAppMinimumBuildNumberAndroid] = @ProviderAppMinimumBuildNumberAndroid,
                                [ProviderAppMinimumBuildNumberIos] = @ProviderAppMinimumBuildNumberIos,
                                [ProviderAppUrlAndroid] = @ProviderAppUrlAndroid,
                                [ProviderAppUrlIos] = @ProviderAppUrlIos,
                                [DeliveryAppMinimumBuildNumberAndroid] = @DeliveryAppMinimumBuildNumberAndroid,
                                [DeliveryAppMinimumBuildNumberIos] = @DeliveryAppMinimumBuildNumberIos,
                                [DeliveryAppUrlAndroid] = @DeliveryAppUrlAndroid,
                                [DeliveryAppUrlIos] = @DeliveryAppUrlIos
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            CustomerAppMinimumBuildNumberAndroid = customerAppMinimumBuildNumberAndroid,
            CustomerAppMinimumBuildNumberIos = customerAppMinimumBuildNumberIos,
            CustomerAppUrlAndroid = customerAppUrlAndroid,
            CustomerAppUrlIos = customerAppUrlIos,
            ProviderAppMinimumBuildNumberAndroid = providerAppMinimumBuildNumberAndroid,
            ProviderAppMinimumBuildNumberIos = providerAppMinimumBuildNumberIos,
            ProviderAppUrlAndroid = providerAppUrlAndroid,
            ProviderAppUrlIos = providerAppUrlIos,
            DeliveryAppMinimumBuildNumberAndroid = deliveryAppMinimumBuildNumberAndroid,
            DeliveryAppMinimumBuildNumberIos = deliveryAppMinimumBuildNumberIos,
            DeliveryAppUrlAndroid = deliveryAppUrlAndroid,
            DeliveryAppUrlIos = deliveryAppUrlIos
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteMobileAppConfigurationAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[MobileAppConfiguration]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<MobileAppConfiguration?> GetMobileAppConfigurationByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [CustomerAppMinimumBuildNumberAndroid],
                            [CustomerAppMinimumBuildNumberIos],
                            [CustomerAppUrlAndroid],
                            [CustomerAppUrlIos],
                            [ProviderAppMinimumBuildNumberAndroid],
                            [ProviderAppMinimumBuildNumberIos],
                            [ProviderAppUrlAndroid],
                            [ProviderAppUrlIos],
                            [DeliveryAppMinimumBuildNumberAndroid],
                            [DeliveryAppMinimumBuildNumberIos],
                            [DeliveryAppUrlAndroid],
                            [DeliveryAppUrlIos]
                            FROM [dbo].[MobileAppConfiguration]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<MobileAppConfiguration>(query, new
        {
            Id = id
        });
    }
}

public class MobileAppConfiguration
{
    public int Id { get; set; }
    public float CustomerAppMinimumBuildNumberAndroid { get; set; }
    public float CustomerAppMinimumBuildNumberIos { get; set; }
    public string CustomerAppUrlAndroid { get; set; } = null!;
    public string CustomerAppUrlIos { get; set; } = null!;
    public float ProviderAppMinimumBuildNumberAndroid { get; set; }
    public float ProviderAppMinimumBuildNumberIos { get; set; }
    public string ProviderAppUrlAndroid { get; set; } = null!;
    public string ProviderAppUrlIos { get; set; } = null!;
    public float DeliveryAppMinimumBuildNumberAndroid { get; set; }
    public float DeliveryAppMinimumBuildNumberIos { get; set; }
    public string DeliveryAppUrlAndroid { get; set; } = null!;
    public string DeliveryAppUrlIos { get; set; } = null!;
}