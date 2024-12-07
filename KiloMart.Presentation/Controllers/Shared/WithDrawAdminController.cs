using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Delivery.Activity;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/withdraw")]
public class WithDrawAdminController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("Withdraw/by-delivery/{deliveryId}")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAsync(int deliveryId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByDeliveryAsync(deliveryId, connection);
        return Success(withdraws);
    }


    [HttpGet("by-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDoneAsync(bool done)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByDoneAsync(done, connection);
        return Success(withdraws);
    }

    [HttpGet("by-delivery-and-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAndDoneAsync(int deliveryId, bool done)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByDeliveryAndDoneAsync(deliveryId, done, connection);
        return Success(withdraws);
    }

    [HttpGet("paginated/by-delivery")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDeliveryAsync(int deliveryId, int pageNumber, int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawsByDeliveryAsync(deliveryId, pageNumber, pageSize, connection);

        return Success(new { Withdraws = withdraws, TotalCount = totalCount });
    }

    [HttpGet("paginated/by-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDoneAsync(bool done, int pageNumber, int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawsByDoneAsync(done, pageNumber, pageSize, connection);

        return Success(new { Withdraws = withdraws, TotalCount = totalCount });
    }

    [HttpGet("paginated/by-delivery-and-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDeliveryAndDoneAsync(int deliveryId, bool done, int pageNumber, int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawsByDeliveryAndDoneAsync(deliveryId, done, pageNumber, pageSize, connection);

        return Success(new { Withdraws = withdraws, TotalCount = totalCount });
    }

    [HttpPost("accept/withdraw")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> AcceptWithDraw(AcceptWithdrawRequest request)
    {
        using var ReadConnection = _dbFactory.CreateDbConnection();

        ReadConnection.Open();
        var withdraw = await Db.GetWithdrawByIdAsync(request.WithdrawId, ReadConnection);
        if (withdraw is null)
        {
            return DataNotFound("withdraw not found");
        }

        var wallet = await Db.GetDeliveryWalletByDeliveryIdAsync(withdraw.Delivery, ReadConnection);
        if (wallet is null)
        {
            return DataNotFound("wallet not found");
        }

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {

            await Db.UpdateWithdrawAsync(connection,
                withdraw.Id,
                withdraw.Delivery,
                withdraw.BankAccountNumber,
                withdraw.IBanNumber,
                withdraw.Date,
                true,
                transaction);

            await Db.InsertDeliveryActivityAsync(connection,
                DateTime.Now,
                request.TotalValue,
                (byte)DeliveryActivityType.Deductions,
                withdraw.Delivery,
                transaction);
            wallet.Value -= request.TotalValue;
            await Db.UpdateDeliveryWalletAsync(connection,
                wallet.Id,
                wallet.Value - request.TotalValue,
                wallet.Delivery,
                transaction);

            transaction.Commit();
            return Success(new { deliverywallet = wallet });
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Fail(new List<string> { e.Message });
        }
    }
}

public class AcceptWithdrawRequest
{
    public long WithdrawId { get; set; }
    public decimal TotalValue { get; set; }
}