using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace KiloMart.Requests.Queries
{
    public partial class Query
    {
        public static async Task<VehicleApiResponse[]> GetVehiclesByDelivery(
            IDbConnection connection, 
            int deliveryId)
        {
            var query = @"
            SELECT TOP (1000) [Id]
                ,[Number]
                ,[Model]
                ,[Type]
                ,[Year]
            FROM [KiloMartMasterDb].[dbo].[Vehicle]
            WHERE [Delivary] = @Delivary";

            var vehicles = await connection.QueryAsync<VehicleApiResponse>(
                query,
                new { Delivary = deliveryId });
            
            return vehicles.ToArray();
        }
    }
    public class VehicleApiResponse
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }


}
