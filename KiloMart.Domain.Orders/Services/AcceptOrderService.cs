using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;

namespace KiloMart.Domain.Orders.Services;

public static class AcceptOrderService
{

    public static async Task<Result<AcceptOrderResponseModel>> Accept(
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
            var offers = await OrdersDb.GetProductOffersAsync(readConnection, products.Select(e => new RequestedProductForAcceptOrder()
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
                        ProductOffer = item.OfferProductId,
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
            decimal totalPrice = response.OrderOffers.Sum(o => o.UnitPrice * o.Quantity);
            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.PREPARING,
                totalPrice,
                order.TransactionId,
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
}
public class AcceptOrderResponseModel
{
    public long OrderId { get; set; }
    public OrderProviderInformation OrderProviderInformation { get; set; } = null!;
    public List<OrderProductOffer> OrderOffers { get; set; } = [];
}



