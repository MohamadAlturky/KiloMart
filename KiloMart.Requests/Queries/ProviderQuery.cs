using Dapper;
using System.Data;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<(ProviderProfileWithUserInfo[] Profiles, int TotalCount)> GetProviderProfilesWithUserInfoPaginated(
        IDbConnection connection,
        int page = 1,
        int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;

        // Query to fetch the paginated data
        var dataQuery = @"
        SELECT 
            p.[Id] AS ProviderId,
            u.[Id] AS UserId,
            p.[DisplayName],
            cp.[FirstName],
            cp.[SecondName],
            cp.[CompanyName],
            cp.[NationalApprovalId],
            cp.[OwnerName],
            cp.[OwnerNationalId],
            u.[Email],
            u.[EmailConfirmed],
            u.[IsActive]
        FROM [ProviderProfile] cp
        INNER JOIN [Party] p
            ON cp.[Provider] = p.[Id]
        INNER JOIN [MembershipUser] u
            ON u.[Party] = p.[Id]
        WHERE p.[IsActive] = 1
        ORDER BY ProviderId
        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        // Query to fetch the total count of records
        var countQuery = @"
        SELECT COUNT(*)
        FROM [ProviderProfile] cp
        INNER JOIN [Party] p
            ON cp.[Provider] = p.[Id]
        INNER JOIN [MembershipUser] u
            ON u.[Party] = p.[Id]
        WHERE p.[IsActive] = 1;";

        // Execute both queries
        var profiles = await connection.QueryAsync<ProviderProfileWithUserInfo>(dataQuery, new { skip, pageSize });
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

        return (profiles.ToArray(), totalCount);
    }
}
public class ProviderProfileWithUserInfo
{
    public int ProviderId { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string NationalApprovalId { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string OwnerNationalId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
}
