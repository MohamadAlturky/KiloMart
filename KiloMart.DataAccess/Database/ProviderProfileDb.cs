using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<int> InsertProviderProfileAsync(IDbConnection connection,
        int provider,
        string firstName,
        string secondName,
        string ownerNationalId,
        string nationalApprovalId,
        string companyName,
        string ownerName,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProviderProfile]
                            ([Provider], [FirstName], [SecondName], [OwnerNationalId], [NationalApprovalId], [CompanyName], [OwnerName])
                            VALUES (@Provider, @FirstName, @SecondName, @OwnerNationalId, @NationalApprovalId, @CompanyName, @OwnerName)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Provider = provider,
            FirstName = firstName,
            SecondName = secondName,
            OwnerNationalId = ownerNationalId,
            NationalApprovalId = nationalApprovalId,
            CompanyName = companyName,
            OwnerName = ownerName
        }, transaction);
    }

    public static async Task<bool> UpdateProviderProfileAsync(IDbConnection connection,
        int id,
        int provider,
        string firstName,
        string secondName,
        string ownerNationalId,
        string nationalApprovalId,
        string companyName,
        string ownerName,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProviderProfile]
                                SET 
                                [Provider] = @Provider,
                                [FirstName] = @FirstName,
                                [SecondName] = @SecondName,
                                [OwnerNationalId] = @OwnerNationalId,
                                [NationalApprovalId] = @NationalApprovalId,
                                [CompanyName] = @CompanyName,
                                [OwnerName] = @OwnerName
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Provider = provider,
            FirstName = firstName,
            SecondName = secondName,
            OwnerNationalId = ownerNationalId,
            NationalApprovalId = nationalApprovalId,
            CompanyName = companyName,
            OwnerName = ownerName
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProviderProfileAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProviderProfile]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProviderProfile?> GetProviderProfileByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Provider], 
                            [FirstName], 
                            [SecondName], 
                            [OwnerNationalId], 
                            [NationalApprovalId], 
                            [CompanyName], 
                            [OwnerName]
                            FROM [dbo].[ProviderProfile]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderProfile>(query, new
        {
            Id = id
        });
    }
    public static async Task<ProviderProfile?> GetProviderProfileByProviderIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Provider], 
                            [FirstName], 
                            [SecondName], 
                            [OwnerNationalId], 
                            [NationalApprovalId], 
                            [CompanyName], 
                            [OwnerName]
                            FROM [dbo].[ProviderProfile]
                            WHERE [Provider] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderProfile>(query, new
        {
            Id = id
        });
    }
}

public class ProviderProfile
{
    public int Id { get; set; }
    public int Provider { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string OwnerNationalId { get; set; } = null!;
    public string NationalApprovalId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
}
