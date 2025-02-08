using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Database;


public static partial class Db
{
    public static async Task<bool> ActivateProductAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Product]
                            SET [IsActive] = 1
                            WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeactivateProductAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Product]
                            SET [IsActive] = 0
                            WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> UpdateProductAsync(
        IDbConnection connection,
        int id,
        string name,
        string description,
        string imageUrl,
        int productCategory,
        string measurementUnit,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Product]
                            SET [Name] = @Name,
                                [Description] = @Description,
                                [ImageUrl] = @ImageUrl,
                                [ProductCategory] = @ProductCategory,
                                [MeasurementUnit] = @MeasurementUnit,
                                [IsActive] = @IsActive
                            WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            ProductCategory = productCategory,
            MeasurementUnit = measurementUnit
        }, transaction);

        return updatedRowsCount > 0;
    }

    // product localized

    public static async Task<bool> InsertProductLocalizedAsync(
       IDbConnection connection,
       string language,
       int productId,
       string measurementUnit,
       string description,
       string name,
       IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProductLocalized] 
                                ([Language], [Product], [MeasurementUnit], [Description], [Name])
                              VALUES 
                                (@Language, @Product, @MeasurementUnit, @Description, @Name)";

        var insertedRowsCount = await connection.ExecuteAsync(query, new
        {
            Language = language,
            Product = productId,
            MeasurementUnit = measurementUnit,
            Description = description,
            Name = name
        }, transaction);

        return insertedRowsCount > 0;
    }

    public static async Task<bool> UpdateProductLocalizedAsync(
        IDbConnection connection,
        string language,
        int productId,
        string measurementUnit,
        string description,
        string name,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProductLocalized]
                              SET [MeasurementUnit] = @MeasurementUnit,
                                  [Description] = @Description,
                                  [Name] = @Name
                              WHERE [Language] = @Language AND [Product] = @Product";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Language = language,
            Product = productId,
            MeasurementUnit = measurementUnit,
            Description = description,
            Name = name
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProductLocalizedAsync(
        IDbConnection connection,
        string language,
        int productId,
        IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProductLocalized]
                              WHERE [Language] = @Language AND [Product] = @Product";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Language = language,
            Product = productId
        }, transaction);

        return deletedRowsCount > 0;
    }
    public static async Task<ProductLocalizedData?> GetProductLocalizedAsync(
            IDbConnection connection,
            int language,
            int productId,
            IDbTransaction? transaction = null)
    {
        const string query = @"SELECT [Language], [Product], [MeasurementUnit], [Description], [Name] 
                              FROM [dbo].[ProductLocalized]
                              WHERE [Language] = @Language AND [Product] = @Product";

        return await connection.QuerySingleOrDefaultAsync<ProductLocalizedData>(query, new
        {
            Language = language,
            Product = productId
        }, transaction);
    }
    public static async Task<IEnumerable<ProductLocalizedData>> GetProductLocalizedValuesAsync(
        IDbConnection connection,
        int productId,
        IDbTransaction? transaction = null)
    {
        const string query = @"SELECT [Language], [Product], [MeasurementUnit], [Description], [Name] 
                              FROM [dbo].[ProductLocalized]
                              WHERE [Product] = @Product";

        return await connection.QueryAsync<ProductLocalizedData>(query, new
        {
            Product = productId
        }, transaction);
    }
}
public class ProductLocalizedData
{
    public string Language { get; set; } = string.Empty;
    public int Product { get; set; }
    public string MeasurementUnit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
