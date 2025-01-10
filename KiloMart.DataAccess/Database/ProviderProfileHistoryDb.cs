using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public partial class Db
{
    public static async Task<long> InsertProviderProfileHistoryAsync(IDbConnection connection,
        string firstName,
        string secondName,
        string nationalApprovalId,
        string companyName,
        string ownerName,
        string ownerNationalId,
        string ownershipDocumentFileUrl,
        string ownerNationalApprovalFileUrl,
        string locationName,
        decimal longitude,
        decimal latitude,
        string buildingType,
        string buildingNumber,
        string floorNumber,
        string apartmentNumber,
        string streetNumber,
        string phoneNumber,
        bool isAccepted,
        bool isRejected,
        DateTime submitDate,
        DateTime? reviewDate,
        int providerId,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProviderProfileHistory]
                                ([FirstName], [SecondName], [NationalApprovalId], [CompanyName], [OwnerName],
                                 [OwnerNationalId], [OwnershipDocumentFileUrl], [OwnerNationalApprovalFileUrl],
                                 [LocationName], [Longitude], [Latitude], [BuildingType], [BuildingNumber],
                                 [FloorNumber], [ApartmentNumber], [StreetNumber], [PhoneNumber],
                                 [IsAccepted], [IsRejected], [SubmitDate], [ReviewDate], [ProviderId], [IsActive])
                                VALUES (@FirstName, @SecondName, @NationalApprovalId, @CompanyName, @OwnerName,
                                        @OwnerNationalId, @OwnershipDocumentFileUrl, @OwnerNationalApprovalFileUrl,
                                        @LocationName, @Longitude, @Latitude, @BuildingType, @BuildingNumber,
                                        @FloorNumber, @ApartmentNumber, @StreetNumber, @PhoneNumber,
                                        @IsAccepted, @IsRejected, @SubmitDate, @ReviewDate, @ProviderId, @IsActive)
                                SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            FirstName = firstName,
            SecondName = secondName,
            NationalApprovalId = nationalApprovalId,
            CompanyName = companyName,
            OwnerName = ownerName,
            OwnerNationalId = ownerNationalId,
            OwnershipDocumentFileUrl = ownershipDocumentFileUrl,
            OwnerNationalApprovalFileUrl = ownerNationalApprovalFileUrl,
            LocationName = locationName,
            Longitude = longitude,
            Latitude = latitude,
            BuildingType = buildingType,
            BuildingNumber = buildingNumber,
            FloorNumber = floorNumber,
            ApartmentNumber = apartmentNumber,
            StreetNumber = streetNumber,
            PhoneNumber = phoneNumber,
            IsAccepted = isAccepted,
            IsRejected = isRejected,
            SubmitDate = submitDate,
            ReviewDate = reviewDate,
            ProviderId = providerId,
            IsActive = isActive
        }, transaction);
    }

    public static async Task<bool> UpdateProviderProfileHistoryAsync(IDbConnection connection,
        long id,
        string firstName,
        string secondName,
        string nationalApprovalId,
        string companyName,
        string ownerName,
        string ownerNationalId,
        string ownershipDocumentFileUrl,
        string ownerNationalApprovalFileUrl,
        string locationName,
        decimal longitude,
        decimal latitude,
        string buildingType,
        string buildingNumber,
        string floorNumber,
        string apartmentNumber,
        string streetNumber,
        string phoneNumber,
        bool isAccepted,
        bool isRejected,
        DateTime submitDate,
        DateTime? reviewDate,
        int providerId,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProviderProfileHistory]
                                    SET 
                                        [FirstName] = @FirstName,
                                        [SecondName] = @SecondName,
                                        [NationalApprovalId] = @NationalApprovalId,
                                        [CompanyName] = @CompanyName,
                                        [OwnerName] = @OwnerName,
                                        [OwnerNationalId] = @OwnerNationalId,
                                        [OwnershipDocumentFileUrl] = @OwnershipDocumentFileUrl,
                                        [OwnerNationalApprovalFileUrl] = @OwnerNationalApprovalFileUrl,
                                        [LocationName] = @LocationName,
                                        [Longitude] = @Longitude,
                                        [Latitude] = @Latitude,
                                        [BuildingType] = @BuildingType,
                                        [BuildingNumber] = @BuildingNumber,
                                        [FloorNumber] = @FloorNumber,
                                        [ApartmentNumber] = @ApartmentNumber,
                                        [StreetNumber] = @StreetNumber,
                                        [PhoneNumber] = @PhoneNumber,
                                        [IsAccepted] = @IsAccepted,
                                        [IsRejected] = @IsRejected,
                                        [SubmitDate] = @SubmitDate,
                                        [ReviewDate] = @ReviewDate,
                                        [ProviderId] = @ProviderId,
                                        [IsActive] = @IsActive
                                    WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            FirstName = firstName,
            SecondName = secondName,
            NationalApprovalId = nationalApprovalId,
            CompanyName = companyName,
            OwnerName = ownerName,
            OwnerNationalId = ownerNationalId,
            OwnershipDocumentFileUrl = ownershipDocumentFileUrl,
            OwnerNationalApprovalFileUrl = ownerNationalApprovalFileUrl,
            LocationName = locationName,
            Longitude = longitude,
            Latitude = latitude,
            BuildingType = buildingType,
            BuildingNumber = buildingNumber,
            FloorNumber = floorNumber,
            ApartmentNumber = apartmentNumber,
            StreetNumber = streetNumber,
            PhoneNumber = phoneNumber,
            IsAccepted = isAccepted,
            IsRejected = isRejected,
            SubmitDate = submitDate,
            ReviewDate = reviewDate,
            ProviderId = providerId,
            IsActive = isActive
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProviderProfileHistoryAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProviderProfileHistory]
                                    WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProviderProfileHistory?> GetProviderProfileHistoryByIdAsync(long id,
    IDbConnection connection,
    IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                                [Id], 
                                [FirstName], 
                                [SecondName], 
                                [NationalApprovalId], 
                                [CompanyName], 
                                [OwnerName], 
                                [OwnerNationalId], 
                                [OwnershipDocumentFileUrl], 
                                [OwnerNationalApprovalFileUrl], 
                                [LocationName], 
                                [Longitude], 
                                [Latitude], 
                                [BuildingType], 
                                [BuildingNumber], 
                                [FloorNumber], 
                                [ApartmentNumber], 
                                [StreetNumber], 
                                [PhoneNumber], 
                                [IsAccepted], 
                                [IsRejected], 
                                [SubmitDate], 
                                [ReviewDate], 
                                [ProviderId], 
                                [IsActive],
                                [ReviewDescription]
                                FROM [dbo].[ProviderProfileHistory]
                                WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderProfileHistory>(query, new
        {
            Id = id
        }, transaction);
    }

    public static async Task<IEnumerable<ProviderProfileHistory>> GetProviderProfileHistoryFilteredAsync(
        IDbConnection connection,
        int? providerId = null,
        bool? isActive = null,
        bool? isRejected = null,
        bool? isAccepted = null,
        DateTime? submitDateFrom = null,
        DateTime? submitDateTo = null,
        DateTime? reviewDateFrom = null,
        DateTime? reviewDateTo = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        const string query = @"
            SELECT
                [Id],
                [FirstName],
                [SecondName],
                [NationalApprovalId],
                [CompanyName],
                [OwnerName],
                [OwnerNationalId],
                [OwnershipDocumentFileUrl],
                [OwnerNationalApprovalFileUrl],
                [LocationName],
                [Longitude],
                [Latitude],
                [BuildingType],
                [BuildingNumber],
                [FloorNumber],
                [ApartmentNumber],
                [StreetNumber],
                [PhoneNumber],
                [IsAccepted],
                [IsRejected],
                [SubmitDate],
                [ReviewDate],
                [ProviderId],
                [IsActive],
                [ReviewDescription]
            FROM [dbo].[ProviderProfileHistory]
            WHERE
                (@ProviderId IS NULL OR [ProviderId] = @ProviderId)
                AND (@IsActive IS NULL OR [IsActive] = @IsActive)
                AND (@IsRejected IS NULL OR [IsRejected] = @IsRejected)
                AND (@IsAccepted IS NULL OR [IsAccepted] = @IsAccepted)
                AND (@SubmitDateFrom IS NULL OR [SubmitDate] >= @SubmitDateFrom)
                AND (@SubmitDateTo IS NULL OR [SubmitDate] <= @SubmitDateTo)
                AND (@ReviewDateFrom IS NULL OR [ReviewDate] >= @ReviewDateFrom)
                AND (@ReviewDateTo IS NULL OR [ReviewDate] <= @ReviewDateTo)
            ORDER BY [Id] DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (pageNumber - 1) * pageSize;

        return await connection.QueryAsync<ProviderProfileHistory>(query, new
        {
            ProviderId = providerId,
            IsActive = isActive,
            IsRejected = isRejected,
            IsAccepted = isAccepted,
            SubmitDateFrom = submitDateFrom,
            SubmitDateTo = submitDateTo,
            ReviewDateFrom = reviewDateFrom,
            ReviewDateTo = reviewDateTo,
            Offset = offset,
            PageSize = pageSize
        });
    }

    public static async Task<long> GetProviderProfileHistoryFilteredCountAsync(
        IDbConnection connection,
        int? providerId = null,
        bool? isActive = null,
        bool? isRejected = null,
        bool? isAccepted = null,
        DateTime? submitDateFrom = null,
        DateTime? submitDateTo = null,
        DateTime? reviewDateFrom = null,
        DateTime? reviewDateTo = null)
    {
        const string query = @"
        SELECT
            COUNT([Id])
        FROM [dbo].[ProviderProfileHistory]
        WHERE
            (@ProviderId IS NULL OR [ProviderId] = @ProviderId)
            AND (@IsActive IS NULL OR [IsActive] = @IsActive)
            AND (@IsRejected IS NULL OR [IsRejected] = @IsRejected)
            AND (@IsAccepted IS NULL OR [IsAccepted] = @IsAccepted)
            AND (@SubmitDateFrom IS NULL OR [SubmitDate] >= @SubmitDateFrom)
            AND (@SubmitDateTo IS NULL OR [SubmitDate] <= @SubmitDateTo)
            AND (@ReviewDateFrom IS NULL OR [ReviewDate] >= @ReviewDateFrom)
            AND (@ReviewDateTo IS NULL OR [ReviewDate] <= @ReviewDateTo);";


        return await connection.QueryFirstOrDefaultAsync<long>(query, new
        {
            ProviderId = providerId,
            IsActive = isActive,
            IsRejected = isRejected,
            IsAccepted = isAccepted,
            SubmitDateFrom = submitDateFrom,
            SubmitDateTo = submitDateTo,
            ReviewDateFrom = reviewDateFrom,
            ReviewDateTo = reviewDateTo
        });
    }
    public static async Task<bool> UpdateProviderProfileHistoryByIdAsync(
        IDbConnection connection,
        long id,
        bool isActive,
        bool isAccepted,
        bool isRejected,
        string reviewDescription,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            UPDATE [dbo].[ProviderProfileHistory]
            SET IsActive = @IsActive,
                IsAccepted = @IsAccepted,
                IsRejected = @IsRejected,
                ReviewDate = GETDATE(),
                ReviewDescription = @ReviewDescription
            WHERE Id = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(
            query,
            new
            {
                Id = id,
                IsActive = isActive,
                IsAccepted = isAccepted,
                IsRejected = isRejected,
                ReviewDescription = reviewDescription
            },
            transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeactivateProviderProfileHistoryByProviderIdAsync(
        IDbConnection connection,
        int providerId,
        bool isActive = false,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            UPDATE [dbo].[ProviderProfileHistory]
            SET IsActive = @IsActive
            WHERE ProviderId = @ProviderId";

        var updatedRowsCount = await connection.ExecuteAsync(
            query,
            new
            {
                ProviderId = providerId,
                IsActive = isActive
            },
            transaction);

        return updatedRowsCount > 0;
    }


    public static async Task<IEnumerable<ProviderProfileHistory>> GetAllProviderProfileHistoryAsync(
        IDbConnection connection,
        int providerId)
    {
        const string query = @"
            SELECT
                [Id],
                [FirstName],
                [SecondName],
                [NationalApprovalId],
                [CompanyName],
                [OwnerName],
                [OwnerNationalId],
                [OwnershipDocumentFileUrl],
                [OwnerNationalApprovalFileUrl],
                [LocationName],
                [Longitude],
                [Latitude],
                [BuildingType],
                [BuildingNumber],
                [FloorNumber],
                [ApartmentNumber],
                [StreetNumber],
                [PhoneNumber],
                [IsAccepted],
                [IsRejected],
                [SubmitDate],
                [ReviewDate],
                [ProviderId],
                [IsActive],
                [ReviewDescription]
            FROM [dbo].[ProviderProfileHistory]
            WHERE
                [ProviderId] = @ProviderId;";

        return await connection.QueryAsync<ProviderProfileHistory>(query, new
        {
            ProviderId = providerId,
        });
    }
    public static async Task<ProviderProfileHistory?> GetActiveProviderProfileHistoryAsync(
        IDbConnection connection,
        int providerId)
    {
        const string query = @"
            SELECT
                [Id],
                [FirstName],
                [SecondName],
                [NationalApprovalId],
                [CompanyName],
                [OwnerName],
                [OwnerNationalId],
                [OwnershipDocumentFileUrl],
                [OwnerNationalApprovalFileUrl],
                [LocationName],
                [Longitude],
                [Latitude],
                [BuildingType],
                [BuildingNumber],
                [FloorNumber],
                [ApartmentNumber],
                [StreetNumber],
                [PhoneNumber],
                [IsAccepted],
                [IsRejected],
                [SubmitDate],
                [ReviewDate],
                [ProviderId],
                [IsActive],
                [ReviewDescription]
            FROM [dbo].[ProviderProfileHistory]
            WHERE
                [ProviderId] = @ProviderId AND [IsActive] = 1;";

        return await connection.QueryFirstOrDefaultAsync<ProviderProfileHistory>(query, new
        {
            ProviderId = providerId,
        });
    }
    public static async Task<ProviderProfileHistory?> GetLatestProviderProfileHistoryAsync(
        IDbConnection connection,
        int providerId)
    {
        const string query = @"
            SELECT  Top(1)
                [Id],
                [FirstName],
                [SecondName],
                [NationalApprovalId],
                [CompanyName],
                [OwnerName],
                [OwnerNationalId],
                [OwnershipDocumentFileUrl],
                [OwnerNationalApprovalFileUrl],
                [LocationName],
                [Longitude],
                [Latitude],
                [BuildingType],
                [BuildingNumber],
                [FloorNumber],
                [ApartmentNumber],
                [StreetNumber],
                [PhoneNumber],
                [IsAccepted],
                [IsRejected],
                [SubmitDate],
                [ReviewDate],
                [ProviderId],
                [IsActive],
                [ReviewDescription]
            FROM [dbo].[ProviderProfileHistory]
            WHERE
                [ProviderId] = @ProviderId 
                ORDER BY [Id] DESC;";

        return await connection.QueryFirstOrDefaultAsync<ProviderProfileHistory>(query, new
        {
            ProviderId = providerId,
        });
    }
}


public class ProviderProfileHistory
{
    public long Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalApprovalId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string OwnerNationalId { get; set; } = null!;
    public string OwnershipDocumentFileUrl { get; set; } = null!;
    public string OwnerNationalApprovalFileUrl { get; set; } = null!;
    public string LocationName { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public string BuildingType { get; set; } = null!;
    public string BuildingNumber { get; set; } = null!;
    public string FloorNumber { get; set; } = null!;
    public string ApartmentNumber { get; set; } = null!;
    public string StreetNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsAccepted { get; set; }
    public bool IsRejected { get; set; }
    public DateTime SubmitDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public int ProviderId { get; set; }
    public bool IsActive { get; set; }
    public string? ReviewDescription { get; set; }

}