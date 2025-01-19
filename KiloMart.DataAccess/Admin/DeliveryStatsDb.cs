using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Admin;
public static partial class Stats
{
    public static async Task<(int ActiveDeliveryCount, decimal DeliveryType1Sum, decimal DeliveryType2Sum)> GetDeliveryStatisticsAsync(IDbConnection connection)
    {
        // Query to count active deliveries
        const string countQuery = @"
                SELECT COUNT(d.[Party]) 
                FROM Delivery d
                INNER JOIN MembershipUser m ON m.Party = d.Party AND m.IsActive = 1";

        // Query to sum values for DeliveryActivity Type 1
        const string sumType1Query = @"
                SELECT SUM([Value]) 
                FROM DeliveryActivity 
                WHERE [Type] = 1";

        // Query to sum values for DeliveryActivity Type 2
        const string sumType2Query = @"
                SELECT SUM([Value]) 
                FROM DeliveryActivity 
                WHERE [Type] = 2";

        // Execute the queries and retrieve results
        var activeDeliveryCount = await connection.ExecuteScalarAsync<int>(countQuery);
        var deliveryType1Sum = await connection.ExecuteScalarAsync<decimal?>(sumType1Query) ?? 0;
        var deliveryType2Sum = await connection.ExecuteScalarAsync<decimal?>(sumType2Query) ?? 0;

        return (activeDeliveryCount, deliveryType1Sum, deliveryType2Sum);
    }

    // Method to get the total count of active deliveries
    public static async Task<int> GetActiveDeliveriesProfilesCountAsync(IDbConnection connection)
    {
        const string countQuery = @"
        SELECT COUNT(*) 
        FROM DeliveryProfileHistory pp
            INNER JOIN MembershipUser m ON m.Party = pp.DeliveryId
            INNER JOIN Party party ON party.Id = pp.DeliveryId
        WHERE pp.IsActive = 1;
    ";

        return await connection.ExecuteScalarAsync<int>(countQuery);
    }

    // Method to get paginated delivery data
    public static async Task<IEnumerable<DeliveryDataDto>> GetPaginatedDeliveriesDataAsync(IDbConnection connection, int page, int pageSize)
    {
        const string dataQuery = @"
        SELECT 
            pp.[Id],
            pp.[FirstName],
            pp.[SecondName],
            pp.[NationalName],
            pp.[NationalId],
            pp.[LicenseNumber],
            pp.[LicenseExpiredDate],
            pp.[DrivingLicenseNumber],
            pp.[DrivingLicenseExpiredDate],
            pp.[VehicleNumber],
            pp.[VehicleModel],
            pp.[VehicleType],
            pp.[VehicleYear],
            pp.[VehiclePhotoFileUrl],
            pp.[DrivingLicenseFileUrl],
            pp.[VehicleLicenseFileUrl],
            pp.[NationalIqamaIDFileUrl],
            pp.[SubmitDate],
            pp.[ReviewDate],
            pp.[DeliveryId],
            pp.[IsActive],
            pp.[IsRejected],
            pp.[IsAccepted],
            pp.[ReviewDescription],
            m.Email,
            m.IsActive UserIsActive,
            party.DisplayName AS DisplayName,
            COUNT(DISTINCT o.Id) AS TotalOrders,
            SUM(pa.Value) AS ReceivedBalance,
            SUM(paall.Value) AS WithdrawalBalance
        FROM [dbo].[DeliveryProfileHistory] pp
        INNER JOIN MembershipUser m ON m.Party = pp.DeliveryId
        INNER JOIN Party party ON party.Id = pp.DeliveryId
        LEFT JOIN dbo.OrderProviderInformation o ON o.Provider = pp.DeliveryId
        LEFT JOIN dbo.DeliveryActivity pa ON pa.Delivery = pp.DeliveryId AND pa.Type = 1
        LEFT JOIN dbo.DeliveryActivity paall ON paall.Delivery = pp.DeliveryId AND paall.Type = 2
        WHERE pp.IsActive = 1
        GROUP BY 
            pp.Id,
            pp.FirstName,
            pp.SecondName,
            pp.NationalName,
            pp.NationalId,
            pp.LicenseNumber,
            pp.LicenseExpiredDate,
            pp.DrivingLicenseNumber,
            pp.DrivingLicenseExpiredDate,
            pp.VehicleNumber,
            pp.VehicleModel,
            pp.VehicleType,
            pp.VehicleYear,
            pp.VehiclePhotoFileUrl,
            pp.DrivingLicenseFileUrl,
            pp.VehicleLicenseFileUrl,
            pp.NationalIqamaIDFileUrl,
            pp.SubmitDate,
            pp.ReviewDate,
            pp.DeliveryId,
            pp.IsActive,
            pp.IsRejected,
            pp.IsAccepted,
            pp.ReviewDescription,
            m.Email,
            party.DisplayName
        ORDER BY 
           pp.Id
        OFFSET (@Page - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    ";

        return await connection.QueryAsync<DeliveryDataDto>(dataQuery, new { Page = page, PageSize = pageSize });
    }

    // Combined method to get paginated deliveries result with total count
    public static async Task<PaginatedDeliveriesResult> GetPaginatedDeliveriesAsync(IDbConnection connection, int page, int pageSize)
    {
        var totalCount = await GetActiveDeliveriesProfilesCountAsync(connection);
        var data = await GetPaginatedDeliveriesDataAsync(connection, page, pageSize);

        return new PaginatedDeliveriesResult
        {
            TotalCount = totalCount,
            Data = data.ToList()
        };
    }

    ///////////////














    // Method to get the total count of active deliveries
    public static async Task<int> GetActiveDeliveriesProfilesCountFilteredAsync(IDbConnection connection, string? searchTerm = null)
    {
        const string countQuery = @"
        SELECT COUNT(*) 
        FROM DeliveryProfileHistory pp
            INNER JOIN MembershipUser m ON m.Party = pp.DeliveryId
            INNER JOIN Party party ON party.Id = pp.DeliveryId
        WHERE pp.IsActive = 1 AND (@SearchTerm IS NULL OR 
                 party.DisplayName LIKE '%' + @SearchTerm + '%' OR 
                 m.Email LIKE '%' + @SearchTerm + '%');
    ";

        return await connection.ExecuteScalarAsync<int>(countQuery, new { SearchTerm = searchTerm });
    }

    // Method to get paginated delivery data
    public static async Task<IEnumerable<DeliveryDataDto>> GetPaginatedDeliveriesDataFilteredAsync
    (IDbConnection connection, int page, int pageSize, string? searchTerm = null)
    {
        const string dataQuery = @"
        SELECT 
            pp.[Id],
            pp.[FirstName],
            pp.[SecondName],
            pp.[NationalName],
            pp.[NationalId],
            pp.[LicenseNumber],
            pp.[LicenseExpiredDate],
            pp.[DrivingLicenseNumber],
            pp.[DrivingLicenseExpiredDate],
            pp.[VehicleNumber],
            pp.[VehicleModel],
            pp.[VehicleType],
            pp.[VehicleYear],
            pp.[VehiclePhotoFileUrl],
            pp.[DrivingLicenseFileUrl],
            pp.[VehicleLicenseFileUrl],
            pp.[NationalIqamaIDFileUrl],
            pp.[SubmitDate],
            pp.[ReviewDate],
            pp.[DeliveryId],
            pp.[IsActive],
            pp.[IsRejected],
            pp.[IsAccepted],
            pp.[ReviewDescription],
            m.Email,
            m.IsActive UserIsActive,
            party.DisplayName AS DisplayName,
            COUNT(DISTINCT o.Id) AS TotalOrders,
            SUM(pa.Value) AS ReceivedBalance,
            SUM(paall.Value) AS WithdrawalBalance
        FROM [dbo].[DeliveryProfileHistory] pp
        INNER JOIN MembershipUser m ON m.Party = pp.DeliveryId
        INNER JOIN Party party ON party.Id = pp.DeliveryId
        LEFT JOIN dbo.OrderProviderInformation o ON o.Provider = pp.DeliveryId
        LEFT JOIN dbo.DeliveryActivity pa ON pa.Delivery = pp.DeliveryId AND pa.Type = 1
        LEFT JOIN dbo.DeliveryActivity paall ON paall.Delivery = pp.DeliveryId AND paall.Type = 2
        WHERE pp.IsActive = 1 AND (@SearchTerm IS NULL OR 
                 party.DisplayName LIKE '%' + @SearchTerm + '%' OR 
                 m.Email LIKE '%' + @SearchTerm + '%')
        GROUP BY 
            pp.Id,
            pp.FirstName,
            pp.SecondName,
            pp.NationalName,
            pp.NationalId,
            pp.LicenseNumber,
            pp.LicenseExpiredDate,
            pp.DrivingLicenseNumber,
            pp.DrivingLicenseExpiredDate,
            pp.VehicleNumber,
            pp.VehicleModel,
            pp.VehicleType,
            pp.VehicleYear,
            pp.VehiclePhotoFileUrl,
            pp.DrivingLicenseFileUrl,
            pp.VehicleLicenseFileUrl,
            pp.NationalIqamaIDFileUrl,
            pp.SubmitDate,
            pp.ReviewDate,
            pp.DeliveryId,
            pp.IsActive,
            pp.IsRejected,
            pp.IsAccepted,
            pp.ReviewDescription,
            m.Email,
            party.DisplayName
        ORDER BY 
           pp.Id
        OFFSET (@Page - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    ";

        return await connection.QueryAsync<DeliveryDataDto>(dataQuery, new { Page = page, PageSize = pageSize, SearchTerm = searchTerm });
    }

    // Combined method to get paginated deliveries result with total count
    public static async Task<PaginatedDeliveriesResult> GetPaginatedDeliveriesFilteredAsync(IDbConnection connection, int page, int pageSize, string? searchTerm = null)
    {
        var totalCount = await GetActiveDeliveriesProfilesCountFilteredAsync(connection, searchTerm);
        var data = await GetPaginatedDeliveriesDataFilteredAsync(connection, page, pageSize, searchTerm);

        return new PaginatedDeliveriesResult
        {
            TotalCount = totalCount,
            Data = data.ToList()
        };
    }

    public static async Task<DeliveryStatisticsDto?> GetDeliveryStatisticsAsync(IDbConnection connection, int deliveryId)
    {
        const string query = @"
        SELECT 
            COUNT(DISTINCT o.Id) AS TotalOrders,
            SUM(pa.Value) AS ReceivedBalance,
            SUM(paall.Value) AS WithdrawalBalance
        FROM dbo.[Delivery] DeliveryParty 
        LEFT JOIN dbo.OrderDeliveryInformation o ON o.Delivery = DeliveryParty.Party
        LEFT JOIN dbo.DeliveryActivity pa ON pa.Delivery = DeliveryParty.Party AND pa.[Type] = 1
        LEFT JOIN dbo.DeliveryActivity paall ON paall.Delivery = DeliveryParty.Party AND paall.[Type] = 2
        WHERE DeliveryParty.Party = @Delivery
        GROUP BY 
            DeliveryParty.Party;";

        var result = await connection.QuerySingleOrDefaultAsync<DeliveryStatisticsDto>(query, new { Delivery = deliveryId });

        return result;
    }

}

public class DeliveryStatisticsDto
{
    public int TotalOrders { get; set; }
    public decimal? ReceivedBalance { get; set; } // Nullable to handle potential null results
    public decimal? WithdrawalBalance { get; set; } // Nullable to handle potential null results
}

public class DeliveryDataDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime DrivingLicenseExpiredDate { get; set; }
    public string VehicleNumber { get; set; }
    public string VehicleModel { get; set; }
    public string VehicleType { get; set; }
    public string VehicleYear { get; set; }
    public string VehiclePhotoFileUrl { get; set; }
    public string DrivingLicenseFileUrl { get; set; }
    public string VehicleLicenseFileUrl { get; set; }
    public string NationalIqamaIDFileUrl { get; set; }
    public DateTime SubmitDate { get; set; }
    public DateTime ReviewDate { get; set; }
    public int DeliveryId { get; set; }
    public bool IsActive { get; set; }
    public bool UserIsActive { get; set; }
    public bool IsRejected { get; set; }
    public bool IsAccepted { get; set; }
    public string ReviewDescription { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public long TotalOrders { get; set; }
    public decimal ReceivedBalance { get; set; }
    public decimal WithdrawalBalance { get; set; }
}


public class PaginatedDeliveriesResult
{
    public int TotalCount { get; set; }
    public IEnumerable<DeliveryDataDto> Data { get; set; }
}


