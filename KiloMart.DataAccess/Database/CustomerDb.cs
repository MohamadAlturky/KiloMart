using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task<int> InsertCustomerAsync(IDbConnection connection,
         int party, 
         IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[Customer]
                                ([Party])
                                VALUES (@Party)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Party = party
            }, transaction);
        }

        public static async Task<bool> UpdateCustomerAsync(IDbConnection connection, int id, int party, IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[Customer]
                                    SET 
                                    [Party] = @Party
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Party = party
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteCustomerAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[Customer]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<Customer?> GetCustomerByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Party]
                                    FROM [dbo].[Customer]
                                    WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<Customer>(query, new
            {
                Id = id
            });
        }

        public static async Task<IEnumerable<Customer>> GetCustomersAsync(IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Party]
                                    FROM [dbo].[Customer]";

            return await connection.QueryAsync<Customer>(query);
        }
    }
}

public class Customer
{
    public int Id { get; set; }
    public int Party { get; set; }
}
