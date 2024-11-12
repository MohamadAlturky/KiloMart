using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task<int> InsertDeliveryAsync(IDbConnection connection,
            int partyId,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[Delivery]
                                ([Party])
                                VALUES (@Party)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Party = partyId
            }, transaction);
        }

        public static async Task<bool> UpdateDeliveryAsync(IDbConnection connection,
            int id,
            int partyId,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[Delivery]
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

        public static async Task<bool> DeleteDeliveryAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[Delivery]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<Delivery?> GetDeliveryByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Party]
                                    FROM [dbo].[Delivery]
                                    WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<Delivery>(query, new
            {
                Id = id
            });
        }

        public static async Task<IEnumerable<Delivery>> GetDeliveriesAsync(IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Party]
                                    FROM [dbo].[Delivery]";

            return await connection.QueryAsync<Delivery>(query);
        }
    }
}

public class Delivery
{
    public int Id { get; set; }
    public int PartyId { get; set; }
}
