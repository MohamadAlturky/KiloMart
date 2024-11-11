using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    /// <summary>
    /// Inserts a new ProductCategory into the database.
    /// </summary>
    /// <param name="connection">The database connection to use.</param>
    /// <param name="name">The name of the category.</param>
    /// <param name="transaction">The transaction to execute the query under.</param>
    /// <returns>The id of the newly inserted category.</returns>
    public static async Task<int> InsertProductCategoryAsync(IDbConnection connection, string name, IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProductCategory]
                                ([IsActive], [Name])
                                VALUES (@IsActive, @Name)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            IsActive = true,
            Name = name
        }, transaction);
    }

    /// <summary>
    /// Updates an existing ProductCategory in the database.
    /// </summary>
    /// <param name="connection">The database connection to use.</param>
    /// <param name="id">The id of the category to update.</param>
    /// <param name="name">The new name of the category.</param>
    /// <param name="isActive">Whether the category is active or not.</param>
    /// <param name="transaction">The transaction to execute the query under.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    public static async Task<bool> UpdateProductCategoryAsync(IDbConnection connection, int id, string name, bool isActive, IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProductCategory]
                                SET 
                                [IsActive] = @IsActive,
                                [Name] = @Name
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            IsActive = isActive,
            Name = name
        }, transaction);

        return updatedRowsCount > 0;
    }

    /// <summary>
    /// Deletes a ProductCategory from the database.
    /// </summary>
    /// <param name="connection">The database connection to use.</param>
    /// <param name="id">The id of the category to delete.</param>
    /// <param name="transaction">The transaction to execute the query under.</param>
    /// <returns>True if the delete was successful, false otherwise.</returns>
    public static async Task<bool> DeleteProductCategoryAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProductCategory]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    /// <summary>
    /// Retrieves a ProductCategory by id from the database.
    /// </summary>
    /// <param name="id">The id of the category to retrieve.</param>
    /// <param name="connection">The database connection to use.</param>
    /// <returns>The retrieved ProductCategory or null if none was found.</returns>
    public static async Task<ProductCategory?> GetProductCategoryByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                                [Id], 
                                [IsActive], 
                                [Name]
                                FROM [dbo].[ProductCategory]
                                WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProductCategory>(query, new
        {
            Id = id
        });
    }
}

public class ProductCategory
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = null!;
}
