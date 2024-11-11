using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<int> InsertDeliveryDocumentAsync(IDbConnection connection,
        string name,
        string url,
        int delivary,
        DeliveryDocumentType documentType,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[DelivaryDocument]
                            ([Name], [Url], [Delivary], [DocumentType])
                            VALUES (@Name, @Url, @Delivary, @DocumentType)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Name = name,
            Url = url,
            Delivary = delivary,
            DocumentType = (int)documentType
        }, transaction);
    }

    public static async Task<bool> UpdateDeliveryDocumentAsync(IDbConnection connection,
        int id,
        string name,
        string url,
        int delivary,
        DeliveryDocumentType documentType,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DelivaryDocument]
                                SET 
                                [Name] = @Name,
                                [Url] = @Url,
                                [Delivary] = @Delivary,
                                [DocumentType] = @DocumentType
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = name,
            Url = url,
            Delivary = delivary,
            DocumentType = (int)documentType
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteDeliveryDocumentAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[DelivaryDocument]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<DeliveryDocument?> GetDeliveryDocumentByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Name], 
                            [Url], 
                            [Delivary], 
                            [DocumentType]
                            FROM [dbo].[DelivaryDocument]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<DeliveryDocument>(query, new
        {
            Id = id
        });
    }
}

public enum DeliveryDocumentType : byte
{
    // Add your document types here
    Undefined = 0,
    // Add other types as needed
}

public class DeliveryDocument
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Delivary { get; set; }
    public DeliveryDocumentType DocumentType { get; set; }
}
