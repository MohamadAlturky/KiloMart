using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<List<Location>> GetLocationsByParty(IDbConnection connection, int party)
    {
        var query = "SELECT * FROM [dbo].[Location] WHERE [Party] = @Party";
        var result = await connection.QueryAsync<Location>(query, new { Party = party });
        return result.ToList();
    }
}
public class Location
{
    public int Id { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}



public partial class Query
{
    public static async Task<(LocationWithPartyInfo[] Locations, int TotalCount)> GetLocationsWithPartyInfoPaginated(
        IDbConnection connection,
        int page = 1,
        int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;

        // Query to fetch the paginated location data with party information
        var dataQuery = @"
        SELECT 
            l.Id AS LocationId,
            l.IsActive AS LocationIsActive,
            l.Latitude AS LocationLatitude,
            l.Longitude AS LocationLongitude,
            l.Name AS LocationName,
            p.Id AS PartyId,
            p.DisplayName AS PartyDisplayName
        FROM dbo.[Location] l
        INNER JOIN Party p ON l.Party = p.Id
        ORDER BY LocationId
        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        // Query to fetch the total count of records
        var countQuery = @"
        SELECT COUNT(*)
        FROM dbo.[Location] l
        INNER JOIN Party p ON l.Party = p.Id;";

        // Execute both queries
        var locations = await connection.QueryAsync<LocationWithPartyInfo>(dataQuery, new { skip, pageSize });
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

        return (locations.ToArray(), totalCount);
    }
    public static async Task<(LocationWithPartyInfo[] Locations, int TotalCount)> GetLocationsWithPartyInfoPaginatedFilteredWithIsActive(
        IDbConnection connection,
        int page = 1,
        int pageSize = 10,
        bool isActive = true)
    {
        int skip = (page - 1) * pageSize;

        // Query to fetch the paginated location data with party information
        var dataQuery = @"
        SELECT 
            l.Id AS LocationId,
            l.IsActive AS LocationIsActive,
            l.Latitude AS LocationLatitude,
            l.Longitude AS LocationLongitude,
            l.Name AS LocationName,
            p.Id AS PartyId,
            p.DisplayName AS PartyDisplayName
        FROM dbo.[Location] l
        INNER JOIN Party p ON l.Party = p.Id
        WHERE l.IsActive = @IsActive
        ORDER BY LocationId
        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        // Query to fetch the total count of records
        var countQuery = @"
        SELECT COUNT(*)
        FROM dbo.[Location] l
        INNER JOIN Party p ON l.Party = p.Id
        WHERE l.IsActive = @IsActive;";

        // Execute both queries
        var locations = await connection.QueryAsync<LocationWithPartyInfo>(dataQuery, new { skip, pageSize, IsActive = isActive });
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { IsActive = isActive });

        return (locations.ToArray(), totalCount);
    }
}


public class LocationWithPartyInfo
{
    public int LocationId { get; set; }
    public bool LocationIsActive { get; set; }
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
    public string LocationName { get; set; } = null!;
    public int PartyId { get; set; }
    public string PartyDisplayName { get; set; } = null!;
}
