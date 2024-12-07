using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Services;

public static class ChangeOrderPaymentTypeService
{

    public static async Task<Result<UpdateOrderPaymentResponseModel>> UpdateOrderPaymentType(
        UpdateOrderPaymentRequestModel model,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<UpdateOrderPaymentResponseModel>.Fail(errors);
        }

        var response = new UpdateOrderPaymentResponseModel();

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var whereClause = "WHERE o.Id = @id";
            var parameters = new { id = model.OrderId };
            OrderDetailsDto? order = await OrderRepository.GetOrderDetailsFirstOrDefaultAsync(
                connection,
                whereClause,
                parameters);

            if (order is null)
            {
                return Result<UpdateOrderPaymentResponseModel>.Fail(["Order Not Found"]);
            }

            if (order.Customer != userPayLoad.Party)
            {
                return Result<UpdateOrderPaymentResponseModel>.Fail(["Order is not for this customer"]);
            }
            
            if(order.OrderStatus == (byte)OrderStatus.COMPLETED)
            {
                return Result<UpdateOrderPaymentResponseModel>.Fail(["Order is Completed"]);
            }

            order.PaymentType = model.PaymentType;
            await OrdersDb.UpdateOrderPaymentAsync(connection,
                order.Id,
                model.PaymentType,
                transaction);
            response.Order = order;

            transaction.Commit();
            return Result<UpdateOrderPaymentResponseModel>.Ok(response);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<UpdateOrderPaymentResponseModel>.Fail([ex.Message]);
        }
    }
}
public class UpdateOrderPaymentRequestModel
{
    public long OrderId { get; set; }
    public byte PaymentType { get; set; }
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (OrderId == 0)
        {
            errors.Add("OrderId required");
        }
        if (PaymentType == 0)
        {
            errors.Add("PaymentType required");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}

public class UpdateOrderPaymentResponseModel
{
    public OrderDetailsDto Order { get; set; } = new();
}