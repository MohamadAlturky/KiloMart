using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Queries;
using KiloMart.Domain.Orders.Repositories;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{

    [HttpGet("details")]
    public async Task<IActionResult> Details(
        [FromQuery] long orderId,
        [FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        
        var whereClause = "WHERE o.Id = @id";
        var parameters = new { id = orderId };

        OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(connection,whereClause,parameters);
        if (order is null)
        {
            return DataNotFound("order not found");
        }
        var activities = await OrderRepository.GetOrderActivitiesAsync(connection,orderId);
        var products = await OrderRepository.GetOrderProductOffersAsync(connection,orderId,language);
        var requestedProducts = await OrderRepository.GetOrderProductsAsync(connection,orderId,language);
        return Success(new 
        {
            order,
            activities,
            requestedProducts,
            products
        });

    }
    // [HttpGet]
    // public async Task<IActionResult> GetOrderById(
    //     [FromQuery] byte status)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await OrdersQuery.GetOrdersByStatus(status, connection);
    //     return Success(result);
    // }
    // [HttpPost("cancel")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> Cancel([FromBody] long id)
    // {
    //     var result = await OrderRequestService.Cancel(_dbFactory,
    //         _userContext.Get(),
    //         id);
    //     if (result.Success)
    //     {
    //         return Ok(new
    //         {
    //             Message = "order canceled successfully",
    //             Order = result.Data
    //         });
    //     }
    //     return StatusCode(500, result.Errors);
    // }

    // [HttpGet("pending-order")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> Mine([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();
    //     List<PendingOrder> orders = [];
    //     var result = await Query.GetOrderRequestsByCustomerAndStatus(
    //         connection,
    //         _userContext.Get().Party,
    //         (byte)OrderRequestStatus.Init);

    //     foreach (var item in result)
    //     {
    //         PendingOrder order = new()
    //         {
    //             OrderInformation = item
    //         };
    //         var items = await Query.GetOrderRequestItemsByOrderRequest(connection, item.Id, language);
    //         order.Items = items;
    //         orders.Add(order);
    //     }
    //     return Ok(orders);
    // }
}
// public class PendingOrder
// {
//     public OrderRequestDto OrderInformation { get; set; } = null!;
//     public OrderRequestItemDto[] Items { get; set; } = null!;
// }