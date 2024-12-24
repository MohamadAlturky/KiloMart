//using Dapper;
//using KiloMart.Core.Contracts;
//using KiloMart.Core.Models;

//namespace KiloMart.Domain.Delivery.Profile;

//public static class DeliveryProfileService
//{
//    public static async Task<Result<CreateDeliveryProfileResponse>> InsertAsync(
//        IDbFactory dbFactory,
//        CreateDeliveryProfileRequest deliveryProfileRequest)
//    {
//        using var connection = dbFactory.CreateDbConnection();
//        connection.Open();

//        try
//        {
//            const string insertDeliveryProfileSql = @"
//                    INSERT INTO DelivaryProfile (
//                        Delivary, FirstName, SecondName, NationalName, NationalId,
//                        LicenseNumber, LicenseExpiredDate, DrivingLicenseNumber, DrivingLicenseExpiredDate)
//                    VALUES (
//                        @Delivery, @FirstName, @SecondName, @NationalName, @NationalId,
//                        @LicenseNumber, @LicenseExpiredDate, @DrivingLicenseNumber, @DrivingLicenseExpiredDate);
//                    SELECT CAST(SCOPE_IDENTITY() as int);";

//            var deliveryProfileId = await connection.QuerySingleAsync<int>(insertDeliveryProfileSql, new
//            {
//                deliveryProfileRequest.Delivery,
//                deliveryProfileRequest.FirstName,
//                deliveryProfileRequest.SecondName,
//                deliveryProfileRequest.NationalName,
//                deliveryProfileRequest.NationalId,
//                deliveryProfileRequest.LicenseNumber,
//                deliveryProfileRequest.LicenseExpiredDate,
//                deliveryProfileRequest.DrivingLicenseNumber,
//                deliveryProfileRequest.DrivingLicenseExpiredDate
//            });



//            return Result<CreateDeliveryProfileResponse>.Ok(new CreateDeliveryProfileResponse
//            {
//                Id = deliveryProfileId,
//                Delivery = deliveryProfileRequest.Delivery,
//                FirstName = deliveryProfileRequest.FirstName,
//                SecondName = deliveryProfileRequest.SecondName,
//                NationalName = deliveryProfileRequest.NationalName,
//                NationalId = deliveryProfileRequest.NationalId,
//                LicenseNumber = deliveryProfileRequest.LicenseNumber,
//                LicenseExpiredDate = deliveryProfileRequest.LicenseExpiredDate,
//                DrivingLicenseNumber = deliveryProfileRequest.DrivingLicenseNumber,
//                DrivingLicenseExpiredDate = deliveryProfileRequest.DrivingLicenseExpiredDate
//            });
//        }
//        catch (Exception e)
//        {
//            return Result<CreateDeliveryProfileResponse>.Fail([e.Message]);
//        }
//    }
//}

using System.Data;
using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.Deliveries.Profile;

public static class DeliveryProfileService
{
    public static async Task<Result<CreateDeliveryProfileResponse>> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        CreateDeliveryProfileRequest deliveryProfileRequest)
    {
        var existingProfile = await Db.GetDeliveryProfileByDeliveryIdAsync(deliveryProfileRequest.Delivery, connection, transaction);
        if (existingProfile is not null)
        {
            return Result<CreateDeliveryProfileResponse>.Fail(new[] { "Profile Already Exists" });
        }

        try
        {
            // SQL to insert into DeliveryProfile and retrieve the new Id
            const string insertDeliveryProfileSql = @"
                    INSERT INTO DelivaryProfile 
                    (Delivary, FirstName, SecondName, NationalName, NationalId, LicenseNumber, LicenseExpiredDate, DrivingLicenseNumber, DrivingLicenseExpiredDate)
                    VALUES 
                    (@Delivery, @FirstName, @SecondName, @NationalName, @NationalId, @LicenseNumber, @LicenseExpiredDate, @DrivingLicenseNumber, @DrivingLicenseExpiredDate);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            // Insert into DeliveryProfile and get the new ID
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
            return Result<CreateDeliveryProfileResponse>.Fail(new[] { e.Message });
        }
    }

    public static async Task<Result<UpdateDeliveryProfileResponse>> UpdateAsync(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        UpdateDeliveryProfileRequest deliveryProfileRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        var existingProfile = await Db.GetDeliveryProfileByDeliveryIdAsync(userPayLoad.Party, connection);
        if (existingProfile is null)
        {
            return Result<UpdateDeliveryProfileResponse>.Fail(new[] { "Profile Not Found" });
        }
        if (existingProfile.Delivary != userPayLoad.Party)
        {
            return Result<UpdateDeliveryProfileResponse>.Fail(new[] { "Unauthorized" });
        }

        try
        {
            existingProfile.FirstName = deliveryProfileRequest.FirstName ?? existingProfile.FirstName;
            existingProfile.SecondName = deliveryProfileRequest.SecondName ?? existingProfile.SecondName;
            existingProfile.NationalName = deliveryProfileRequest.NationalName ?? existingProfile.NationalName;
            existingProfile.NationalId = deliveryProfileRequest.NationalId ?? existingProfile.NationalId;
            existingProfile.LicenseNumber = deliveryProfileRequest.LicenseNumber ?? existingProfile.LicenseNumber;
            if(deliveryProfileRequest.LicenseExpiredDate.HasValue)
            {
                existingProfile.LicenseExpiredDate = deliveryProfileRequest.LicenseExpiredDate.Value;
            }
            existingProfile.DrivingLicenseNumber = deliveryProfileRequest.DrivingLicenseNumber ?? existingProfile.DrivingLicenseNumber;
            existingProfile.DrivingLicenseExpiredDate = deliveryProfileRequest.DrivingLicenseExpiredDate ?? existingProfile.DrivingLicenseExpiredDate;

            // SQL to update the DeliveryProfile
            const string updateDeliveryProfileSql = @"
                    UPDATE DelivaryProfile
                    SET FirstName = @FirstName,
                        SecondName = @SecondName,
                        NationalName = @NationalName,
                        NationalId = @NationalId,
                        LicenseNumber = @LicenseNumber,
                        LicenseExpiredDate = @LicenseExpiredDate,
                        DrivingLicenseNumber = @DrivingLicenseNumber,
                        DrivingLicenseExpiredDate = @DrivingLicenseExpiredDate
                    WHERE Delivary = @Delivery;";

            // Execute the update query
            var affectedRows = await connection.ExecuteAsync(updateDeliveryProfileSql, new
            {
                deliveryProfileRequest.FirstName,
                deliveryProfileRequest.SecondName,
                deliveryProfileRequest.NationalName,
                deliveryProfileRequest.NationalId,
                deliveryProfileRequest.LicenseNumber,
                deliveryProfileRequest.LicenseExpiredDate,
                deliveryProfileRequest.DrivingLicenseNumber,
                deliveryProfileRequest.DrivingLicenseExpiredDate,
                Delivery = userPayLoad.Party
            });

            // Check if the update was successful
            if (affectedRows > 0)
            {
                return Result<UpdateDeliveryProfileResponse>.Ok(new UpdateDeliveryProfileResponse
                {
                    DeliveryId = existingProfile.Delivary,
                    FirstName = existingProfile.FirstName,
                    SecondName = existingProfile.SecondName,
                    NationalName = existingProfile.NationalName,
                    NationalId = existingProfile.NationalId,
                    LicenseNumber = existingProfile.LicenseNumber,
                    LicenseExpiredDate = existingProfile.LicenseExpiredDate,
                    DrivingLicenseNumber = existingProfile.DrivingLicenseNumber,
                    DrivingLicenseExpiredDate = existingProfile.DrivingLicenseExpiredDate
                });
            }
            else
            {
                return Result<UpdateDeliveryProfileResponse>.Fail(new[] { "Update failed, no rows affected." });
            }
        }
        catch (Exception e)
        {
            return Result<UpdateDeliveryProfileResponse>.Fail(new[] { e.Message });
        }
    }
}
