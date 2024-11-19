using System.Numerics;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.OrderRequests;
using KiloMart.Requests.Queries;

namespace KiloMart.Commands.Services;

public static class OrderRequestsService
{
    public static async Task<Result<OrderRequestDto>> Cancel(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        long id)
    {
        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var orderRequest = await Query.GetOrderRequestsByIdAndStatus(
               connection,
               userPayLoad.Party,
               (byte)OrderRequestStatus.Init);
            
            if (orderRequest is null)
            {
                return Result<OrderRequestDto>.Fail(["Not Found"]);

            }
            if (orderRequest.Customer != userPayLoad.Party)
            {
                return Result<OrderRequestDto>.Fail(["Un Authorized"]);

            }
            orderRequest.OrderRequestStatus = (byte)OrderRequestStatus.Canceled;
            await Db.UpdateOrderRequestAsync(connection,
                orderRequest.Id,
                orderRequest.Customer,
                orderRequest.Date,
                (byte)orderRequest.OrderRequestStatus);

            return Result<OrderRequestDto>.Ok(orderRequest);
        }
        catch (Exception e)
        {
            return Result<OrderRequestDto>.Fail([e.Message]);
        }
    }
}
