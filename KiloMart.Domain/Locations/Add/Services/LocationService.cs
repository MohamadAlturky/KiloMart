using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Locations.Add.Models;

namespace KiloMart.Domain.Locations.Add.Services;

public static partial class LocationService
{
    public static async Task<Result<CreateLocationResponse>> Insert(IDbFactory dbFactory, CreateLocationRequest location, int party)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            const string sql = @"
                INSERT INTO Location (Longitude, Latitude, Name, Party, IsActive)
                VALUES (@Longitude, @Latitude, @Name, @Party, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            CreateLocationResponse response = new();
            response.Id = await connection.QuerySingleAsync<int>(sql, new
            {
                location.Longitude,
                location.Latitude,
                location.Name,
                party,
                IsActive = true
            });

            response.Longitude = location.Longitude;
            response.Latitude = location.Latitude;
            response.Name = location.Name;
            response.Party = party;

            return Result<CreateLocationResponse>.Ok(response);
        }
        catch (Exception e)
        {
            return Result<CreateLocationResponse>.Fail([e.Message]);
        }
    }

   
   
}
