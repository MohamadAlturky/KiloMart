using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Database;
public static partial class Db
{
    public static async Task<int> InsertProductLocalizedAsync(IDbConnection connection,
        int language,
        int product,
        string measurementUnit,
        string description,
        string name,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProductLocalized]
                            ([Language], [Product], [MeasurementUnit], [Description], [Name])
                            VALUES (@Language, @Product, @MeasurementUnit, @Description, @Name)
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        // However the above SQL Insert will not get the last identity inserted.
        // There is no SCOPE_IDENTITY so we use OUTPUT.
        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Language = language,
            Product = product,
            MeasurementUnit = measurementUnit,
            Description = description,
            Name = name
        }, transaction);
    }

    public static async Task<bool> UpdateProductLocalizedAsync(IDbConnection connection,
        int language,
        int product,
        string measurementUnit,
        string description,
        string name,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProductLocalized]
                                SET 
                                [MeasurementUnit] = @MeasurementUnit,
                                [Description] = @Description,
                                [Name] = @Name
                                WHERE [Language] = @Language AND [Product] = @Product";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Language = language,
            Product = product,
            MeasurementUnit = measurementUnit,
            Description = description,
            Name = name
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProductLocalizedAsync(IDbConnection connection, int language, int product, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProductLocalized]
                                WHERE [Language] = @Language AND [Product] = @Product";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Language = language,
            Product = product
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProductLocalized?> GetProductLocalizedByLanguageAndProductAsync(int language, int product, IDbConnection connection)
    {
        const string query = @"SELECT 
                                [Language],
                                [Product], 
                                [MeasurementUnit], 
                                [Description], 
                                [Name]
                                FROM [dbo].[ProductLocalized]
                                WHERE [Language] = @Language AND [Product] = @Product";

        return await connection.QueryFirstOrDefaultAsync<ProductLocalized>(query, new
        {
            Language = language,
            Product = product
        });
    }
}

public class ProductLocalized
{
    public int Language { get; set; }
    public int Product { get; set; }
    public string MeasurementUnit { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Name { get; set; } = null!;
}
