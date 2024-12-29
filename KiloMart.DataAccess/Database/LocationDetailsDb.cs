using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    /// <summary>
    /// Table Specification
    //  CREATE TABLE [dbo].[LocationDetails](
    // 	[Id] [int] IDENTITY(1,1) NOT NULL,
    // 	[BuildingType] [varchar](50) NOT NULL,
    // 	[BuildingNumber] [varchar](50) NOT NULL,
    // 	[FloorNumber] [varchar](50) NOT NULL,
    // 	[ApartmentNumber] [varchar](50) NOT NULL,
    // 	[StreetNumber] [varchar](50) NOT NULL,
    // 	[PhoneNumber] [varchar](50) NOT NULL,
    // 	[Location] [int] NOT NULL)
    /// </summary>
    /// 
    public static partial class Db
    {
        public static async Task<int> InsertLocationDetailsAsync(IDbConnection connection,
            string buildingType,
            string buildingNumber,
            string floorNumber,
            string apartmentNumber,
            string streetNumber,
            string phoneNumber,
            int location,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[LocationDetails]
                            ([BuildingType], [BuildingNumber], [FloorNumber], [ApartmentNumber], [StreetNumber], [PhoneNumber], [Location])
                            VALUES (@BuildingType, @BuildingNumber, @FloorNumber, @ApartmentNumber, @StreetNumber, @PhoneNumber, @Location)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                BuildingType = buildingType,
                BuildingNumber = buildingNumber,
                FloorNumber = floorNumber,
                ApartmentNumber = apartmentNumber,
                StreetNumber = streetNumber,
                PhoneNumber = phoneNumber,
                Location = location
            }, transaction);
        }

        public static async Task<bool> UpdateLocationDetailsAsync(IDbConnection connection,
            int id,
            string buildingType,
            string buildingNumber,
            string floorNumber,
            string apartmentNumber,
            string streetNumber,
            string phoneNumber,
            int location,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[LocationDetails]
                                SET 
                                [BuildingType] = @BuildingType,
                                [BuildingNumber] = @BuildingNumber,
                                [FloorNumber] = @FloorNumber,
                                [ApartmentNumber] = @ApartmentNumber,
                                [StreetNumber] = @StreetNumber,
                                [PhoneNumber] = @PhoneNumber,
                                [Location] = @Location
                                WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                BuildingType = buildingType,
                BuildingNumber = buildingNumber,
                FloorNumber = floorNumber,
                ApartmentNumber = apartmentNumber,
                StreetNumber = streetNumber,
                PhoneNumber = phoneNumber,
                Location = location
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteLocationDetailsAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[LocationDetails]
                                WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<LocationDetails?> GetLocationDetailsByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                            [Id], 
                            [BuildingType], 
                            [BuildingNumber], 
                            [FloorNumber], 
                            [ApartmentNumber], 
                            [StreetNumber], 
                            [PhoneNumber], 
                            [Location]
                            FROM [dbo].[LocationDetails]
                            WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<LocationDetails>(query, new
            {
                Id = id
            });
        }

        public static async Task<LocationDetails?> GetLocationDetailsByLocationIdAsync(int locationId, IDbConnection connection)
        {
            const string query = @"SELECT 
                            [Id], 
                            [BuildingType], 
                            [BuildingNumber], 
                            [FloorNumber], 
                            [ApartmentNumber], 
                            [StreetNumber], 
                            [PhoneNumber], 
                            [Location]
                            FROM [dbo].[LocationDetails]
                            WHERE [Location] = @locationId";

            return await connection.QueryFirstOrDefaultAsync<LocationDetails>(query, new
            {
                locationId = locationId
            });
        }
    }

    public class LocationDetails
    {
        public int Id { get; set; }
        public string BuildingType { get; set; } = null!;
        public string BuildingNumber { get; set; } = null!;
        public string FloorNumber { get; set; } = null!;
        public string ApartmentNumber { get; set; } = null!;
        public string StreetNumber { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int Location { get; set; }
    }
}
