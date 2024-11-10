using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.Models;
using KiloMart.Domain.Locations.Models;

namespace KiloMart.Domain.Locations.DataAccess;

public static class LocationRepository
{
    public static async Task InsertLocation(Location model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        INSERT INTO [dbo].[Location] ([Longitude], [Latitude], [Name], [Party], [IsActive])
            VALUES (@Longitude, @Latitude, @Name, @Party, @IsActive)";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task UpdateLocation(Location model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        UPDATE [dbo].[Location]
        SET [Longitude] = @Longitude, [Latitude] = @Latitude, [Name] = @Name, [Party] = @Party, [IsActive] = @IsActive
        WHERE [Id] = @Id";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task<List<Location>> GetLocationsByParty(int partyId, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[Location] WHERE [Party] = @Party";
        var result = await connection.QueryAsync<Location>(query, new { Party = partyId });
        return result.ToList();
    }

    public static async Task<Location?> GetLocationById(int id, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[Location] WHERE [Id] = @Id";
        var result = await connection.QueryFirstOrDefaultAsync<Location>(query, new { Id = id });
        return result;
    }
}
