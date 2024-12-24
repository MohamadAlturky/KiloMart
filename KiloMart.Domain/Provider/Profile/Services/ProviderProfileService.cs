//using Dapper;
//using KiloMart.Core.Contracts;
//using KiloMart.Core.Models;
//using KiloMart.Domain.Provider.Profile.Models;

//namespace KiloMart.Domain.Provider.Profile.Services;

//public static class ProviderProfileService
//{
//    public static async Task<Result<CreateProviderProfileResponse>> InsertAsync(
//        IDbFactory dbFactory,
//        CreateProviderProfileRequest providerProfileRequest)
//    {
//        using var connection = dbFactory.CreateDbConnection();
//        connection.Open();

//        try
//        {
//            const string insertProviderProfileSql = @"
//                INSERT INTO ProviderProfile (
//                    Provider, FirstName, SecondName, OwnerNationalId,
//                    NationalApprovalId, CompanyName, OwnerName)
//                VALUES (
//                    @Provider, @FirstName, @SecondName, @OwnerNationalId,
//                    @NationalApprovalId, @CompanyName, @OwnerName);
//                SELECT CAST(SCOPE_IDENTITY() as int);";

//            var providerProfileId = await connection.QuerySingleAsync<int>(insertProviderProfileSql, new
//            {
//                providerProfileRequest.Provider,
//                providerProfileRequest.FirstName,
//                providerProfileRequest.SecondName,
//                providerProfileRequest.OwnerNationalId,
//                providerProfileRequest.NationalApprovalId,
//                providerProfileRequest.CompanyName,
//                providerProfileRequest.OwnerName
//            });

//            return Result<CreateProviderProfileResponse>.Ok(new CreateProviderProfileResponse
//            {
//                Id = providerProfileId,
//                Provider = providerProfileRequest.Provider,
//                FirstName = providerProfileRequest.FirstName,
//                SecondName = providerProfileRequest.SecondName,
//                OwnerNationalId = providerProfileRequest.OwnerNationalId,
//                NationalApprovalId = providerProfileRequest.NationalApprovalId,
//                CompanyName = providerProfileRequest.CompanyName,
//                OwnerName = providerProfileRequest.OwnerName
//            });
//        }
//        catch (Exception e)
//        {
//            return Result<CreateProviderProfileResponse>.Fail([e.Message]);
//        }
//    }
//}

using System.Data;
using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.Providers.Profile;

public static class ProviderProfileService
{
    public static async Task<Result<CreateProviderProfileResponse>> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        CreateProviderProfileRequest providerProfileRequest)
    {
        var existingProfile = await Db.GetProviderProfileByProviderIdAsync(providerProfileRequest.Provider, connection, transaction);
        if (existingProfile is not null)
        {
            return Result<CreateProviderProfileResponse>.Fail(["Profile Already Exists"]);
        }

        try
        {
            // SQL to insert into ProviderProfile and retrieve the new Id
            const string insertProviderProfileSql = @"
                    INSERT INTO ProviderProfile (Provider, FirstName, SecondName, OwnerNationalId, NationalApprovalId, CompanyName, OwnerName)
                    VALUES (@Provider, @FirstName, @SecondName, @OwnerNationalId, @NationalApprovalId, @CompanyName, @OwnerName);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            // Insert into ProviderProfile and get the new ID
            var providerProfileId = await connection.QuerySingleAsync<int>(insertProviderProfileSql, new
            {
                providerProfileRequest.Provider,
                providerProfileRequest.FirstName,
                providerProfileRequest.SecondName,
                providerProfileRequest.OwnerNationalId,
                providerProfileRequest.NationalApprovalId,
                providerProfileRequest.CompanyName,
                providerProfileRequest.OwnerName
            },transaction);

            return Result<CreateProviderProfileResponse>.Ok(new CreateProviderProfileResponse
            {
                Id = providerProfileId,
                Provider = providerProfileRequest.Provider,
                FirstName = providerProfileRequest.FirstName,
                SecondName = providerProfileRequest.SecondName,
                OwnerNationalId = providerProfileRequest.OwnerNationalId,
                NationalApprovalId = providerProfileRequest.NationalApprovalId,
                CompanyName = providerProfileRequest.CompanyName,
                OwnerName = providerProfileRequest.OwnerName
            });
        }
        catch (Exception e)
        {
            return Result<CreateProviderProfileResponse>.Fail(new[] { e.Message });
        }
    }

    public static async Task<Result<UpdateProviderProfileResponse>> UpdateAsync(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        UpdateProviderProfileRequest providerProfileRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        var existingProfile = await Db.GetProviderProfileByProviderIdAsync(userPayLoad.Party, connection);
        if (existingProfile is null)
        {
            return Result<UpdateProviderProfileResponse>.Fail(new[] { "Profile Not Found" });
        }
        if (existingProfile.Provider != userPayLoad.Party)
        {
            return Result<UpdateProviderProfileResponse>.Fail(new[] { "Unauthorized" });
        }

        try
        {
            existingProfile.FirstName = providerProfileRequest.FirstName ?? existingProfile.FirstName;
            existingProfile.SecondName = providerProfileRequest.SecondName ?? existingProfile.SecondName;
            existingProfile.OwnerNationalId = providerProfileRequest.OwnerNationalId ?? existingProfile.OwnerNationalId;
            existingProfile.NationalApprovalId = providerProfileRequest.NationalApprovalId ?? existingProfile.NationalApprovalId;
            existingProfile.CompanyName = providerProfileRequest.CompanyName ?? existingProfile.CompanyName;
            existingProfile.OwnerName = providerProfileRequest.OwnerName ?? existingProfile.OwnerName;

            // SQL to update the ProviderProfile
            const string updateProviderProfileSql = @"
                    UPDATE ProviderProfile
                    SET FirstName = @FirstName,
                        SecondName = @SecondName,
                        OwnerNationalId = @OwnerNationalId,
                        NationalApprovalId = @NationalApprovalId,
                        CompanyName = @CompanyName,
                        OwnerName = @OwnerName
                    WHERE Provider = @Provider;";

            // Execute the update query
            var affectedRows = await connection.ExecuteAsync(updateProviderProfileSql, new
            {
                providerProfileRequest.FirstName,
                providerProfileRequest.SecondName,
                providerProfileRequest.OwnerNationalId,
                providerProfileRequest.NationalApprovalId,
                providerProfileRequest.CompanyName,
                providerProfileRequest.OwnerName,
                Provider = userPayLoad.Party
            });

            // Check if the update was successful
            if (affectedRows > 0)
            {
                return Result<UpdateProviderProfileResponse>.Ok(new UpdateProviderProfileResponse
                {
                    ProviderId = existingProfile.Provider,
                    FirstName = existingProfile.FirstName,
                    SecondName = existingProfile.SecondName,
                    OwnerNationalId = existingProfile.OwnerNationalId,
                    NationalApprovalId = existingProfile.NationalApprovalId,
                    CompanyName = existingProfile.CompanyName,
                    OwnerName = existingProfile.OwnerName
                });
            }
            else
            {
                return Result<UpdateProviderProfileResponse>.Fail(new[] { "Update failed, no rows affected." });
            }
        }
        catch (Exception e)
        {
            return Result<UpdateProviderProfileResponse>.Fail(new[] { e.Message });
        }
    }
}

