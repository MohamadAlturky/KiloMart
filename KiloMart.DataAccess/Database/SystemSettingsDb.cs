using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;
public static partial class Db
{
    public static async Task<bool> UpdateSystemSettingsAsync(IDbConnection connection,
        int id,
        decimal deliveryOrderFee,
        decimal systemOrderFee,
        bool cancelOrderWhenNoProviderHasAllProducts,
        int timeInMinutesToMakeTheCircleBigger,
        decimal circleRaduis,
        int maxMinutesToCancelOrderWaitingAProvider,
        decimal minOrderValue,
        decimal distanceToAdd,
        decimal maxDistanceToAdd,
        decimal raduisForGetProducts,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[SystemSettings]
                                SET 
                                [RaduisForGetProducts] = @RaduisForGetProducts,
                                [DeliveryOrderFee] = @DeliveryOrderFee,
                                [SystemOrderFee] = @SystemOrderFee,
                                [CancelOrderWhenNoProviderHasAllProducts] = @CancelOrderWhenNoProviderHasAllProducts,
                                [TimeInMinutesToMakeTheCircleBigger] = @TimeInMinutesToMakeTheCircleBigger,
                                [CircleRaduis] = @CircleRaduis,
                                [MaxMinutesToCancelOrderWaitingAProvider] = @MaxMinutesToCancelOrderWaitingAProvider,
                                [MinOrderValue] = @MinOrderValue,
                                [DistanceToAdd] = @DistanceToAdd,
                                [MaxDistanceToAdd] = @MaxDistanceToAdd
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            DeliveryOrderFee = deliveryOrderFee,
            SystemOrderFee = systemOrderFee,
            CancelOrderWhenNoProviderHasAllProducts = cancelOrderWhenNoProviderHasAllProducts,
            TimeInMinutesToMakeTheCircleBigger = timeInMinutesToMakeTheCircleBigger,
            CircleRaduis = circleRaduis,
            MaxMinutesToCancelOrderWaitingAProvider = maxMinutesToCancelOrderWaitingAProvider,
            MinOrderValue = minOrderValue,
            DistanceToAdd = distanceToAdd,
            MaxDistanceToAdd = maxDistanceToAdd,
            RaduisForGetProducts = raduisForGetProducts
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<SystemSettings?> GetSystemSettingsByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [DeliveryOrderFee], 
                            [SystemOrderFee], 
                            [CancelOrderWhenNoProviderHasAllProducts],
                            [TimeInMinutesToMakeTheCircleBigger],
                            [CircleRaduis],
                            [MaxMinutesToCancelOrderWaitingAProvider],
                            [MinOrderValue],
                            [DistanceToAdd],
                            [MaxDistanceToAdd],
                            [RaduisForGetProducts]
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
    public decimal DeliveryOrderFee { get; set; }
    public decimal SystemOrderFee { get; set; }
    public bool CancelOrderWhenNoProviderHasAllProducts { get; set; }
    public int TimeInMinutesToMakeTheCircleBigger { get; set; }
    public decimal CircleRaduis { get; set; }
    public int MaxMinutesToCancelOrderWaitingAProvider { get; set; }
    public decimal MinOrderValue { get; set; }
    public decimal DistanceToAdd { get; set; }
    public decimal MaxDistanceToAdd { get; set; }
    public decimal RaduisForGetProducts { get; set; }
}
