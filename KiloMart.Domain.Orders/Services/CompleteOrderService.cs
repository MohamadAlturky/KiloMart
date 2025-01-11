using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Delivery.Activity;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Services;

public class CompleteOrderService
{
    public static async Task<Result<CompleteOrderResponseModel>> CompleteOrder(
        CompleteOrderRequestModel model,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        // Validate the incoming request model
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<CompleteOrderResponseModel>.Fail(errors);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();


        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();
        SystemSettings? systemSettings = await Db.GetSystemSettingsByIdAsync(0, readConnection);

        if (systemSettings is null)
        {
            return Result<CompleteOrderResponseModel>.Fail(["System Settings Not Found"]);
        }
        try
        {
            // Fetch the existing order based on OrderId
            var whereClause = "WHERE o.Id = @id";
            var parameters = new { id = model.OrderId };
            OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(readConnection, whereClause, parameters);

            if (order is null)
            {
                return Result<CompleteOrderResponseModel>.Fail(["Order not found"]);
            }

            // Check if the order can be completed
            if (order.OrderStatus != (byte)OrderStatus.SHIPPED)
            {
                return Result<CompleteOrderResponseModel>.Fail(["Cannot complete an order that is not SHIPPED"]);
            }

            if (!order.IsPaid && (order.PaymentType == (byte)PaymentType.Elcetronic))
            {
                return Result<CompleteOrderResponseModel>.Fail(["the order is not paid"]);
            }

            #region Insert Delivery  And Provider Activity
            if (order.PaymentType == ((byte)PaymentType.Elcetronic))
            {
                await Db.InsertDeliveryActivityAsync(
                    connection,
                    DateTime.Now,
                    order.DeliveryFee,
                    (byte)DeliveryActivityType.Receives,
                    userPayLoad.Party,
                    transaction);
            }
            else
            {
                await Db.InsertDeliveryActivityAsync(
                    connection,
                    DateTime.Now,
                    order.TotalPrice - order.DeliveryFee,
                    (byte)DeliveryActivityType.Deductions,
                    userPayLoad.Party,
                    transaction);
            }

            if (!order.Provider.HasValue)
            {
                throw new Exception("Provider Not Found");
            }
            await Db.InsertProviderActivityAsync(
                connection,
                DateTime.Now,
                order.ItemsPrice,
                order.Provider.Value,
                (byte)DeliveryActivityType.Receives,
                transaction);


            #endregion

            // Update order status to completed
            order.OrderStatus = (byte)OrderStatus.COMPLETED;

            await OrdersDb.UpdateOrderAsync(connection,
                order.Id,
                (byte)OrderStatus.COMPLETED,
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
            // Log order activity
            OrderActivity activity = new()
            {
                Date = DateTime.Now,
                Order = order.Id,
                OperatedBy = userPayLoad.Party,
                OrderActivityType = (byte)OrderActivityType.CompletedByDelivery
            };
            await OrdersDb.InsertOrderActivityAsync(connection,
                activity.Order,
                activity.Date,
                activity.OrderActivityType,
                activity.OperatedBy,
                transaction);

            transaction.Commit();

            return Result<CompleteOrderResponseModel>.Ok(new CompleteOrderResponseModel
            {
                OrderId = order.Id,
                Status = "Completed"
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<CompleteOrderResponseModel>.Fail(new[] { ex.Message });
        }
    }
}

public class CompleteOrderRequestModel
{
    public long OrderId { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (OrderId <= 0)
        {
            errors.Add("Valid OrderId required");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}

public class CompleteOrderResponseModel
{
    public long OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
}
