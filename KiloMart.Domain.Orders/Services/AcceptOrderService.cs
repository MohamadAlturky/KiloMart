using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Delivery.Activity;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Services;

public static class AcceptOrderService
{

    public static async Task<Result<AcceptOrderResponseModel>> ProviderAccept(
        long orderId,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        int providerId = userPayLoad.Party;
        var response = new AcceptOrderResponseModel()
        {
            OrderId = orderId
        };

        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();

        SystemSettings? systemSettings = await Db.GetSystemSettingsByIdAsync(0, readConnection);

        if (systemSettings is null)
        {
            return Result<AcceptOrderResponseModel>.Fail(["System Settings Not Found"]);
        }
        var location = await Db.GetLocationByPartyAsync(providerId, readConnection);

        if (location is null)
        {
            return Result<AcceptOrderResponseModel>.Fail(["Provider Don't Have a Location Not Found"]);
        }
        if (location.Party != userPayLoad.Party)
        {
            return Result<AcceptOrderResponseModel>.Fail(["Location is not for this provider"]);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            Order? order = await OrdersDb.GetOrderByIdAsync(orderId, readConnection);

            if (order is null)
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Not Found"]);
            }

            var products = await OrdersDb.GetOrderProductByOrderIdAsync(orderId, readConnection);

            if (products.Count == 0)
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Is Empty !!-!!"]);
            }
            response.OrderProviderInformation = new OrderProviderInformation()
            {
                Location = location.Id,
                Order = orderId,
                Provider = providerId
            };

            response.OrderProviderInformation.Id = await OrdersDb.InsertOrderProviderInfoAsync(
                connection,
                response.OrderProviderInformation.Order,
                response.OrderProviderInformation.Provider,
                response.OrderProviderInformation.Location,
                transaction
            );
            var offers = await OrdersDb.GetProductOffersAsync(readConnection, products.Select(e =>
            new RequestedProductForAcceptOrder()
            {
                ProductId = e.Product,
                RequestedQuantity = e.Quantity
            }), providerId);

            foreach (var item in offers)
            {
                if (item.RequestedQuantity <= item.OfferQuantity)
                {
                    var orderOffer = new OrderProductOffer
                    {
                        Order = orderId,
                        ProductOffer = item.OfferId,
                        Quantity = item.RequestedQuantity,
                        UnitPrice = item.OfferPrice
                    };
                    orderOffer.Id = await OrdersDb.InsertOrderProductOfferAsync(connection,
                        orderOffer.Order,
                        orderOffer.ProductOffer,
                        orderOffer.UnitPrice,
                        orderOffer.Quantity,
                        transaction);

                    var newQuantity = item.OfferQuantity - item.RequestedQuantity;
                    await Db.UpdateProductOfferQuantityAsync(connection,
                        item.OfferId,
                        newQuantity,
                        transaction);

                    response.OrderOffers.Add(orderOffer);
                }
            }
            decimal itemsPrice = response.OrderOffers.Sum(o => o.UnitPrice * o.Quantity);
            decimal systemFee = systemSettings.SystemOrderFee;
            decimal deliveryFee = systemSettings.DeliveryOrderFee;
            decimal totalPrice = systemFee + deliveryFee + itemsPrice;

            if (totalPrice < systemSettings.MinOrderValue)
            {
                transaction.Rollback();
                return Result<AcceptOrderResponseModel>.Fail(["Total Price is less than the MinOrderValue that is configured by the admin"]);
            }


            await Db.InsertSystemActivityAsync(
                connection,
                DateTime.Now,
                systemSettings.SystemOrderFee,
                orderId,
                transaction);

            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.PREPARING,
                totalPrice,
                itemsPrice,
                systemFee,
                deliveryFee,
                order.TransactionId,
                order.Date,
                order.PaymentType,
                order.IsPaid,
                transaction);

            await OrdersDb.InsertOrderActivityAsync(connection,
                orderId,
                DateTime.Now,
                (byte)OrderActivityType.AcceptedByProvider,
                providerId,
                transaction);

            transaction.Commit();
            return Result<AcceptOrderResponseModel>.Ok(response);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<AcceptOrderResponseModel>.Fail([ex.Message]);
        }

    }
    public static async Task<Result<AcceptOrderResponseModel>> DeliveryAccept(
       long orderId,
       UserPayLoad userPayLoad,
       IDbFactory dbFactory)
    {
        int deliveryId = userPayLoad.Party;
        var response = new AcceptOrderResponseModel()
        {
            OrderId = orderId
        };

        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();

        SystemSettings? systemSettings = await Db.GetSystemSettingsByIdAsync(0, readConnection);

        if (systemSettings is null)
        {
            return Result<AcceptOrderResponseModel>.Fail(["System Settings Not Found"]);
        }
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var whereClause = "WHERE o.Id = @id";
            var parameters = new { id = orderId };

            // Get the orders
            OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(readConnection, whereClause, parameters);

            if (order is null)
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Not Found"]);
            }
            if (order.Delivery is not null)
            {
                return Result<AcceptOrderResponseModel>.Fail(["Some one took this order"]);
            }
            if (order.OrderStatus != ((byte)OrderStatus.PREPARING))
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Status is not PREPARING"]);
            }

            response.OrderDeliveryInformation = new OrderDeliveryInformation()
            {
                Order = orderId,
                Delivery = userPayLoad.Party
            };

            response.OrderDeliveryInformation.Id = await OrdersDb.InsertOrderDeliveryInfoAsync(
                connection,
                response.OrderDeliveryInformation.Order,
                response.OrderDeliveryInformation.Delivery,
                transaction
            );

            await OrdersDb.InsertOrderActivityAsync(connection,
                orderId,
                DateTime.Now,
                (byte)OrderActivityType.AcceptedByDelivery,
                deliveryId,
                transaction);


            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.SHIPPED,
                order.TotalPrice,
                order.ItemsPrice,
                order.SystemFee,
                order.DeliveryFee,
                order.TransactionId,
                order.Date,
                order.PaymentType,
                order.IsPaid,
                transaction);

            transaction.Commit();
            return Result<AcceptOrderResponseModel>.Ok(response);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<AcceptOrderResponseModel>.Fail([ex.Message]);
        }

    }
}
public class AcceptOrderResponseModel
{
    public long OrderId { get; set; }
    public OrderProviderInformation? OrderProviderInformation { get; set; }
    public OrderDeliveryInformation? OrderDeliveryInformation { get; set; }
    public List<OrderProductOffer> OrderOffers { get; set; } = [];
}



