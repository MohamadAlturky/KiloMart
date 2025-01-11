using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{
    public class StatsResult
    {
        public long TotalProviders { get; set; }
        public long ActiveProviders { get; set; }
        public decimal TotalProvidersBalance { get; set; }
        public long TotalProductOffers { get; set; }
    }

    public static async Task<StatsResult> GetStatisticsAsync(IDbConnection connection)
    {
        const string query = @"
                SELECT 
                    (SELECT COUNT(Party) FROM dbo.Provider) AS TotalProviders,
                    (SELECT COUNT(p.Party) FROM dbo.Provider p INNER JOIN MemberShipUser m ON m.Party = p.Party) AS ActiveProviders,
                    (SELECT SUM(Value) FROM dbo.ProviderActivity) AS TotalProvidersBalance,
                    (SELECT COUNT(*) FROM dbo.ProductOffer WHERE IsActive = 1) AS TotalProductOffers;
            ";

        return await connection.QuerySingleAsync<StatsResult>(query);
    }
}
