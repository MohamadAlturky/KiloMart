using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for Favorite Products
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertFavoriteProductAsync(IDbConnection connection,
        int customer,
        int product,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[FavoriteProducts]
                            ([Customer], [Product])
                            VALUES (@Customer, @Product)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Customer = customer,
            Product = product
        }, transaction);
    }

    public static async Task<bool> DeleteFavoriteProductAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[FavoriteProducts]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<IEnumerable<FavoriteProduct>> GetFavoriteProductsByCustomerIdAsync(int customerId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Customer], 
                            [Product]
                            FROM [dbo].[FavoriteProducts]
                            WHERE [Customer] = @CustomerId";

        return await connection.QueryAsync<FavoriteProduct>(query, new
        {
            CustomerId = customerId
        });
    }
    public static async Task<FavoriteProduct?> GetFavoriteProductsByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Customer], 
                            [Product]
                            FROM [dbo].[FavoriteProducts]
                            WHERE [Id] = @id";

        return await connection.QueryFirstOrDefaultAsync<FavoriteProduct>(query, new
        {
            id = id
        });
    }
}

public class FavoriteProduct
{
    public long Id { get; set; }
    public int Customer { get; set; }
    public int Product { get; set; }
}