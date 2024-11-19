using Dapper;
using System.Data;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<OrderRequestDto[]> GetOrderRequestsByCustomerAndStatus(
        IDbConnection connection,
        int customerId,
        int orderRequestStatus)
    {
        // SQL query to fetch order requests based on customer ID and status
        var sqlQuery = @"
        SELECT 
            [Id],
            [Customer],
            [Date],
            [OrderRequestStatus]
        FROM 
            OrderRequest
        WHERE 
            Customer = @Customer AND OrderRequestStatus = @OrderRequestStatus;";

        // Execute the query and map results to OrderRequestDto
        var orderRequests = await connection.QueryAsync<OrderRequestDto>(
            sqlQuery, new { Customer = customerId, OrderRequestStatus = orderRequestStatus });

        return orderRequests.ToArray();
    }
    public static async Task<OrderRequestDto?> GetOrderRequestsByIdAndStatus(
        IDbConnection connection,
        int Id,
        int orderRequestStatus)
    {
        // SQL query to fetch order requests based on customer ID and status
        var sqlQuery = @"
        SELECT 
            [Id],
            [Customer],
            [Date],
            [OrderRequestStatus]
        FROM 
            OrderRequest
        WHERE 
            Id = @Id AND OrderRequestStatus = @OrderRequestStatus;";

        // Execute the query and map results to OrderRequestDto
        var orderRequests = await connection.QueryFirstOrDefaultAsync<OrderRequestDto>(
            sqlQuery, new { Id = Id, OrderRequestStatus = orderRequestStatus });

        return orderRequests;
    }
}

// DTO class to hold the result of the query
public class OrderRequestDto
{
    public long Id { get; set; }
    public int Customer { get; set; }
    public DateTime Date { get; set; }
    public int OrderRequestStatus { get; set; }
}