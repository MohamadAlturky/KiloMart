using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;

namespace KiloMart.Domain.Delivery.Profile;

public static class DeliveryProfileService
{
    public static async Task<Result<CreateDeliveryProfileResponse>> InsertAsync(
        IDbFactory dbFactory,
        CreateDeliveryProfileRequest deliveryProfileRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

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
            });



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
            return Result<CreateDeliveryProfileResponse>.Fail([e.Message]);
        }
    }
}
