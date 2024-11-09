using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Provider.Profile.Models;

namespace KiloMart.Domain.Provider.Profile.Services;

public static class ProviderProfileService
{
    public static async Task<Result<CreateProviderProfileResponse>> InsertAsync(
        IDbFactory dbFactory,
        CreateProviderProfileRequest providerProfileRequest,
        CreateProviderProfileLocalizedRequest localizedRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string insertProviderProfileSql = @"
                INSERT INTO ProviderProfile (
                    Provider, FirstName, SecondName, OwnerNationalId,
                    NationalApprovalId, CompanyName, OwnerName)
                VALUES (
                    @Provider, @FirstName, @SecondName, @OwnerNationalId,
                    @NationalApprovalId, @CompanyName, @OwnerName);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var providerProfileId = await connection.QuerySingleAsync<int>(insertProviderProfileSql, new
            {
                providerProfileRequest.Provider,
                providerProfileRequest.FirstName,
                providerProfileRequest.SecondName,
                providerProfileRequest.OwnerNationalId,
                providerProfileRequest.NationalApprovalId,
                providerProfileRequest.CompanyName,
                providerProfileRequest.OwnerName
            }, transaction);

            const string insertProviderProfileLocalizedSql = @"
                INSERT INTO ProviderProfileLocalized (
                    ProviderProfile, Language, FirstName, SecondName, CompanyName, OwnerName)
                VALUES (
                    @ProviderProfile, @Language, @FirstName, @SecondName, @CompanyName, @OwnerName);";

            await connection.ExecuteAsync(insertProviderProfileLocalizedSql, new
            {
                ProviderProfile = providerProfileId,
                localizedRequest.Language,
                localizedRequest.FirstName,
                localizedRequest.SecondName,
                localizedRequest.CompanyName,
                localizedRequest.OwnerName
            }, transaction);

            transaction.Commit();

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
            transaction.Rollback();
            return Result<CreateProviderProfileResponse>.Fail([e.Message]);
        }
    }
}
