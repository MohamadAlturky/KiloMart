using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace KiloMart.Domain.Orders.DataAccess;

public partial class OrdersDb
{
    public static async Task<List<ProductOffer>> GetProductOffersAsync(
    IDbConnection connection,
    IEnumerable<RequestedProductForAcceptOrder> requestedProducts,
    int providerId,
    byte language = 1)
    {
        // Create a DataTable to hold the product requests
        var productRequestTable = new DataTable();
        productRequestTable.Columns.Add("Product", typeof(int));
        productRequestTable.Columns.Add("Quantity", typeof(decimal));

        // Fill the DataTable with requested products
        foreach (var product in requestedProducts)
        {
            productRequestTable.Rows.Add(product.ProductId, product.RequestedQuantity);
        }

        // Create the temporary table in SQL Server
        var createTempTableQuery = @"
        CREATE TABLE #ProductRequest (
            Product INT,
            Quantity decimal
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
	        p.Quantity RequestedQuantity,
            
            pd.[ProductId],
            pd.[ProductImageUrl],
            pd.[ProductIsActive],
            pd.[ProductMeasurementUnit],
            pd.[ProductDescription],
            pd.[ProductName],
            pd.[ProductCategoryId],
            pd.[ProductCategoryIsActive],
            pd.[ProductCategoryName],
            pd.[DealId],
            pd.[DealEndDate],
            pd.[DealStartDate],
            pd.[DealIsActive],
            pd.[DealOffPercentage]
        FROM 
            #ProductRequest p
        INNER JOIN 
            ProductOffer po ON p.Product = po.Product 
        INNER JOIN 
            dbo.GetProductDetailsFN(@language) pd ON po.Product = pd.ProductId
        WHERE 
            po.IsActive = 1 AND po.Provider = @ProviderId;";

        // Execute the offer query and return results
        var offers = await connection.QueryAsync<ProductOffer>(offerQuery,
        new
        {
            ProviderId = providerId,
            language = language
        });

        // Optionally drop the temporary table after use (not strictly necessary)
        await connection.ExecuteAsync("DROP TABLE #ProductRequest;");

        return offers.ToList();
    }
}

// Define your ProductOffer class to match the query result structure
public class ProductOffer
{
    public int OfferId { get; set; }
    public decimal OfferQuantity { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal OfferPrice { get; set; }
    public decimal OfferOffPercentage { get; set; }
    public int OfferProductId { get; set; }
    ///////
    ///////
    ///////
    ///////
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductCategoryId { get; set; }
    public bool ProductCategoryIsActive { get; set; }
    public string ProductCategoryName { get; set; } = null!;
    public int? DealId { get; set; }
    public DateTime? DealEndDate { get; set; }
    public DateTime? DealStartDate { get; set; }
    public bool? DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }
}

// Define your RequestedProduct class as needed
public class RequestedProductForAcceptOrder
{
    public int ProductId { get; set; }
    public decimal RequestedQuantity { get; set; }
}