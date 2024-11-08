using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.Locations.Details.Models;

namespace KiloMart.Domain.Locations.Details.Services;

public static partial class LocationDetailsService
{
    public static async Task<Result<CreateLocationDetailsResponse>> InsertLocationDetails(IDbFactory dbFactory, CreateLocationDetailsRequest details)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            const string sql = @"
                    INSERT INTO LocationDetails (BuildingType, BuildingNumber, FloorNumber, ApartmentNumber, StreetNumber, PhoneNumber, Location)
                    VALUES (@BuildingType, @BuildingNumber, @FloorNumber, @ApartmentNumber, @StreetNumber, @PhoneNumber, @Location);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await connection.QuerySingleAsync<int>(sql, new
            {
                details.BuildingType,
                details.BuildingNumber,
                details.FloorNumber,
                details.ApartmentNumber,
                details.StreetNumber,
                details.PhoneNumber,
                details.Location
            });

            return Result<CreateLocationDetailsResponse>.Ok(new CreateLocationDetailsResponse
            {
                Id = id,
                BuildingType = details.BuildingType,
                BuildingNumber = details.BuildingNumber,
                FloorNumber = details.FloorNumber,
                ApartmentNumber = details.ApartmentNumber,
                StreetNumber = details.StreetNumber,
                PhoneNumber = details.PhoneNumber,
                Location = details.Location
            });
        }
        catch (Exception e)
        {
            return Result<CreateLocationDetailsResponse>.Fail([e.Message]);
        }
    }
}