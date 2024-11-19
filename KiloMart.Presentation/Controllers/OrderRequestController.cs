using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.OrderRequests;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers;

[ApiController]
[Route("api/customer/order-request")]
public class OrderRequestController(IDbFactory dbFactory, IUserContext userContext) 
: AppController(dbFactory, userContext)
{
    [HttpPost]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> CreateOrderRequest(
        [FromBody] CreateOrderRequestApiRequestModel model)
    {
        if (model is null)
        {
            return BadRequest("Invalid request model.");
        }

        var userPayload = _userContext.Get();
        
        var result = await OrderRequestService.Insert(model, userPayload, _dbFactory);

        if (result.Success)
        {
            return CreatedAtAction(nameof(CreateOrderRequest), result.Data);
        }
        return BadRequest(result.Errors);
    }

        [HttpGet("pending-order")]
        [Guard([Roles.Customer])]
        public async Task<IActionResult> Mine([FromQuery] byte language)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            List<PendingOrder> orders = [];
            var result = await Query.GetOrderRequestsByCustomerAndStatus(
                connection,
                _userContext.Get().Party,
                (byte)OrderRequestStatus.Init);

            foreach(var item in result)
            {
            PendingOrder order = new()
            {
                OrderInformation = item
            };
            var items = await Query.GetOrderRequestItemsByOrderRequest(connection,item.Id,language);
                order.Items = items;
                orders.Add(order);
            }
            return Ok(orders);
        }
}
public class PendingOrder
{
    public OrderRequestDto OrderInformation {get;set;}
    public OrderRequestItemDto[] Items {get;set;}
}