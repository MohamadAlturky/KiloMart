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
        int FirstTimeInMinutesToMakeTheCircleBigger,
        decimal FirstCircleRaduis,
        int SecondTimeInMinutesToMakeTheCircleBigger,
        int MaxMinutesToCancelOrderWaitingADelivery,
        int MaxMinutesToCancelOrderWaitingAProvider,
        decimal SecondCircleRaduis,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[SystemSettings]
                                SET 
                                [DeliveryOrderFee] = @DeliveryOrderFee,
                                [ProviderOrderFee] = @ProviderOrderFee,
                                [CancelOrderWhenNoProviderHasAllProducts] = @CancelOrderWhenNoProviderHasAllProducts,
                                [SecondCircleRaduis] = @SecondCircleRaduis,
                                [SecondTimeInMinutesToMakeTheCircleBigger] = @SecondTimeInMinutesToMakeTheCircleBigger,
                                [FirstCircleRaduis] = @FirstCircleRaduis,
                                [FirstTimeInMinutesToMakeTheCircleBigger] = @FirstTimeInMinutesToMakeTheCircleBigger, 
                                [MaxMinutesToCancelOrderWaitingAProvider] = @MaxMinutesToCancelOrderWaitingAProvider, 
                                [MaxMinutesToCancelOrderWaitingADelivery] = @MaxMinutesToCancelOrderWaitingADelivery 
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            DeliveryOrderFee = deliveryOrderFee,
            ProviderOrderFee = providerOrderFee,
            CancelOrderWhenNoProviderHasAllProducts = cancelOrderWhenNoProviderHasAllProducts,
            FirstTimeInMinutesToMakeTheCircleBigger,
            FirstCircleRaduis,
            SecondTimeInMinutesToMakeTheCircleBigger,
            SecondCircleRaduis,
            MaxMinutesToCancelOrderWaitingADelivery,
            MaxMinutesToCancelOrderWaitingAProvider
        }, transaction);

        return updatedRowsCount > 0;
    }


    public static async Task<SystemSettings?> GetSystemSettingsByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [DeliveryOrderFee], 
                            [ProviderOrderFee], 
                            [CancelOrderWhenNoProviderHasAllProducts],
                            [SecondCircleRaduis],
                            [SecondTimeInMinutesToMakeTheCircleBigger],
                            [FirstCircleRaduis],
                            [FirstTimeInMinutesToMakeTheCircleBigger],
                            [MaxMinutesToCancelOrderWaitingADelivery],
                            [MaxMinutesToCancelOrderWaitingAProvider]                            
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
    public decimal SecondCircleRaduis { get; set; }
    public int SecondTimeInMinutesToMakeTheCircleBigger { get; set; }
    public decimal FirstCircleRaduis { get; set; }
    public int FirstTimeInMinutesToMakeTheCircleBigger { get; set; }
    public int MaxMinutesToCancelOrderWaitingAProvider { get; set; }
    public int MaxMinutesToCancelOrderWaitingADelivery { get; set; }
}