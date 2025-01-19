using Dapper;
using KiloMart.Core.Models;
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


    // Method to get the total count of active customers
    public static async Task<int> GetActiveCustomersCountFilteredAsync(IDbConnection connection, string? searchTerm = null)
    {
        const string countQuery = @"
        SELECT COUNT(DISTINCT m.Id) 
        FROM [dbo].[MembershipUser] m
            INNER JOIN dbo.[Customer] c ON m.Party = c.Party
            INNER JOIN dbo.[Party] p ON p.Id = c.Party
        WHERE (@SearchTerm IS NULL OR 
                 p.DisplayName LIKE '%' + @SearchTerm + '%' OR 
                 m.Email LIKE '%' + @SearchTerm + '%');
    ";

        return await connection.ExecuteScalarAsync<int>(countQuery, new { SearchTerm = searchTerm });
    }

    // Method to get paginated customer data
    public static async Task<IEnumerable<CustomerDataDto>> GetPaginatedCustomersDataFilteredAsync(
        IDbConnection connection, int page, int pageSize, string? searchTerm = null)
    {
        const string dataQuery = @"
        SELECT 
            m.Id,
            m.Email,
            m.EmailConfirmed,
            m.PasswordHash,
            m.Role,
            m.Party,
            m.IsActive,
            m.Language,
            m.IsDeleted,
            p.DisplayName,
            (SELECT SUM(o.TotalPrice) FROM dbo.[Order] o INNER JOIN OrderCustomerInformation oci ON oci.[Customer] = m.Party)  AS Balance

        FROM [dbo].[MembershipUser] m
            INNER JOIN dbo.[Customer] c ON m.Party = c.Party
            INNER JOIN dbo.[Party] p ON p.Id = c.Party
        WHERE (@SearchTerm IS NULL OR 
                 p.DisplayName LIKE '%' + @SearchTerm + '%' OR 
                 m.Email LIKE '%' + @SearchTerm + '%')
        ORDER BY m.Id
        OFFSET (@Page - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    ";

        return await connection.QueryAsync<CustomerDataDto>(dataQuery, new { Page = page, PageSize = pageSize, SearchTerm = searchTerm });
    }

    // Combined method to get paginated customers result with total count
    public static async Task<PaginatedResult<CustomerDataDto>> GetPaginatedCustomersFilteredAsync(IDbConnection connection, int page, int pageSize, string? searchTerm = null)
    {
        var totalCount = await GetActiveCustomersCountFilteredAsync(connection, searchTerm);
        var data = await GetPaginatedCustomersDataFilteredAsync(connection, page, pageSize, searchTerm);

        return new PaginatedResult<CustomerDataDto>
        {
            TotalCount = totalCount,
            Items = data.ToArray()
        };
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
public class CustomerDataDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public int Party { get; set; }
    public bool IsActive { get; set; }
    public string Language { get; set; }
    public bool IsDeleted { get; set; }
    public string DisplayName { get; set; }
    public decimal Balance { get; set; }
}
