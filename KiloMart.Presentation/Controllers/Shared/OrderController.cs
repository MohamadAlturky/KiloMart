using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.Repositories;
using KiloMart.Domain.Register.Utils;
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

        OrderDetailsDto? order =
            await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(connection, whereClause, parameters);
        if (order is null)
        {
            return DataNotFound("order not found");
        }

        var activities = await OrderRepository.GetOrderActivitiesAsync(connection, orderId);
        var products = await OrderRepository.GetOrderProductOffersAsync(connection, orderId, language);
        var requestedProducts = await OrderRepository.GetOrderProductsAsync(connection, orderId, language);
        foreach (var item in requestedProducts)
        {
            var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (product is not null)
            {
                item.ActualUnitPrice = product.UnitPrice;
            }
        }

        if (_userContext.Get().Role == (byte)Roles.Provider
            && order.OrderStatus == (byte)OrderStatus.ORDER_PLACED)
        {
            var systemSettings = await Db.GetSystemSettingsByIdAsync(0, connection);
            if (systemSettings is null)
            {
                return DataNotFound("system settings not found");
            }

            order.SystemFee = systemSettings.SystemOrderFee;
            order.DeliveryFee = systemSettings.DeliveryOrderFee;
            DriverFreeFee? driverFreeFee = await Db.GetActiveDriverFreeFeesAsync(connection);

            if (driverFreeFee is not null)
            {
                order.DeliveryFee = 0;
            }

            var sql = @"
            SELECT [Id]
                    ,[Product]
                    ,[Price]
                    ,[OffPercentage]
                    ,[FromDate]
                    ,[ToDate]
                    ,[Quantity]
                    ,[Provider]
                    ,[IsActive]
                FROM [dbo].[ProductOffer]
                WHERE
                [Provider] = @providerId AND [IsActive] = 1";
            var has = new List<OrderProductOfferDetailsDto>();
            var provideroffers =
                await connection.QueryAsync<ProductOffer>(sql, new { providerId = _userContext.Get().Party });
            if (provideroffers is not null)
            {
                foreach (var item in requestedProducts)
                {
                    var product = provideroffers.FirstOrDefault(p => p.Product == item.ProductId);
                    if (product is not null)
                    {
                        item.ActualUnitPrice = product.Price;
                        has.Add(
                            new OrderProductOfferDetailsDto
                            {
                                Id = 0,
                                Order = item.ItemOrder,
                                ProductOffer = product.Id,
                                Quantity = item.ItemQuantity,
                                UnitPrice = product.Price,
                                ProductId = item.ProductId,
                                ProductImageUrl = item.ProductImageUrl,
                                ProductIsActive = item.ProductIsActive,
                                ProductMeasurementUnit = item.ProductMeasurementUnit,
                                ProductDescription = item.ProductDescription,
                                ProductName = item.ProductName,
                                ProductCategoryId = item.ProductCategoryId,
                                ProductCategoryIsActive = item.ProductCategoryIsActive,
                                ProductCategoryName = item.ProductCategoryName,
                                DealId = item.DealId,
                                DealEndDate = item.DealEndDate,
                                DealStartDate = item.DealStartDate,
                                DealIsActive = item.DealIsActive,
                                DealOffPercentage = item.DealOffPercentage
                            }
                        );
                    }
                    order.ItemsPrice = 0;
                    order.TotalPrice = 0;
                    var fees = order.SystemFee + order.DeliveryFee;
                    products = has;
                    if(requestedProducts.Any())
                    {
                        var total = requestedProducts.Sum(p => p.ItemQuantity
                            * p.ActualUnitPrice
                            * (p.DealOffPercentage ?? 100) / 100);
                        order.ItemsPrice = total ?? order.TotalPrice;
                    }
                    order.TotalPrice = order.ItemsPrice + fees;
                }
            }
        }

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