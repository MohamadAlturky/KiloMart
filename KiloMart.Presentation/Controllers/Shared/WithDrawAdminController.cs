using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.DateServices;
using KiloMart.Domain.Delivery.Activity;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/withdraw")]
public class WithDrawAdminController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("Withdraw/by-delivery-or-provider/{id}")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAsync(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByPartyAsync(id, connection);
        return Success(withdraws);
    }


    [HttpGet("by-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDoneAsync(
        [FromQuery] bool done)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByDoneAsync(done, connection);
        return Success(withdraws);
    }

    [HttpGet("by-delivery-or-provider-and-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAndDoneAsync(
        [FromQuery] int partyId, [FromQuery] bool done)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByPartyAndDoneAsync(partyId, done, connection);
        return Success(withdraws);
    }

    [HttpGet("paginated/by-delivery-or-provider")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDeliveryAsync(
        [FromQuery] int partyId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawsByPartyAsync(partyId, pageNumber, pageSize, connection);

        return Success(new { Withdraws = withdraws, TotalCount = totalCount });
    }

    [HttpGet("paginated/by-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDoneAsync(
        [FromQuery] bool done, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawsByDoneAsync(done, pageNumber, pageSize, connection);

        return Success(new { Withdraws = withdraws, TotalCount = totalCount });
    }

    [HttpGet("paginated/by-delivery-and-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDeliveryAndDoneAsync(
        [FromQuery] int partyId,
        [FromQuery] bool done,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawsByPartyAndDoneAsync(partyId, done, pageNumber, pageSize, connection);

        return Success(new { Withdraws = withdraws, TotalCount = totalCount });
    }

    [HttpPost("accept/withdraw")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> AcceptWithDraw([FromBody] AcceptWithdrawRequest request)
    {
        using var ReadConnection = _dbFactory.CreateDbConnection();

        ReadConnection.Open();

        var withdraw = await Db.GetWithdrawByIdAsync(request.WithdrawId, ReadConnection);
        if (withdraw is null)
        {
            return DataNotFound("withdraw not found");
        }
        var user = await Db.GetMembershipUserByPartyAsync(ReadConnection, withdraw.Party);
        if (user is null)
        {
            return DataNotFound("user not found");
        }

        if (user.Role == (byte)Roles.Delivery)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {

                await Db.UpdateWithdrawAsync(connection,
                    withdraw.Id,
                    withdraw.Party,
                    withdraw.BankAccountNumber,
                    withdraw.IBanNumber,
                    withdraw.Date,
                    true,
                    true,
                    false,
                    transaction);

                await Db.InsertDeliveryActivityAsync(connection,
                    SaudiDateTimeHelper.GetCurrentTime(),
                    request.TotalValue,
                    (byte)DeliveryActivityType.Deductions,
                    withdraw.Party,
                    transaction);

                transaction.Commit();

                var wallet = await Db.GetDeliveryActivityTotalValueByDeliveryAsync(
                    withdraw.Party,
                    ((byte)DeliveryActivityType.Receives),
                    ((byte)DeliveryActivityType.Deductions),
                    ReadConnection);
                return Success(new { deliverywallet = wallet });
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Fail(new List<string> { e.Message });
            }
        }
        else
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                await Db.UpdateWithdrawAsync(connection,
                        withdraw.Id,
                        withdraw.Party,
                        withdraw.BankAccountNumber,
                        withdraw.IBanNumber,
                        withdraw.Date,
                        true,
                        true,
                        false,
                        transaction);

                await Db.InsertProviderActivityAsync(connection,
                    SaudiDateTimeHelper.GetCurrentTime(),
                    request.TotalValue,
                    withdraw.Party,
                    (byte)DeliveryActivityType.Deductions,
                    transaction);
                transaction.Commit();

                var wallet = await Db.GetProviderActivityTotalValueByProviderAsync(
                    withdraw.Party,
                    ((byte)DeliveryActivityType.Receives),
                    ((byte)DeliveryActivityType.Deductions),
                    ReadConnection);
                return Success(new { ProviderWallet = wallet });
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Fail(new List<string> { e.Message });
            }
        }
    }
    [HttpPost("reject/withdraw")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> RejectWithDraw([FromQuery] long WithdrawId)
    {
        using var ReadConnection = _dbFactory.CreateDbConnection();

        ReadConnection.Open();

        var withdraw = await Db.GetWithdrawByIdAsync(WithdrawId, ReadConnection);
        if (withdraw is null)
        {
            return DataNotFound("withdraw not found");
        }
        var user = await Db.GetMembershipUserByPartyAsync(ReadConnection, withdraw.Party);
        if (user is null)
        {
            return DataNotFound("user not found");
        }


        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await Db.UpdateWithdrawAsync(connection,
                withdraw.Id,
                withdraw.Party,
                withdraw.BankAccountNumber,
                withdraw.IBanNumber,
                withdraw.Date,
                true,
                false,
                true,
                transaction);


            transaction.Commit();
            return Success();
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