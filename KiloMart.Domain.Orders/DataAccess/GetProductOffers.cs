using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace KiloMart.Domain.Orders.DataAccess;

public partial class OrdersDb
{
    public static async Task<List<ProductOffer>> GetProductOffersAsync(
    IDbConnection connection,
    IEnumerable<RequestedProductForAcceptOrder> requestedProducts,
    int providerId)
    {
        // Create a DataTable to hold the product requests
        var productRequestTable = new DataTable();
        productRequestTable.Columns.Add("Product", typeof(int));
        productRequestTable.Columns.Add("Quantity", typeof(float));

        // Fill the DataTable with requested products
        foreach (var product in requestedProducts)
        {
            productRequestTable.Rows.Add(product.ProductId, product.RequestedQuantity);
        }

        // Create the temporary table in SQL Server
        var createTempTableQuery = @"
        CREATE TABLE #ProductRequest (
            Product INT,
            Quantity FLOAT
        );";

        await connection.ExecuteAsync(createTempTableQuery);

        // Use SqlBulkCopy to efficiently insert data into the temporary table
        using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection))
        {
            bulkCopy.DestinationTableName = "#ProductRequest";
            await bulkCopy.WriteToServerAsync(productRequestTable);
        }

        // Query to get active product offers based on the temporary table
        var offerQuery = @"
        SELECT 
            po.Id AS OfferId,
            po.Quantity AS OfferQuantity,
            po.Price AS OfferPrice,
            po.OffPercentage AS OfferOffPercentage,
            po.Product AS OfferProductId,
	        p.Quantity RequestedQuantity
        FROM 
            #ProductRequest p
        INNER JOIN 
            ProductOffer po ON p.Product = po.Product 
        WHERE 
            po.IsActive = 1 AND po.Provider = @ProviderId;";

        // Execute the offer query and return results
        var offers = await connection.QueryAsync<ProductOffer>(offerQuery, new { ProviderId = providerId });

        // Optionally drop the temporary table after use (not strictly necessary)
        await connection.ExecuteAsync("DROP TABLE #ProductRequest;");

        return offers.ToList();
    }
} 

// Define your ProductOffer class to match the query result structure
public class ProductOffer
{
    public int OfferId { get; set; }
    public float OfferQuantity { get; set; }
    public float RequestedQuantity { get; set; }
    public decimal OfferPrice { get; set; }
    public float OfferOffPercentage { get; set; }
    public int OfferProductId { get; set; }
}

// Define your RequestedProduct class as needed
public class RequestedProductForAcceptOrder
{
    public int ProductId { get; set; }
    public double RequestedQuantity { get; set; }
}