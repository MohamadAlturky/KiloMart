using System.Data;
using Dapper;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[ProductCategoryLocalized](
/// [Name] [varchar](200) NOT NULL,
/// [Language] [tinyint] NOT NULL,
/// [ProductCategory] [int] NOT NULL)
/// </summary>

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task InsertProductCategoryLocalizedAsync(IDbConnection connection,
        string name,
        byte language,
        int productCategory,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProductCategoryLocalized]
                            ([Name], [Language], [ProductCategory])
                            VALUES (@Name, @Language, @ProductCategory);";

        await connection.ExecuteScalarAsync(query, new
        {
            Name = name,
            Language = language,
            ProductCategory = productCategory
        }, transaction);
    }

    public static async Task<bool> UpdateProductCategoryLocalizedAsync(IDbConnection connection,
        string name,
        byte language,
        int productCategory,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProductCategoryLocalized]
                                SET 
                                [Name] = @Name,
                                [Language] = @Language
                                WHERE [ProductCategory] = @ProductCategory AND [Language] = @Language";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Name = name,
            Language = language,
            ProductCategory = productCategory
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProductCategoryLocalizedAsync(IDbConnection connection, int productCategory, byte language, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProductCategoryLocalized]
                                WHERE [ProductCategory] = @ProductCategory AND [Language] = @Language";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            ProductCategory = productCategory,
            Language = language
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProductCategoryLocalized?> GetProductCategoryLocalizedAsync(int productCategory, byte language, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Name], 
                            [Language], 
                            [ProductCategory]
                            FROM [dbo].[ProductCategoryLocalized]
                            WHERE [ProductCategory] = @ProductCategory AND [Language] = @Language";

        return await connection.QueryFirstOrDefaultAsync<ProductCategoryLocalized>(query, new
        {
            ProductCategory = productCategory,
            Language = language
        });
    }

    public static async Task<IEnumerable<ProductCategoryLocalized>> GetAllProductCategoryLocalizedAsync(IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Name], 
                            [Language], 
                            [ProductCategory]
                            FROM [dbo].[ProductCategoryLocalized]";

        return await connection.QueryAsync<ProductCategoryLocalized>(query);
    }
}

public class ProductCategoryLocalized
{
    public string Name { get; set; } = null!;
    public byte Language { get; set; }
    public int ProductCategory { get; set; }
}
