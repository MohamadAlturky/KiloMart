using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;
public static partial class Db
{
    public static async Task<long> InsertDeliveryProfileHistoryAsync(IDbConnection connection,
        string firstName,
        string secondName,
        string nationalName,
        string nationalId,
        string licenseNumber,
        DateTime licenseExpiredDate,
        string drivingLicenseNumber,
        DateTime drivingLicenseExpiredDate,
        string vehicleNumber,
        string vehicleModel,
        string vehicleType,
        string vehicleYear,
        string vehiclePhotoFileUrl,
        string drivingLicenseFileUrl,
        string vehicleLicenseFileUrl,
        string nationalIqamaIDFileUrl,
        DateTime submitDate,
        int deliveryId,
        bool isActive,
        bool isRejected,
        bool isAccepted,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[DeliveryProfileHistory]
                            ([FirstName], [SecondName], [NationalName], [NationalId], [LicenseNumber], [LicenseExpiredDate],
                            [DrivingLicenseNumber], [DrivingLicenseExpiredDate], [VehicleNumber], [VehicleModel], [VehicleType],
                            [VehicleYear], [VehiclePhotoFileUrl], [DrivingLicenseFileUrl], [VehicleLicenseFileUrl],
                            [NationalIqamaIDFileUrl], [SubmitDate], [DeliveryId], [IsActive], [IsRejected], [IsAccepted])
                            VALUES (@FirstName, @SecondName, @NationalName, @NationalId, @LicenseNumber, @LicenseExpiredDate,
                            @DrivingLicenseNumber, @DrivingLicenseExpiredDate, @VehicleNumber, @VehicleModel, @VehicleType,
                            @VehicleYear, @VehiclePhotoFileUrl, @DrivingLicenseFileUrl, @VehicleLicenseFileUrl,
                            @NationalIqamaIDFileUrl, @SubmitDate, @DeliveryId, @IsActive, @IsRejected, @IsAccepted)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            FirstName = firstName,
            SecondName = secondName,
            NationalName = nationalName,
            NationalId = nationalId,
            LicenseNumber = licenseNumber,
            LicenseExpiredDate = licenseExpiredDate,
            DrivingLicenseNumber = drivingLicenseNumber,
            DrivingLicenseExpiredDate = drivingLicenseExpiredDate,
            VehicleNumber = vehicleNumber,
            VehicleModel = vehicleModel,
            VehicleType = vehicleType,
            VehicleYear = vehicleYear,
            VehiclePhotoFileUrl = vehiclePhotoFileUrl,
            DrivingLicenseFileUrl = drivingLicenseFileUrl,
            VehicleLicenseFileUrl = vehicleLicenseFileUrl,
            NationalIqamaIDFileUrl = nationalIqamaIDFileUrl,
            SubmitDate = submitDate,
            DeliveryId = deliveryId,
            IsActive = isActive,
            IsRejected = isRejected,
            IsAccepted = isAccepted
        }, transaction);
    }

    public static async Task<bool> UpdateDeliveryProfileHistoryAsync(IDbConnection connection,
        long id,
        string firstName,
        string secondName,
        string nationalName,
        string nationalId,
        string licenseNumber,
        DateTime licenseExpiredDate,
        string drivingLicenseNumber,
        DateTime drivingLicenseExpiredDate,
        string vehicleNumber,
        string vehicleModel,
        string vehicleType,
        string vehicleYear,
        string vehiclePhotoFileUrl,
        string drivingLicenseFileUrl,
        string vehicleLicenseFileUrl,
        string nationalIqamaIDFileUrl,
        DateTime submitDate,
        DateTime? reviewDate,
        int deliveryId,
        bool isActive,
        bool isRejected,
        bool isAccepted,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DeliveryProfileHistory]
                                SET 
                                [FirstName] = @FirstName,
                                [SecondName] = @SecondName,
                                [NationalName] = @NationalName,
                                [NationalId] = @NationalId,
                                [LicenseNumber] = @LicenseNumber,
                                [LicenseExpiredDate] = @LicenseExpiredDate,
                                [DrivingLicenseNumber] = @DrivingLicenseNumber,
                                [DrivingLicenseExpiredDate] = @DrivingLicenseExpiredDate,
                                [VehicleNumber] = @VehicleNumber,
                                [VehicleModel] = @VehicleModel,
                                [VehicleType] = @VehicleType,
                                [VehicleYear] = @VehicleYear,
                                [VehiclePhotoFileUrl] = @VehiclePhotoFileUrl,
                                [DrivingLicenseFileUrl] = @DrivingLicenseFileUrl,
                                [VehicleLicenseFileUrl] = @VehicleLicenseFileUrl,
                                [NationalIqamaIDFileUrl] = @NationalIqamaIDFileUrl,
                                [SubmitDate] = @SubmitDate,
                                [ReviewDate] = @ReviewDate,
                                [DeliveryId] = @DeliveryId,
                                [IsActive] = @IsActive,
                                [IsRejected] = @IsRejected,
                                [IsAccepted] = @IsAccepted
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            FirstName = firstName,
            SecondName = secondName,
            NationalName = nationalName,
            NationalId = nationalId,
            LicenseNumber = licenseNumber,
            LicenseExpiredDate = licenseExpiredDate,
            DrivingLicenseNumber = drivingLicenseNumber,
            DrivingLicenseExpiredDate = drivingLicenseExpiredDate,
            VehicleNumber = vehicleNumber,
            VehicleModel = vehicleModel,
            VehicleType = vehicleType,
            VehicleYear = vehicleYear,
            VehiclePhotoFileUrl = vehiclePhotoFileUrl,
            DrivingLicenseFileUrl = drivingLicenseFileUrl,
            VehicleLicenseFileUrl = vehicleLicenseFileUrl,
            NationalIqamaIDFileUrl = nationalIqamaIDFileUrl,
            SubmitDate = submitDate,
            ReviewDate = reviewDate,
            DeliveryId = deliveryId,
            IsActive = isActive,
            IsRejected = isRejected,
            IsAccepted = isAccepted
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteDeliveryProfileHistoryAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[DeliveryProfileHistory]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }



    public static async Task<DeliveryProfileHistory?> GetDeliveryProfileHistoryByIdAsync(
        long id,
        IDbConnection connection,
        IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                            [Id], 
                            [FirstName], 
                            [SecondName], 
                            [NationalName], 
                            [NationalId], 
                            [LicenseNumber], 
                            [LicenseExpiredDate], 
                            [DrivingLicenseNumber], 
                            [DrivingLicenseExpiredDate], 
                            [VehicleNumber], 
                            [VehicleModel], 
                            [VehicleType], 
                            [VehicleYear], 
                            [VehiclePhotoFileUrl], 
                            [DrivingLicenseFileUrl], 
                            [VehicleLicenseFileUrl], 
                            [NationalIqamaIDFileUrl], 
                            [SubmitDate], 
                            [ReviewDate], 
                            [DeliveryId], 
                            [IsActive], 
                            [IsRejected], 
                            [IsAccepted],
                            [ReviewDescription]
                            FROM [dbo].[DeliveryProfileHistory]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<DeliveryProfileHistory>(query, new
        {
            Id = id
        }, transaction);
    }
    public static async Task<DeliveryProfileHistory?> GetDeliveryActiveProfileHistoryAsync(
            IDbConnection connection,
            int deliveryId)
    {
        const string query = @"
        SELECT
            [Id],
            [FirstName],
            [SecondName],
            [NationalName],
            [NationalId],
            [LicenseNumber],
            [LicenseExpiredDate],
            [DrivingLicenseNumber],
            [DrivingLicenseExpiredDate],
            [VehicleNumber],
            [VehicleModel],
            [VehicleType],
            [VehicleYear],
            [VehiclePhotoFileUrl],
            [DrivingLicenseFileUrl],
            [VehicleLicenseFileUrl],
            [NationalIqamaIDFileUrl],
            [SubmitDate],
            [ReviewDate],
            [DeliveryId],
            [IsActive],
            [IsRejected],
            [IsAccepted],
            [ReviewDescription]
        FROM [dbo].[DeliveryProfileHistory]
        WHERE
            [DeliveryId] = @DeliveryId AND [IsActive] = 1;";


        return await connection.QueryFirstOrDefaultAsync<DeliveryProfileHistory>(query, new
        {
            DeliveryId = deliveryId,
        });
    }
    public static async Task<IEnumerable<DeliveryProfileHistory>> GetDeliveryAllProfileHistoryAsync(
            IDbConnection connection,
            int deliveryId)
    {
        const string query = @"
        SELECT
            [Id],
            [FirstName],
            [SecondName],
            [NationalName],
            [NationalId],
            [LicenseNumber],
            [LicenseExpiredDate],
            [DrivingLicenseNumber],
            [DrivingLicenseExpiredDate],
            [VehicleNumber],
            [VehicleModel],
            [VehicleType],
            [VehicleYear],
            [VehiclePhotoFileUrl],
            [DrivingLicenseFileUrl],
            [VehicleLicenseFileUrl],
            [NationalIqamaIDFileUrl],
            [SubmitDate],
            [ReviewDate],
            [DeliveryId],
            [IsActive],
            [IsRejected],
            [IsAccepted],
            [ReviewDescription]
        FROM [dbo].[DeliveryProfileHistory]
        WHERE
            [DeliveryId] = @DeliveryId;";


        return await connection.QueryAsync<DeliveryProfileHistory>(query, new
        {
            DeliveryId = deliveryId,
        });
    }
    public static async Task<IEnumerable<DeliveryProfileHistory>> GetDeliveryProfileHistoryFilteredAsync(
        IDbConnection connection,
        int? deliveryId = null,
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
            [NationalName],
            [NationalId],
            [LicenseNumber],
            [LicenseExpiredDate],
            [DrivingLicenseNumber],
            [DrivingLicenseExpiredDate],
            [VehicleNumber],
            [VehicleModel],
            [VehicleType],
            [VehicleYear],
            [VehiclePhotoFileUrl],
            [DrivingLicenseFileUrl],
            [VehicleLicenseFileUrl],
            [NationalIqamaIDFileUrl],
            [SubmitDate],
            [ReviewDate],
            [DeliveryId],
            [IsActive],
            [IsRejected],
            [IsAccepted],
            [ReviewDescription]
        FROM [dbo].[DeliveryProfileHistory]
        WHERE
            (@DeliveryId IS NULL OR [DeliveryId] = @DeliveryId)
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

        return await connection.QueryAsync<DeliveryProfileHistory>(query, new
        {
            DeliveryId = deliveryId,
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
    public static async Task<long> GetDeliveryProfileHistoryFilteredCountAsync(
        IDbConnection connection,
        int? deliveryId = null,
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
    FROM [dbo].[DeliveryProfileHistory]
    WHERE
        (@DeliveryId IS NULL OR [DeliveryId] = @DeliveryId)
        AND (@IsActive IS NULL OR [IsActive] = @IsActive)
        AND (@IsRejected IS NULL OR [IsRejected] = @IsRejected)
        AND (@IsAccepted IS NULL OR [IsAccepted] = @IsAccepted)
        AND (@SubmitDateFrom IS NULL OR [SubmitDate] >= @SubmitDateFrom)
        AND (@SubmitDateTo IS NULL OR [SubmitDate] <= @SubmitDateTo)
        AND (@ReviewDateFrom IS NULL OR [ReviewDate] >= @ReviewDateFrom)
        AND (@ReviewDateTo IS NULL OR [ReviewDate] <= @ReviewDateTo);";


        return await connection.QueryFirstOrDefaultAsync<long>(query, new
        {
            DeliveryId = deliveryId,
            IsActive = isActive,
            IsRejected = isRejected,
            IsAccepted = isAccepted,
            SubmitDateFrom = submitDateFrom,
            SubmitDateTo = submitDateTo,
            ReviewDateFrom = reviewDateFrom,
            ReviewDateTo = reviewDateTo
        });
    }


    public static async Task<bool> UpdateDeliveryProfileHistoryByIdAsync(
            IDbConnection connection,
            long id,
            bool isActive,
            bool isAccepted,
            bool isRejected,
            string reviewDescription,
            IDbTransaction? transaction = null)
    {
        const string query = @"
                UPDATE [dbo].[DeliveryProfileHistory]
                SET IsActive = @IsActive,
                    IsAccepted = @IsAccepted,
                    IsRejected = @IsRejected,
                    ReviewDescription = @ReviewDescription
                    ReviewDate = GETDATE()
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

    public static async Task<bool> DeactivateDeliveryProfileHistoryByDeliveryIdAsync(
        IDbConnection connection,
        int deliveryId,
        bool isActive = false,
        IDbTransaction? transaction = null)
    {
        const string query = @"
                UPDATE [dbo].[DeliveryProfileHistory]
                SET IsActive = @IsActive
                WHERE DeliveryId = @DeliveryId";

        var updatedRowsCount = await connection.ExecuteAsync(
            query,
            new
            {
                DeliveryId = deliveryId,
                IsActive = isActive
            },
            transaction);

        return updatedRowsCount > 0;
    }
}



public class DeliveryProfileHistory
{
    public long Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateTime DrivingLicenseExpiredDate { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public string VehicleModel { get; set; } = null!;
    public string VehicleType { get; set; } = null!;
    public string VehicleYear { get; set; } = null!;
    public string VehiclePhotoFileUrl { get; set; } = null!;
    public string DrivingLicenseFileUrl { get; set; } = null!;
    public string VehicleLicenseFileUrl { get; set; } = null!;
    public string NationalIqamaIDFileUrl { get; set; } = null!;
    public DateTime SubmitDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public int DeliveryId { get; set; }
    public bool IsActive { get; set; }
    public bool IsRejected { get; set; }
    public bool IsAccepted { get; set; }
    public string? ReviewDescription { get; set; }
}