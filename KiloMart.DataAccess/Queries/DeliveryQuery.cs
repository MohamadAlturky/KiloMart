using Dapper;
using System.Data;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<(DeliveryProfileWithUserInfo[] Profiles, int TotalCount)> GetDeliveryProfilesWithUserInfoPaginated(
        IDbConnection connection,
        int page = 1,
        int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;

        // Query to fetch the paginated data for DeliveryProfiles
        var dataQuery = @"
            SELECT 
                p.[Id] AS DeliveryId,
                u.[Id] AS UserId,
                p.[DisplayName],
                cp.[FirstName],
                cp.[SecondName],
                cp.[NationalId],
                cp.[NationalName],
                cp.[DrivingLicenseExpiredDate],
                cp.DrivingLicenseNumber,
                cp.LicenseExpiredDate,
                cp.LicenseNumber,
                u.[Email],
                u.[EmailConfirmed],
                u.[IsActive]
            FROM [DelivaryProfile] cp
            INNER JOIN [Party] p
                ON cp.[Delivary] = p.[Id]
            INNER JOIN [MembershipUser] u
                ON u.[Party] = p.[Id]
            WHERE p.[IsActive] = 1
            ORDER BY DeliveryId
            OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        // Query to fetch the total count of records
        var countQuery = @"
            SELECT COUNT(*)
            FROM [DelivaryProfile] cp
            INNER JOIN [Party] p
                ON cp.[Delivary] = p.[Id]
            INNER JOIN [MembershipUser] u
                ON u.[Party] = p.[Id]
            WHERE p.[IsActive] = 1;";

        // Execute both queries
        var profiles = await connection.QueryAsync<DeliveryProfileWithUserInfo>(dataQuery, new { skip, pageSize });
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

        return (profiles.ToArray(), totalCount);
    }
}
public class DeliveryProfileWithUserInfo
{
    public int DeliveryId { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public DateTime? DrivingLicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateTime? LicenseExpiredDate { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
}
