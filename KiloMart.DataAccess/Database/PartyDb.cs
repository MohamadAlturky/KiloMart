using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task<int> InsertPartyAsync(IDbConnection connection,
            string displayName,
            bool isActive,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[Party]
                                ([DisplayName], [IsActive])
                                VALUES (@DisplayName, @IsActive)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                DisplayName = displayName,
                IsActive = isActive
            }, transaction);
        }

        public static async Task<bool> UpdatePartyAsync(IDbConnection connection,
            int id,
            string displayName,
            bool isActive,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[Party]
                                    SET 
                                    [DisplayName] = @DisplayName,
                                    [IsActive] = @IsActive
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                DisplayName = displayName,
                IsActive = isActive
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeletePartyAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[Party]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<Party?> GetPartyByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [DisplayName], 
                                    [IsActive]
                                    FROM [dbo].[Party]
                                    WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<Party>(query, new
            {
                Id = id
            });
        }

        public static async Task<IEnumerable<Party>> GetPartiesAsync(IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [DisplayName], 
                                    [IsActive]
                                    FROM [dbo].[Party]";

            return await connection.QueryAsync<Party>(query);
        }
    }
}

public class Party
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
}
