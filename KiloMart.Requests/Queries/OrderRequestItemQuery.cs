using Dapper;
using System.Data;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<OrderRequestItemDto[]> GetOrderRequestItemsByOrderRequest(
        IDbConnection connection,
        long orderRequestId,
        byte language)
    {
        // SQL query to fetch order request items with product details
        var sqlQuery = @"
        SELECT 
            ort.Id AS OrderRequestItemId,
            ort.Quantity AS OrderRequestItemQuantity,
            p.Id AS ProductId,
            COALESCE(pl.Description, p.Description) AS ProductDescription,
            COALESCE(pl.Name, p.Name) AS ProductName,
            COALESCE(pl.MeasurementUnit, p.MeasurementUnit) AS ProductMeasurementUnit,
            p.ImageUrl AS ProductImageUrl
        FROM 
            OrderRequestItem ort
        INNER JOIN 
            Product p ON p.Id = ort.Product
        LEFT JOIN 
            ProductLocalized pl ON p.Id = pl.Product AND pl.Language = @Language
        WHERE 
            ort.OrderRequest = @OrderRequest;";

        // Execute the query and map results to OrderRequestItemDto
        var orderRequestItems = await connection.QueryAsync<OrderRequestItemDto>(
            sqlQuery, new { OrderRequest = orderRequestId, Language = language });

        return orderRequestItems.ToArray();
    }
}

// DTO class to hold the result of the query
public class OrderRequestItemDto
{
    public long OrderRequestItemId { get; set; }
    public float OrderRequestItemQuantity { get; set; }
    public int ProductId { get; set; }
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
}