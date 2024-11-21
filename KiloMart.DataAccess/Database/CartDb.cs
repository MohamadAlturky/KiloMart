using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for Cart
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertCartAsync(IDbConnection connection,
        int product,
        float quantity,
        int customer,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Cart]
                            ([Product], [Quantity], [Customer])
                            VALUES (@Product, @Quantity, @Customer)
                             SELECT CAST(SCOPE_IDENTITY() AS INT)";

        var id = await connection.ExecuteScalarAsync<long>(query, new
        {
            Product = product,
            Quantity = quantity,
            Customer = customer
        }, transaction);

        return id; // Return the ID of the inserted cart item
    }

    public static async Task<bool> UpdateCartAsync(IDbConnection connection,
        long id,
        int product,
        float quantity,
        int customer,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Cart]
                                SET 
                                [Product] = @Product,
                                [Quantity] = @Quantity,
                                [Customer] = @Customer
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Product = product,
            Quantity = quantity,
            Customer = customer
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteCartAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Cart]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Cart?> GetCartByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Product], 
                            [Quantity], 
                            [Customer]
                            FROM [dbo].[Cart]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Cart>(query, new
        {
            Id = id
        });
    }
     public static async Task<IEnumerable<Cart>> GetCartsByCustomerAsync(int customerId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Product], 
                            [Quantity], 
                            [Customer]
                            FROM [dbo].[Cart]
                            WHERE [Customer] = @CustomerId";

        return await connection.QueryAsync<Cart>(query, new
        {
            CustomerId = customerId
        });
    }

    public static async Task<bool> DeleteAllCartsByCustomerAsync(IDbConnection connection, int customerId, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Cart]
                                WHERE [Customer] = @CustomerId";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            CustomerId = customerId
        }, transaction);

        return deletedRowsCount > 0;
    }
}

public class Cart
{
    public long Id { get; set; }
    public int Product { get; set; }
    public float Quantity { get; set; }
    public int Customer { get; set; }
}