using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries
{
    public partial class Query
    {

        public static async Task<ProductPriceInfo[]> GetProductPriceRangeForCustomer(IDbConnection connection, int customerId)
        {
            var query = @"
                SELECT 
                    Product,
                    MIN(Price) AS MinPrice,
                    MAX(Price) AS MaxPrice 
                FROM ProductOffer 
                WHERE IsActive = 1 
                AND Product IN (SELECT Product FROM Cart WHERE Customer = @customer)
                GROUP BY Product;";

            var priceInfo = await connection.QueryAsync<ProductPriceInfo>(
                query,
                new { customer = customerId });

            return priceInfo.ToArray();
        }
    }

    public class ProductPriceInfo
    {
        public string Product { get; set; } = null!;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }

}