using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Database;

public static class ProviderCircleDb
{
    public static async Task<IEnumerable<ProviderCircleIds>> GetProviderCirclesAsync(IDbConnection connection,
    SystemSettings settings,
    decimal latitude,
    decimal longitude,
    IDbTransaction? transaction = null)
    {
        decimal maxDistanceToAdd = settings.MaxDistanceToAdd;
        decimal raduis = settings.CircleRaduis;
        var sql = @"
            SELECT p.Party AS Id 
                FROM [Provider] p 
                INNER JOIN [Location] l 
                    ON l.Party = p.Party 
                    AND l.IsActive = 1
                WHERE dbo.GetDistanceBetweenPoints(l.[Latitude], l.[Longitude], @Latitude, @Longitude) <= @Radius + @MaxDistanceToAdd
                ORDER BY p.Party;
        ";
        return await connection.QueryAsync<ProviderCircleIds>(sql, new { Latitude = latitude, Longitude = longitude, Radius = raduis, MaxDistanceToAdd = maxDistanceToAdd }, transaction: transaction);
    }

    public static async Task<IEnumerable<ProviderCircleIds>> GetDeliveryProviderCirclesAsync(IDbConnection connection,
    IDbTransaction? transaction = null)
    {
        var sql = @"
            SELECT p.Party AS Id 
                FROM [Delivery] p;";
        return await connection.QueryAsync<ProviderCircleIds>(sql, transaction: transaction);
    }
}

public class ProviderCircleIds
{
    public int Id { get; set; }
}

