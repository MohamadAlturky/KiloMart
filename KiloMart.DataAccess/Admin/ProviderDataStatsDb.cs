using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin
{
    public static partial class Stats
    {
        // Method to get the total count of active providers
        public static async Task<int> GetActiveProvidersProfilesCountAsync(IDbConnection connection)
        {
            const string countQuery = @"
            SELECT COUNT(*) 
                FROM ProviderProfileHistory pp
                    INNER JOIN MembershipUser m ON m.Party = pp.ProviderId
                    INNER JOIN Party party ON party.Id = pp.ProviderId
            WHERE pp.IsActive = 1;
            ";
            return await connection.ExecuteScalarAsync<int>(countQuery);
        }

        // Method to get paginated provider data
        public static async Task<IEnumerable<ProviderDataDto>> GetPaginatedProvidersDataAsync(IDbConnection connection, int page, int pageSize)
        {
            const string dataQuery = @"
                WITH ProviderData AS (
                    SELECT 
                        pp.ProviderId,
                        party.DisplayName AS DisplayName,
                        m.Email,
                        pp.PhoneNumber AS PhoneNumber,
                        pp.IsActive,
                        COUNT(DISTINCT o.Id) AS TotalOrders,
                        COUNT(DISTINCT po.Id) AS TotalProducts,
                        SUM(pa.Value)/(COUNT(DISTINCT po.Id) * COUNT(DISTINCT o.Id)) AS ReceivedBalance,
                        SUM(paall.Value)/(COUNT(DISTINCT po.Id) * COUNT(DISTINCT o.Id)) AS WithdrawalBalance,
                        pp.Longitude AS Long,
                        pp.Latitude AS Lat,
                        pp.LocationName AS City,
                        pp.BuildingNumber,
                        pp.ApartmentNumber,
                        pp.FloorNumber,
                        pp.StreetNumber
                    FROM dbo.ProviderProfileHistory pp
                    INNER JOIN MembershipUser m ON m.Party = pp.ProviderId
                    INNER JOIN Party party ON party.Id = pp.ProviderId
                    LEFT JOIN dbo.OrderProviderInformation o ON o.Provider = pp.ProviderId
                    LEFT JOIN dbo.ProductOffer po ON po.Provider = pp.ProviderId AND po.IsActive = 1
                    LEFT JOIN dbo.ProviderActivity pa ON pa.Provider = pp.ProviderId AND pa.[Type] = 1
                    LEFT JOIN dbo.ProviderActivity paall ON pa.Provider = pp.ProviderId AND paall.[Type] = 2
                    WHERE pp.IsActive = 1
                    GROUP BY 
                        pp.ProviderId, 
                        party.DisplayName, 
                        m.Email, 
                        pp.PhoneNumber, 
                        pp.IsActive, 
                        pp.Longitude, 
                        pp.Latitude, 
                        pp.LocationName, 
                        pp.BuildingNumber, 
                        pp.ApartmentNumber, 
                        pp.FloorNumber, 
                        pp.StreetNumber
                )
                SELECT 
                    pd.*
                FROM ProviderData pd
                ORDER BY pd.ProviderId
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            return await connection.QueryAsync<ProviderDataDto>(dataQuery, new { Page = page, PageSize = pageSize });
        }

        // Combined method to get paginated providers result with total count
        public static async Task<PaginatedProvidersResult> GetPaginatedProvidersAsync(IDbConnection connection, int page, int pageSize)
        {
            var totalCount = await GetActiveProvidersProfilesCountAsync(connection);
            var data = await GetPaginatedProvidersDataAsync(connection, page, pageSize);

            return new PaginatedProvidersResult
            {
                TotalCount = totalCount,
                Data = data.ToList()
            };
        }


        public static async Task<IEnumerable<ProviderDataDto>> GetPaginatedProvidersDataAsync(
            IDbConnection connection,
            int page,
            int pageSize,
            string? searchTerm = null)
        {
            const string dataQuery = @"
        WITH ProviderData AS (
            SELECT 
                pp.ProviderId,
                party.DisplayName AS DisplayName,
                m.Email,
                pp.PhoneNumber AS PhoneNumber,
                pp.IsActive,
                COUNT(DISTINCT o.Id) AS TotalOrders,
                COUNT(DISTINCT po.Id) AS TotalProducts,
                SUM(pa.Value)/(COUNT(DISTINCT po.Id) * COUNT(DISTINCT o.Id)) AS ReceivedBalance,
                SUM(paall.Value)/(COUNT(DISTINCT po.Id) * COUNT(DISTINCT o.Id)) AS WithdrawalBalance,
                pp.Longitude AS Long,
                pp.Latitude AS Lat,
                pp.LocationName AS City,
                pp.BuildingNumber,
                pp.ApartmentNumber,
                pp.FloorNumber,
                pp.StreetNumber
            FROM dbo.ProviderProfileHistory pp
            INNER JOIN MembershipUser m ON m.Party = pp.ProviderId
            INNER JOIN Party party ON party.Id = pp.ProviderId
            LEFT JOIN dbo.OrderProviderInformation o ON o.Provider = pp.ProviderId
            LEFT JOIN dbo.ProductOffer po ON po.Provider = pp.ProviderId AND po.IsActive = 1
            LEFT JOIN dbo.ProviderActivity pa ON pa.Provider = pp.ProviderId AND pa.[Type] = 1
            LEFT JOIN dbo.ProviderActivity paall ON paall.Provider = pp.ProviderId AND paall.[Type] = 2
            WHERE pp.IsActive = 1
            AND (@SearchTerm IS NULL OR 
                 party.DisplayName LIKE '%' + @SearchTerm + '%' OR 
                 m.Email LIKE '%' + @SearchTerm + '%')
            GROUP BY 
                pp.ProviderId, 
                party.DisplayName, 
                m.Email, 
                pp.PhoneNumber, 
                pp.IsActive, 
                pp.Longitude, 
                pp.Latitude, 
                pp.LocationName, 
                pp.BuildingNumber, 
                pp.ApartmentNumber, 
                pp.FloorNumber, 
                pp.StreetNumber
        )
        SELECT 
            pd.*
        FROM ProviderData pd
        ORDER BY pd.ProviderId
        OFFSET (@Page - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";

            return await connection.QueryAsync<ProviderDataDto>(dataQuery, new { Page = page, PageSize = pageSize, SearchTerm = searchTerm });
        }
        public static async Task<int> GetActiveFilteredProvidersProfilesCountAsync(IDbConnection connection, string? searchTerm = null)
        {
            const string countQuery = @"
            SELECT COUNT(*) 
                FROM ProviderProfileHistory pp
                    INNER JOIN MembershipUser m ON m.Party = pp.ProviderId
                    INNER JOIN Party party ON party.Id = pp.ProviderId
            WHERE pp.IsActive = 1 AND (@SearchTerm IS NULL OR 
                 party.DisplayName LIKE '%' + @SearchTerm + '%' OR 
                 m.Email LIKE '%' + @SearchTerm + '%');";

            return await connection.ExecuteScalarAsync<int>(countQuery, new { SearchTerm = searchTerm });
        }
        public static async Task<ProviderStatisticsDto?> GetProviderStatisticsAsync(IDbConnection connection, int providerId)
        {
            const string query = @"
                    SELECT 
                        COUNT(DISTINCT o.Id) AS TotalOrders,
                        COUNT(DISTINCT po.Id) AS TotalProducts,
                        SUM(pa.Value) / NULLIF(COUNT(DISTINCT po.Id) * COUNT(DISTINCT o.Id), 0) AS ReceivedBalance,
                        SUM(paall.Value) / NULLIF(COUNT(DISTINCT po.Id) * COUNT(DISTINCT o.Id), 0) AS WithdrawalBalance
                    FROM dbo.[Provider] ProviderParty 
                    LEFT JOIN dbo.OrderProviderInformation o ON o.Provider = ProviderParty.Party
                    LEFT JOIN dbo.ProductOffer po ON po.Provider = ProviderParty.Party AND po.IsActive = 1
                    LEFT JOIN dbo.ProviderActivity pa ON pa.Provider = ProviderParty.Party AND pa.[Type] = 1
                    LEFT JOIN dbo.ProviderActivity paall ON paall.Provider = ProviderParty.Party AND paall.[Type] = 2
                    WHERE ProviderParty.Party = @Provider
                    GROUP BY 
                        ProviderParty.Party;";

            var result = await connection.QuerySingleOrDefaultAsync<ProviderStatisticsDto>(query, new { Provider = providerId });

            return result;
        }

        public class ProviderStatisticsDto
        {
            public int TotalOrders { get; set; }
            public int TotalProducts { get; set; }
            public decimal? ReceivedBalance { get; set; } // Use nullable to handle division by zero
            public decimal? WithdrawalBalance { get; set; } // Use nullable to handle division by zero
        }

    }
}

public class ProviderDataDto
{
    public int ProviderId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public decimal ReceivedBalance { get; set; }
    public decimal WithdrawalBalance { get; set; }
    public double Long { get; set; }
    public double Lat { get; set; }
    public string City { get; set; }
    public string BuildingNumber { get; set; }
    public string ApartmentNumber { get; set; }
    public string FloorNumber { get; set; }
    public string StreetNumber { get; set; }
}

public class PaginatedProvidersResult
{
    public int TotalCount { get; set; }
    public IEnumerable<ProviderDataDto> Data { get; set; }
}
