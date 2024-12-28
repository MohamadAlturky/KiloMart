using System.Data;
using Dapper;
using KiloMart.Core.Models;

namespace KiloMart.Requests.Queries
{
    public partial class Query
    {

        #region ADMIN
        public static async Task<PaginatedResult<VehicleApiResponseList>> GetVehiclesPaginated(
                IDbConnection connection,
                int pageNumber,
                int pageSize)
        {
            var query = @"
            SELECT v.[Id] VehicleId,
                   v.[Number] VehicleNumber,
                   v.[Model] VehicleModel,
                   v.[Type] VehicleType,
                   v.[Year] VehicleYear,
                   p.[DisplayName] PartyDisplayName,
                   p.[Id] PartyId,
                   p.[IsActive] PartyIsActive
            FROM [dbo].[Vehicle] v
            INNER JOIN [dbo].[Party] p ON v.Delivary = p.Id
            ORDER BY v.[Id]
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;


            SELECT COUNT(*) 
            FROM [dbo].[Vehicle] v";

            using var multi = await connection.QueryMultipleAsync(
                query,
                new
                {
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize
                });

            var vehicles = (await multi.ReadAsync<VehicleApiResponseList>()).ToArray();
            var totalCount = await multi.ReadFirstAsync<int>();

            return new PaginatedResult<VehicleApiResponseList>
            {
                Items = vehicles,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }




        public class VehicleApiResponseList
        {
            public int VehicleId { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleModel { get; set; }
            public string VehicleType { get; set; }
            public string VehicleYear { get; set; }
            public string PartyDisplayName { get; set; }
            public int PartyId { get; set; }
            public bool PartyIsActive { get; set; }
        }
        #endregion
        public static async Task<VehicleApiResponse[]> GetVehiclesByDelivery(
            IDbConnection connection,
            int deliveryId)
        {
            var query = @"
            SELECT [Id]
                ,[Number]
                ,[Model]
                ,[Type]
                ,[Year]
            FROM [dbo].[Vehicle]
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

}