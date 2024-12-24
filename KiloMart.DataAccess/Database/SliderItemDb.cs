using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[SliderItem](
/// 	[Id] [int] IDENTITY(1,1) NOT NULL,
/// 	[ImageUrl] [varchar](100) NOT NULL,
/// 	[Target] [int] NULL)
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertSliderItemAsync(IDbConnection connection,
        string imageUrl,
        int? target,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[SliderItem]
                            ([ImageUrl], [Target])
                            VALUES (@ImageUrl, @Target)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            ImageUrl = imageUrl,
            Target = target
        }, transaction);
    }

    public static async Task<bool> UpdateSliderItemAsync(IDbConnection connection,
        int id,
        string imageUrl,
        int? target,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[SliderItem]
                                SET 
                                [ImageUrl] = @ImageUrl,
                                [Target] = @Target
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            ImageUrl = imageUrl,
            Target = target
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteSliderItemAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[SliderItem]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<SliderItem?> GetSliderItemByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [ImageUrl], 
                            [Target]
                            FROM [dbo].[SliderItem]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<SliderItem>(query, new
        {
            Id = id
        });
    }
    public static async Task<IEnumerable<SliderItem>> GetSliderItemListAsync(IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [ImageUrl], 
                            [Target]
                            FROM [dbo].[SliderItem]";

        return await connection.QueryAsync<SliderItem>(query);
    }
}

public class SliderItem
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int? Target { get; set; }
}
