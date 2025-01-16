using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Admin;
public static partial class Stats
{

    public static async Task<CustomerMembershipCountsDto> GetCustomerMembershipCountsAsync(IDbConnection connection)
    {
        const string activeCountQuery = @"
        SELECT COUNT(*) 
        FROM dbo.[Customer] c 
        INNER JOIN dbo.[MembershipUser] m ON m.[Party] = c.[Party] 
        WHERE IsActive = 1;";

        const string inactiveCountQuery = @"
        SELECT COUNT(*) 
        FROM dbo.[Customer] c 
        INNER JOIN dbo.[MembershipUser] m ON m.[Party] = c.[Party] 
        WHERE IsActive = 0;";

        var activeCountTask = connection.ExecuteScalarAsync<int>(activeCountQuery);
        var inactiveCountTask = connection.ExecuteScalarAsync<int>(inactiveCountQuery);

        await Task.WhenAll(activeCountTask, inactiveCountTask);

        return new CustomerMembershipCountsDto
        {
            ActiveCount = activeCountTask.Result,
            InactiveCount = inactiveCountTask.Result
        };
    }

    public static async Task<CustomerOrderStatisticsDto?> GetCustomerOrderStatisticsAsync(IDbConnection connection, int customerId)
    {
        const string query = @"
        SELECT 
		CustomerParty.Party,
        (SELECT COUNT(*) FROM dbo.OrderCustomerInformation o where o.Customer = CustomerParty.Party) AS OrdersCount,
		(SELECT SUM(o.TotalPrice) FROM OrderCustomerInformation oci INNER JOIN dbo.[Order] o ON oci.[Order] = o.Id
            WHERE o.OrderStatus = 5 AND oci.Customer = CustomerParty.Party) OrdersValue
        FROM dbo.[Customer] CustomerParty 
        LEFT JOIN dbo.OrderCustomerInformation o ON o.Customer = CustomerParty.Party
        WHERE CustomerParty.Party = @Customer
        GROUP BY 
            CustomerParty.Party;
";

        var result = await connection.QuerySingleOrDefaultAsync<CustomerOrderStatisticsDto>(query, new { Customer = customerId });

        return result;
    }

public static async Task<List<LocationDto>> GetLocationsByPartyAsync(IDbConnection connection, int partyId)
{
    const string query = @"
        SELECT 
            l.[Id] AS LocationId,
            l.[Longitude] AS LocationLongitude,
            l.[Latitude] AS LocationLatitude,
            l.[Name] AS LocationName,
            l.[Party] AS LocationParty,
            l.[IsActive] AS LocationIsActive,
            ld.[Id] AS LocationDetailsId,
            ld.[BuildingType] AS LocationDetailsBuildingType,
            ld.[BuildingNumber] AS LocationDetailsBuildingNumber,
            ld.[FloorNumber] AS LocationDetailsFloorNumber,
            ld.[ApartmentNumber] AS LocationDetailsApartmentNumber,
            ld.[StreetNumber] AS LocationDetailsStreetNumber,
            ld.[PhoneNumber] AS LocationDetailsPhoneNumber
        FROM dbo.[Location] l 
        LEFT JOIN dbo.[LocationDetails] ld ON ld.[Location] = l.[Id]
        WHERE l.Party = @party
        ORDER BY l.[Id] DESC;";

    var locations = await connection.QueryAsync<LocationDto>(query, new { party = partyId });

    return locations.ToList();
}

}
public class CustomerMembershipCountsDto
{
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
}

public class CustomerOrderStatisticsDto
{
    public int Party { get; set; }
    public int OrdersCount { get; set; }
    public decimal OrdersValue { get; set; }
}


public class LocationDto
{
    public int LocationId { get; set; }
    public decimal? LocationLongitude { get; set; } // Use nullable for potential null values
    public decimal? LocationLatitude { get; set; } // Use nullable for potential null values
    public string LocationName { get; set; }
    public int LocationParty { get; set; }
    public bool LocationIsActive { get; set; }
    
    public int? LocationDetailsId { get; set; }
    public string? LocationDetailsBuildingType { get; set; }
    public string? LocationDetailsBuildingNumber { get; set; }
    public string? LocationDetailsFloorNumber { get; set; } // Use nullable for potential null values
    public string? LocationDetailsApartmentNumber { get; set; }
    public string? LocationDetailsStreetNumber { get; set; }
    public string? LocationDetailsPhoneNumber { get; set; }
}
