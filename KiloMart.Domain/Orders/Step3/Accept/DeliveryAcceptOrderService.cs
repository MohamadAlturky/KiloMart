using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Shared;

namespace KiloMart.Domain.Orders.Step3.Accept;
public static class DeliveryAcceptOrderService
{
    public static async Task<Result<bool>> Accept(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        long id)
    {
        // insert to db
        using var connection = dbFactory.CreateDbConnection();
        using var readConnection = dbFactory.CreateDbConnection();
        connection.Open();
        readConnection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var order = await Db.GetOrderByIdAsync(id, readConnection);
            if (order is null)
            {
                return Result<bool>.Fail(["Not Found"]);
            }
            await Db.UpdateOrderAsync
                (connection,
                id, (byte)OrderStatus.AcceptedFromDelivery,
                order.TotalPrice,
                order.TransactionId,
                order.CustomerLocation,
                order.ProviderLocation,
                order.Customer,
                order.Provider,
                transaction);

            OrderActivity orderActivity = new()
            {
                Date = DateTime.Now,
                OperatedBy = userPayLoad.Party,
                Order = id,
                OrderActivityType = (byte)OrderActivityType.AcceptedFromDelivary
            };

            orderActivity.Id = await Db.InsertOrderActivityAsync(connection,
                orderActivity.Order,
                orderActivity.Date,
                orderActivity.OrderActivityType,
                orderActivity.OperatedBy,
                transaction);

            transaction.Commit();
            return Result<bool>.Ok(true);
        }
        catch (Exception exception)
        {
            transaction.Rollback();
            return Result<bool>.Fail([exception.Message]);
        }
    }
}