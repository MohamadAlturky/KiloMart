using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<PriceSummary?> GetPriceSummaryByCustomerAsync(IDbConnection connection, int customerId)
    {
        const string sql = @"
            SELECT SUM(MAX) AS MaxValue, SUM(MIN) AS MinValue FROM (
                SELECT p.MaxPrice * c.Quantity AS MAX, p.MinPrice * c.Quantity AS MIN
                FROM (
                    SELECT po.Product,
                        MAX(po.Price) AS MaxPrice,
                        MIN(po.Price) AS MinPrice 
                    FROM ProductOffer po
                    GROUP BY po.Product
                ) p 
                INNER JOIN Cart c
                ON c.Product = p.Product AND c.Customer = @customer
            ) DataTable";

        var parameters = new { customer = customerId };

        var result = await connection.QueryFirstOrDefaultAsync<PriceSummary?>(sql, parameters);

        return result;
    }
    public static async Task<ProductPriceInfo[]> GetProductPriceRangeForCustomer(IDbConnection connection, int customerId)
    {
        var query = @"
                SELECT 
                po.Product,
                d.OffPercentage OffPercentage,
                MIN(po.Price) AS MinPrice,
                MAX(po.Price) AS MaxPrice
            FROM dbo.[ProductOffer] po
            INNER JOIN dbo.[Deal] d ON po.Product = d.Product
            WHERE po.IsActive = 1 
            AND po.Product IN (SELECT Product FROM Cart WHERE Customer = @customer)
            GROUP BY po.Product, d.OffPercentage;";

        var priceInfo = await connection.QueryAsync<ProductPriceInfo>(
            query,
            new { customer = customerId });

        return priceInfo.ToArray();
    }
}

public class ProductPriceInfo
{
    public int Product { get; set; }
    public decimal OffPercentage { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}

public class PriceSummary
{
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
}
