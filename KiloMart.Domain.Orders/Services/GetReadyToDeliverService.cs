using System.Data;
using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Orders.Common;

namespace KiloMart.Domain.Orders.Services
{
    public static class GetReadyToDeliverService
    {
        public static async Task<Result<List<ReadyToDeliverOrder>>> List(
            decimal latitude,
            decimal longitude,
            int timeToMakeTheRaduisBigger,
            decimal distanceToAdd,
            decimal maxDistanceToAdd,
            decimal raduis,
            IDbFactory dbFactory)
        {


            const string sql = @"
            SELECT * FROM (
                SELECT 
                    o.Id,
                    o.OrderStatus,
                    o.TotalPrice,
                    o.TransactionId,
                    o.Date,
                    o.SpecialRequest,
                    oci.Customer,
                    oci.Location AS CustomerLocation,
                    oci.Id AS CustomerInformationId,
                    opi.Provider,
                    opi.Location AS ProviderLocation,
                    opi.Id AS ProviderInformationId,
                    odi.Delivery,
                    odi.Id AS DeliveryInformationId,

                    cl.[Name] AS CustomerLocationName,
                    cl.[Latitude] AS CustomerLocationLatitude,
                    cl.[Longitude] AS CustomerLocationLongitude,
                    pl.[Name] AS ProviderLocationName,
                    pl.[Latitude] AS ProviderLocationLatitude,
                    pl.[Longitude] AS ProviderLocationLongitude,

                    -- Haversine formula for Customer Location distance
                    6371 * 2 * ATN2(
                        SQRT(
                            POWER(SIN(RADIANS(cl.[Latitude] - @Latitude) / 2), 2) + 
                            COS(RADIANS(@Latitude)) * COS(RADIANS(cl.[Latitude])) *
                            POWER(SIN(RADIANS(cl.[Longitude] - @Longitude) / 2), 2)
                        ),
                        SQRT(1 - (
                            POWER(SIN(RADIANS(cl.[Latitude] - @Latitude) / 2), 2) + 
                            COS(RADIANS(@Latitude)) * COS(RADIANS(cl.[Latitude])) *
                            POWER(SIN(RADIANS(cl.[Longitude] - @Longitude) / 2), 2)
                        ))
                    ) AS CustomerDistanceInKilometers,

                    -- Haversine formula for Provider Location distance
                    6371 * 2 * ATN2(
                        SQRT(
                            POWER(SIN(RADIANS(pl.[Latitude] - @Latitude) / 2), 2) + 
                            COS(RADIANS(@Latitude)) * COS(RADIANS(pl.[Latitude])) *
                            POWER(SIN(RADIANS(pl.[Longitude] - @Longitude) / 2), 2)
                        ),
                        SQRT(1 - (
                            POWER(SIN(RADIANS(pl.[Latitude] - @Latitude) / 2), 2) + 
                            COS(RADIANS(@Latitude)) * COS(RADIANS(pl.[Latitude])) *
                            POWER(SIN(RADIANS(pl.[Longitude] - @Longitude) / 2), 2)
                        ))
                    ) AS ProviderDistanceInKilometers,
                    6371 *
                    ACOS(COS(RADIANS(@Latitude)) * COS(RADIANS(cl.[Latitude])) * COS(RADIANS(cl.[Longitude]) - RADIANS(@Longitude)) 
                    + SIN(RADIANS(@Latitude)) * SIN(RADIANS(cl.[Latitude]))) AS DistanceInKm,
                    oa.[Date] AS DateWhenProviderAcceptIt,
                    GETDATE() AS NOW,

                    -- Time differences
                    DATEDIFF(MINUTE, o.[Date], GETDATE()) AS DifferenceInMinutes
                FROM 
                    dbo.[Order] o
                LEFT JOIN 
                    dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
                LEFT JOIN 
                    dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
                LEFT JOIN 
                    dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
                LEFT JOIN 
                    dbo.[Location] cl ON cl.Id = oci.[Location]
                LEFT JOIN 
                    dbo.[Location] pl ON pl.Id = opi.[Location]
                INNER JOIN dbo.[OrderActivity] oa ON oa.[Order] = o.Id AND oa.OrderActivityType = 5
            ) p
            WHERE OrderStatus = @status AND ProviderDistanceInKilometers <= @Raduis +
            CASE
                WHEN FLOOR(p.DifferenceInMinutes / @TimeToMakeTheRaduisBigger) * @DistanceToAdd < @MaxDistanceToAdd
                THEN FLOOR(p.DifferenceInMinutes / @TimeToMakeTheRaduisBigger) * @DistanceToAdd
                ELSE @MaxDistanceToAdd
            END";

            using IDbConnection connection = dbFactory.CreateDbConnection();
            connection.Open();

            var parameters = new 
            { 
                Latitude = latitude,
                Longitude = longitude,
                TimeToMakeTheRaduisBigger = timeToMakeTheRaduisBigger,
                DistanceToAdd = distanceToAdd,
                MaxDistanceToAdd = maxDistanceToAdd,
                Raduis = raduis,
                status = OrderStatus.PREPARING
            };

            IEnumerable<ReadyToDeliverOrder> orders = await connection.QueryAsync<ReadyToDeliverOrder>(sql, parameters);

            return Result<List<ReadyToDeliverOrder>>.Ok(orders.ToList());
        }
    }

public class ReadyToDeliverOrder
{
    public int Id { get; set; }
    public byte OrderStatus { get; set; }
    public string? SpecialRequest { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; }
    public DateTime Date { get; set; }
    public string Customer { get; set; }
    public int CustomerInformationId { get; set; }
    public string CustomerLocation { get; set; }
    public string Provider { get; set; }
    public int ProviderInformationId { get; set; }
    public string ProviderLocation { get; set; }
    public string Delivery { get; set; }
    public int DeliveryInformationId { get; set; }
    public string CustomerLocationName { get; set; }
    public decimal CustomerLocationLatitude { get; set; }
    public decimal CustomerLocationLongitude { get; set; }
    public string ProviderLocationName { get; set; }
    public decimal ProviderLocationLatitude { get; set; }
    public decimal ProviderLocationLongitude { get; set; }
    public decimal CustomerDistanceInKilometers { get; set; }
    public decimal ProviderDistanceInKilometers { get; set; }
    public decimal DistanceInKm { get; set; }
    public DateTime DateWhenProviderAcceptIt { get; set; }
    public DateTime Now { get; set; }
    public int DifferenceInMinutes { get; set; }
}

}