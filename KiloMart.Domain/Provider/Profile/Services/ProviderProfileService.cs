using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Provider.Profile.Models;

namespace KiloMart.Domain.Provider.Profile.Services;

public static class ProviderProfileService
{
    public static async Task<Result<CreateProviderProfileResponse>> InsertAsync(
        IDbFactory dbFactory,
        CreateProviderProfileRequest providerProfileRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

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
            });

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
            return Result<CreateProviderProfileResponse>.Fail([e.Message]);
        }
    }
}
