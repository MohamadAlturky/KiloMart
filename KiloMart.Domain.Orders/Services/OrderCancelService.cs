using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Services;

public class OrderCancelService
{
    public static async Task<Result<CancelOrderResponse>> CustomerCancel(
        long orderId,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();
        var whereClause = "WHERE Id = @orderId";
        OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(readConnection,
        whereClause,
        new
        {
            orderId = orderId
        });

        if (order is null)
        {
            return Result<CancelOrderResponse>.Fail(["Order Not Found"]);
        }
        if (order.Customer != userPayLoad.Party)
        {
            return Result<CancelOrderResponse>.Fail(["Un Authorized"]);
        }
        if (order.OrderStatus != (byte)OrderStatus.ORDER_PLACED)
        {
            return Result<CancelOrderResponse>.Fail(["Un Authorized :: can't cancel orders with status not equal to ORDER PLACED"]);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.CANCELED,
                order.TotalPrice,
                order.TransactionId,
                order.Date,
                order.PaymentType,
                transaction);

            await OrdersDb.InsertOrderActivityAsync(connection,
                orderId,
                DateTime.Now,
                (byte)OrderActivityType.CanceledByCustomerBeforeProviderAcceptIt,
                userPayLoad.Party,
                transaction);

            transaction.Commit();
            order.OrderStatus = (byte)OrderActivityType.CanceledByCustomerBeforeProviderAcceptIt;
            return Result<CancelOrderResponse>.Ok(new()
            {
                Order = order,
                Status = "Canceled By The Customer Before Any Provider Accepted It"
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<CancelOrderResponse>.Fail([ex.Message]);
        }
    }
    public static async Task<Result<CancelOrderResponse>> ProviderCancel(
        long orderId,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        int providerId = userPayLoad.Party;
        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();
        var whereClause = "WHERE Id = @orderId";
        OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(readConnection,
        whereClause,
        new
        {
            orderId = orderId
        });

        if (order is null)
        {
            return Result<CancelOrderResponse>.Fail(["Order Not Found"]);
        }
        if (order.Provider != userPayLoad.Party)
        {
            return Result<CancelOrderResponse>.Fail(["Un Authorized"]);
        }
        if (order.OrderStatus != (byte)OrderStatus.PREPARING)
        {
            return Result<CancelOrderResponse>.Fail(["Un Authorized :: can't cancel orders with status not equal to PREPARING"]);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.CANCELED,
                order.TotalPrice,
                order.TransactionId,
                order.Date,
                order.PaymentType,
                transaction);

            await OrdersDb.InsertOrderActivityAsync(connection,
                orderId,
                DateTime.Now,
                (byte)OrderActivityType.CanceledByProviderBeforeDeliveryAcceptIt,
                userPayLoad.Party,
                transaction);

            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            //return the offers
            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            var ordersOffers = await OrderRepository
            .GetAllOrderProductOffersByOrderIdAsync(
                connection,
                order.Id);

            foreach (var item in ordersOffers)
            {

                await Db.IncreaseProductOfferQuantityAsync(connection,
                    item.ProductOffer,
                    item.Quantity,
                    transaction);
            }

            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            transaction.Commit();
            order.OrderStatus = (byte)OrderActivityType.CanceledByProviderBeforeDeliveryAcceptIt;
            return Result<CancelOrderResponse>.Ok(new()
            {
                Order = order,
                Status = "Canceled By The Provider Before Any Delivery Accepted It"
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<CancelOrderResponse>.Fail([ex.Message]);
        }
    }
    public static async Task<Result<CancelOrderResponse>> DeliveryCancel(
        long orderId,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();
        var whereClause = "WHERE Id = @orderId";
        OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(readConnection,
        whereClause,
        new
        {
            orderId = orderId
        });

        if (order is null)
        {
            return Result<CancelOrderResponse>.Fail(["Order Not Found"]);
        }
        if (order.Provider != userPayLoad.Party)
        {
            return Result<CancelOrderResponse>.Fail(["Un Authorized"]);
        }
        if (!(order.OrderStatus == (byte)OrderStatus.SHIPPED || order.OrderStatus == (byte)OrderStatus.PREPARING))
        {
            return Result<CancelOrderResponse>.Fail(["Un Authorized :: can't cancel orders with status not equal to PREPARING or SHIPPED"]);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.CANCELED,
                order.TotalPrice,
                order.TransactionId,
                order.Date,
                order.PaymentType,
                transaction);

            await OrdersDb.InsertOrderActivityAsync(connection,
                orderId,
                DateTime.Now,
                (byte)OrderActivityType.CanceledByDelivery,
                userPayLoad.Party,
                transaction);

            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            //return the offers
            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            var ordersOffers = await OrderRepository
            .GetAllOrderProductOffersByOrderIdAsync(
                connection,
                order.Id);

            foreach (var item in ordersOffers)
            {

                await Db.IncreaseProductOfferQuantityAsync(connection,
                    item.ProductOffer,
                    item.Quantity,
                    transaction);
            }

            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////
            ////////////////////







            transaction.Commit();
            order.OrderStatus = (byte)OrderActivityType.CanceledByDelivery;
            return Result<CancelOrderResponse>.Ok(new()
            {
                Order = order,
                Status = "Canceled By The Delivery"
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<CancelOrderResponse>.Fail([ex.Message]);
        }
    }
}
public class CancelOrderResponse
{
    public OrderDetailsDto Order { get; set; } = null!;
    public string Status { get; set; } = null!;
}