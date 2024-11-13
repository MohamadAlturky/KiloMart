using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task<int> InsertProviderAsync(IDbConnection connection,
            int partyId,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[Provider]
                                ([Party])
                                VALUES (@Party)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Party = partyId
            }, transaction);
        }

        public static async Task<bool> UpdateProviderAsync(IDbConnection connection,
            int id,
            int partyId,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[Provider]
                                    SET 
                                    [Party] = @Party
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Party = partyId
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteProviderAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[Provider]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<Provider?> GetProviderByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Party]
                                    FROM [dbo].[Provider]
                                    WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<Provider>(query, new
            {
                Id = id
            });
        }

        public static async Task<IEnumerable<Provider>> GetProvidersAsync(IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Party]
                                    FROM [dbo].[Provider]";

            return await connection.QueryAsync<Provider>(query);
        }
    }
}

public class Provider
{
    public int Id { get; set; }
    public int PartyId { get; set; }
}
