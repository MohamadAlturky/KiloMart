using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<int> InsertProviderDocumentAsync(IDbConnection connection,
        string name,
        byte documentType,
        string url,
        int provider,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProviderDocument]
                            ([Name], [DocumentType], [Url], [Provider])
                            VALUES (@Name, @DocumentType, @Url, @Provider)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Name = name,
            DocumentType = documentType,
            Url = url,
            Provider = provider
        }, transaction);
    }

    public static async Task<bool> UpdateProviderDocumentAsync(IDbConnection connection,
        int id,
        string name,
        byte documentType,
        string url,
        int provider,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProviderDocument]
                                SET 
                                [Name] = @Name,
                                [DocumentType] = @DocumentType,
                                [Url] = @Url,
                                [Provider] = @Provider
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = name,
            DocumentType = documentType,
            Url = url,
            Provider = provider
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProviderDocumentAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProviderDocument]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProviderDocument?> GetProviderDocumentByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Name], 
                            [DocumentType], 
                            [Url], 
                            [Provider]
                            FROM [dbo].[ProviderDocument]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProviderDocument>(query, new
        {
            Id = id
        });
    }

    public static async Task<IEnumerable<ProviderDocument>> GetProviderDocumentsByProviderIdAsync(int providerId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Name], 
                            [DocumentType], 
                            [Url], 
                            [Provider]
                            FROM [dbo].[ProviderDocument]
                            WHERE [Provider] = @ProviderId";

        return await connection.QueryAsync<ProviderDocument>(query, new
        {
            ProviderId = providerId
        });
    }
}
public class ProviderDocument
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte DocumentType { get; set; }
    public string Url { get; set; } = null!;
    public int Provider { get; set; }
}