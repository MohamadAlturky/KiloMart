using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.Delivery.Profile.Models;

namespace KiloMart.Domain.Delivery.Profile.Services;

public static class DeliveryProfileService
{
    public static async Task<Result<CreateDeliveryProfileResponse>> InsertAsync(
        IDbFactory dbFactory,
        CreateDeliveryProfileRequest deliveryProfileRequest,
        CreateDeliveryProfileLocalizedRequest localizedRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string insertDeliveryProfileSql = @"
                    INSERT INTO DelivaryProfile (
                        Delivary, FirstName, SecondName, NationalName, NationalId,
                        LicenseNumber, LicenseExpiredDate, DrivingLicenseNumber, DrivingLicenseExpiredDate)
                    VALUES (
                        @Delivery, @FirstName, @SecondName, @NationalName, @NationalId,
                        @LicenseNumber, @LicenseExpiredDate, @DrivingLicenseNumber, @DrivingLicenseExpiredDate);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            var deliveryProfileId = await connection.QuerySingleAsync<int>(insertDeliveryProfileSql, new
            {
                deliveryProfileRequest.Delivery,
                deliveryProfileRequest.FirstName,
                deliveryProfileRequest.SecondName,
                deliveryProfileRequest.NationalName,
                deliveryProfileRequest.NationalId,
                deliveryProfileRequest.LicenseNumber,
                deliveryProfileRequest.LicenseExpiredDate,
                deliveryProfileRequest.DrivingLicenseNumber,
                deliveryProfileRequest.DrivingLicenseExpiredDate
            }, transaction);

            const string insertDeliveryProfileLocalizedSql = @"
                    INSERT INTO DelivaryProfileLocalized (DelivaryProfile, Language, FirstName, SecondName, NationalName)
                    VALUES (@DeliveryProfile, @Language, @FirstName, @SecondName, @NationalName);";

            await connection.ExecuteAsync(insertDeliveryProfileLocalizedSql, new
            {
                DeliveryProfile = deliveryProfileId,
                localizedRequest.Language,
                localizedRequest.FirstName,
                localizedRequest.SecondName,
                localizedRequest.NationalName
            }, transaction);

            transaction.Commit();

            return Result<CreateDeliveryProfileResponse>.Ok(new CreateDeliveryProfileResponse
            {
                Id = deliveryProfileId,
                Delivery = deliveryProfileRequest.Delivery,
                FirstName = deliveryProfileRequest.FirstName,
                SecondName = deliveryProfileRequest.SecondName,
                NationalName = deliveryProfileRequest.NationalName,
                NationalId = deliveryProfileRequest.NationalId,
                LicenseNumber = deliveryProfileRequest.LicenseNumber,
                LicenseExpiredDate = deliveryProfileRequest.LicenseExpiredDate,
                DrivingLicenseNumber = deliveryProfileRequest.DrivingLicenseNumber,
                DrivingLicenseExpiredDate = deliveryProfileRequest.DrivingLicenseExpiredDate
            });
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<CreateDeliveryProfileResponse>.Fail([e.Message]);
        }
    }
}
