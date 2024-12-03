using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertProviderWalletAsync(IDbConnection connection,
        float value,
        int Provider,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProviderWallet]
                            ([Value], [Provider])
                            VALUES (@Value, @Provider)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Value = value,
            Provider = Provider
        }, transaction);
    }

    public static async Task<bool> UpdateProviderWalletAsync(IDbConnection connection,
        int id,
        float value,
        int Provider,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProviderWallet]
                                SET 
                                [Value] = @Value,
                                [Provider] = @Provider
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Value = value,
            Provider = Provider
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProviderWalletAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProviderWallet]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProviderWallet?> GetProviderWalletByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Value], 
                            [Provider]
                            FROM [dbo].[ProviderWallet]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderWallet>(query, new
        {
            Id = id
        });
    }
    public static async Task<ProviderWallet?> GetProviderWalletByProviderIdAsync(int ProviderId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Value], 
                            [Provider]
                            FROM [dbo].[ProviderWallet]
                            WHERE [Provider] = @ProviderId";

        return await connection.QueryFirstOrDefaultAsync<ProviderWallet>(query, new
        {
            ProviderId = ProviderId
        });
    }
}

public class ProviderWallet
{
    public int Id { get; set; }
    public float Value { get; set; }
    public int Provider { get; set; }
}