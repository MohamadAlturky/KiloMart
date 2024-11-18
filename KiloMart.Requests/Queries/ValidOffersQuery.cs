using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiloMart.Requests.Queries
{
    public partial class Query
    {
        public static async Task<ProductOfferCount[]> GetProductOfferCounts(
            IDbConnection connection,
            IEnumerable<RequestedProduct> requestedProducts,
            float Latitude,
            float Longitude)
        {
            // Create a DataTable to hold requested products
            var requestedProductsTable = new DataTable();
            requestedProductsTable.Columns.Add("ProductId", typeof(int));
            requestedProductsTable.Columns.Add("RequestedQuantity", typeof(int));

            // Fill the DataTable with requested products
            foreach (var product in requestedProducts)
            {
                requestedProductsTable.Rows.Add(product.ProductId, product.RequestedQuantity);
            }

            // Create a temporary table in SQL Server and use a table-valued parameter
            var createTempTableQuery = @"
                CREATE TABLE #RequestedProducts (
                    ProductId INT,
                    RequestedQuantity INT
                );";

            // Execute create temp table command
            await connection.ExecuteAsync(createTempTableQuery);

            // Use SqlBulkCopy to efficiently insert data into the temporary table
            using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection))
            {
                bulkCopy.DestinationTableName = "#RequestedProducts";
                await bulkCopy.WriteToServerAsync(requestedProductsTable);
            }

            // Query to count active product offers grouped by provider and calculate distance
            var countQuery = @"
                SELECT 
                    ProductOffer.Provider,
                    COUNT(*) AS Count,
                    SQRT(SQUARE(Location.Latitude - @Latitude) + SQUARE(Location.Longitude - @Longitude)) AS Distance 
                FROM ProductOffer WITH (NOLOCK)
                INNER JOIN Product WITH (NOLOCK) ON Product.Id = ProductOffer.Product
                INNER JOIN #RequestedProducts pr WITH (NOLOCK) ON pr.ProductId = Product.Id
                INNER JOIN Location WITH (NOLOCK) ON Provider = Location.Party
                WHERE ProductOffer.IsActive = 1 
                  AND ProductOffer.Quantity >= pr.RequestedQuantity
                GROUP BY ProductOffer.Provider, Location.Latitude, Location.Longitude;";

            // Execute count query and return results
            var result = await connection.QueryAsync<ProductOfferCount>(countQuery, new
            {
                Longitude,
                Latitude
            });

            // Optionally drop the temporary table after use (not strictly necessary)
            await connection.ExecuteAsync("DROP TABLE #RequestedProducts;");

            return result.ToArray();
        }
    }

    // Ensure this class has properties for Provider, Count, and Distance.
    public class ProductOfferCount
    {
        public int Provider { get; set; }
        public int Count { get; set; }
        public double Distance { get; set; } // Add this property for distance calculation.
    }
}
public class RequestedProduct
{
    public int ProductId { get; set; }
    public int RequestedQuantity { get; set; }
}