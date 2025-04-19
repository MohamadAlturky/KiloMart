using EdfaPayApi.Core.Interfaces;
using EdfaPayApi.Core.Models;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.DateServices;
using KiloMart.Domain.Notifications;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Repositories;
using KiloMart.Presentation.RealTime;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace KiloMart.Domain.Orders.Services;

public static class AcceptOrderService
{

    public static async Task<Result<AcceptOrderResponseModel>> ProviderAccept(
        long orderId,
        UserPayLoad userPayLoad,
        IPaymentService paymentService,
        IConfiguration configuration,
        IDbFactory dbFactory,
        IHubContext<NotificationHub> hubContext)
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
        var orderDicountCode = await Db.GetOrderDiscountCodeByOrderIdAsync(orderId, readConnection);
        DiscountCode? discountCode = null;
        if (orderDicountCode is not null)
        {
            discountCode = await Db.GetDiscountCodeByIdAsync(orderDicountCode.DiscountCode, readConnection);
        }
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            Order? order = await OrdersDb.GetOrderByIdAsync(orderId, readConnection);
            DriverFreeFee? driverFreeFee = await Db.GetActiveDriverFreeFeesAsync(readConnection);
            if (order is null)
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Not Found"]);
            }
            if (order.OrderStatus != ((byte)OrderStatus.ORDER_PLACED))
            {
                return Result<AcceptOrderResponseModel>.Fail(["Some Provider Already Accepted This Order"]);
            }
            if (order.OrderStatus == ((byte)OrderStatus.PREPARING))
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order is Already Accepted"]);
            }
            var orderCustomer = await OrdersDb.GetOrderCustomerInfoByOrderIdAsync(orderId, readConnection);
            if (orderCustomer is null)
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Customer Not Found"]);
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
            decimal itemsPriceAfterOffPercentage = 0;

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
                    itemsPriceAfterOffPercentage +=
                    orderOffer.UnitPrice
                    * orderOffer.Quantity
                    * (item.DealOffPercentage ?? 100) / 100;
                }
            }
            decimal itemsPrice = itemsPriceAfterOffPercentage;//response.OrderOffers.Sum(o => o.UnitPrice * o.Quantity);
            decimal systemFee = systemSettings.SystemOrderFee;
            decimal deliveryFee = systemSettings.DeliveryOrderFee;
            decimal totalPrice = systemFee + deliveryFee + itemsPriceAfterOffPercentage;

            if (driverFreeFee is not null)
            {
                totalPrice -= deliveryFee;
                deliveryFee = 0;
            }
            if (totalPrice < systemSettings.MinOrderValue)
            {
                transaction.Rollback();
                return Result<AcceptOrderResponseModel>.Fail(["Total Price is less than the MinOrderValue that is configured by the admin"]);
            }

            if (discountCode is not null)
            {
                if (discountCode.DiscountType == ((byte)DiscountType.FIXED))
                {
                    totalPrice -= discountCode.Value;
                }
                else
                {
                    totalPrice = totalPrice * (100 - discountCode.Value) / 100;
                }
            }

            // await Db.InsertSystemActivityAsync(
            //     connection,
            //     SaudiDateTimeHelper.GetCurrentTime(),
            //     systemSettings.SystemOrderFee,
            //     orderId,
            //     transaction);

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
                order.SpecialRequest,
                transaction);

            await OrdersDb.InsertOrderActivityAsync(connection,
                orderId,
                SaudiDateTimeHelper.GetCurrentTime(),
                (byte)OrderActivityType.AcceptedByProvider,
                providerId,
                transaction);

            var deliveryProviderCircles = await ProviderCircleDb.GetDeliveryProviderCirclesAsync(connection, transaction);
            foreach (var deliveryProviderCircle in deliveryProviderCircles)
            {
                await NotificationsService.SendNotification(connection,
                    "New Order",
                    "Id # " + orderId,
                    deliveryProviderCircle.Id,
                    hubContext,
                    transaction);
            }
            await NotificationsService.SendNotification(connection,
                "Provider Accepted Order",
                "Provider # " + providerId + " accepted order # " + orderId,
                orderCustomer.Customer,
                hubContext,
                transaction);

            transaction.Commit();
            try
            {
                if (order.PaymentType == ((byte)PaymentType.Elcetronic))
                {
                    using var newConnection = dbFactory.CreateDbConnection();
                    newConnection.Open();
                    var customerOrderInformation = await OrdersDb.GetOrderCustomerInfoByOrderIdAsync(orderId, newConnection);
                    if (customerOrderInformation is null)
                    {
                        throw new Exception("Customer Order Information Not Found");
                    }
                    var card = await Db.GetIsPrimaryCardsByCustomerAsync(newConnection, customerOrderInformation.Customer);
                    if (card is null)
                    {
                        throw new Exception("Card Not Found");
                    }
                    var requestMini = new PaymentRequestMini()
                    {
                        CardNumber = card.Number,
                        CardExpYear = card.ExpireDate.Year.ToString(),
                        CardExpMonth = card.ExpireDate.Month.ToString(),
                        CardCvv2 = card.SecurityCode,
                        OrderAmount = totalPrice,
                        OrderDescription = $"Order {orderId}",
                        OrderId = orderId.ToString()
                    };
                    var request = requestMini.ToPaymentRequest();
                    // Generate hash
                    request.Hash = paymentService.GenerateHash(
                        request.PayerEmail,
                        request.CardNumber,
                        configuration["PaymentGateway:MerchantPassword"]!
                    );

                    var responseData = await paymentService.ProcessPaymentAsync(request);
                    if (responseData.status == "SUCCESS")
                    {
                        await OrdersDb.UpdateOrderIsPaidAsync(newConnection,
                            order.Id,
                            true);
                    }
                    if (responseData.status == "REDIRECT")
                    {
                        var obj = new
                        {
                            orderId,
                            responseData.redirect_url,
                            responseData.redirect_method,
                            responseData.redirect_params
                        };
                        await Db.InsertNotificationAsync(newConnection,
                            "Please Pay the Order",
                            $"Order {orderId} need to be paid, please pay it to continue using the payment url",
                            SaudiDateTimeHelper.GetCurrentTime(),
                            customerOrderInformation.Customer,
                            JsonConvert.SerializeObject(obj),
                            transaction);
                        await NotificationsService.SendNotification(newConnection,
                            "Please Pay the Order",
                            JsonConvert.SerializeObject(obj),
                            customerOrderInformation.Customer,
                            hubContext,
                            transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<AcceptOrderResponseModel>.Fail([ex.Message]);
            }
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
       IDbFactory dbFactory,
       IHubContext<NotificationHub> hubContext)
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
            if (order.OrderStatus == ((byte)OrderStatus.SHIPPED))
            {
                return Result<AcceptOrderResponseModel>.Fail(["Order Status is Already Accepted"]);
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
                SaudiDateTimeHelper.GetCurrentTime(),
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
                order.SpecialRequest,
                transaction);
            if (order.Customer.HasValue)
            {

                await NotificationsService.SendNotification(connection,
                    "Delivery Accepted Order",
                    "Delivery # " + deliveryId + " accepted order # " + orderId,
                    order.Customer.Value,
                    hubContext,
                    transaction);
            }
            if (order.Provider.HasValue)
            {
                await NotificationsService.SendNotification(connection,
                    "Delivery Accepted Order",
                    "Delivery # " + deliveryId + " accepted order # " + orderId,
                    order.Provider.Value,
                    hubContext,
                    transaction);
            }

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



public enum DiscountType
{
    FIXED = 1,
    PERCENTAGE = 2
}
