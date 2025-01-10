using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{

    public static async Task<int> GetActiveProductOffersCount(IDbConnection connection)
    {
        const string query = "SELECT COUNT([Id]) AS TotalCount FROM dbo.[ProductOffer] WHERE IsActive = 1";
        return await connection.ExecuteScalarAsync<int>(query);
    }
}
