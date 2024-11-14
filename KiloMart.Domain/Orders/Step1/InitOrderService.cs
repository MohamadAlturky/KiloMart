using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Orders.Shared;

namespace KiloMart.Domain.Orders.Step1;
public static class InitOrderService
{
    public static async Task<Result<Order>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        CreateOrderRequest model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Order>.Fail(errors);
        }
    }
}