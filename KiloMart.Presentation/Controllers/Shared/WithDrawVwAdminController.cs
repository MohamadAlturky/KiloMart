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
[Route("api/admin/withdrawVw")]
public class WithDrawAdminVwController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    // Retrieves withdraw details by Party (Delivery/Provider)
    [HttpGet("Withdraw/by-delivery-or-provider/{id}")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAsync(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawDetailsVwByPartyAsync(id, connection);
        return Success(withdraws.Select(e => new WithdrawDetailsDto(e)).ToList());
    }

    // Retrieves withdraw details filtered by Done status
    [HttpGet("by-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDoneAsync([FromQuery] bool done)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawDetailsVwByDoneAsync(done, connection);
        return Success(withdraws.Select(e=>new WithdrawDetailsDto(e)).ToList());
    }

    // Retrieves withdraw details filtered by Party and Done status
    [HttpGet("by-delivery-or-provider-and-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAndDoneAsync([FromQuery] int partyId, [FromQuery] bool done)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawDetailsVwByPartyAndDoneAsync(partyId, done, connection);
        return Success(withdraws.Select(e=>new WithdrawDetailsDto(e)).ToList());
    }

    // Retrieves paginated withdraw details filtered by Party
    [HttpGet("paginated/by-delivery-or-provider")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDeliveryAsync(
        [FromQuery] int partyId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawDetailsVwByPartyAsync(partyId, pageNumber, pageSize, connection);
        return Success(new { Withdraws = withdraws.Select(e=>new WithdrawDetailsDto(e)).ToList(), TotalCount = totalCount });
    }

    // Retrieves paginated withdraw details filtered by Done status
    [HttpGet("paginated/by-done")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPaginatedWithdrawsByDoneAsync(
        [FromQuery] bool done,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawDetailsVwByDoneAsync(done, pageNumber, pageSize, connection);
        return Success(new { Withdraws = withdraws.Select(e=>new WithdrawDetailsDto(e)).ToList(), TotalCount = totalCount });
    }

    // Retrieves paginated withdraw details filtered by Party and Done status
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

        var (withdraws, totalCount) = await Db.GetPaginatedWithdrawDetailsVwByPartyAndDoneAsync(partyId, done, pageNumber, pageSize, connection);
        return Success(new { Withdraws = withdraws.Select(e=>new WithdrawDetailsDto(e)).ToList(), TotalCount = totalCount });
    }

    // Accepts a withdraw request by updating its status and inserting a corresponding activity record.
    [HttpPost("accept/withdraw")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> AcceptWithDraw([FromBody] AcceptWithdrawRequest request)
    {
        using var readConnection = _dbFactory.CreateDbConnection();
        readConnection.Open();

        // Use the new function to retrieve withdraw details
        var withdraw = await Db.GetWithdrawDetailsVwByIdAsync(request.WithdrawId, readConnection);
        if (withdraw is null)
        {
            return DataNotFound("withdraw not found");
        }

        var user = await Db.GetMembershipUserByPartyAsync(readConnection, withdraw.PartyId);
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
                    withdraw.PartyId,
                    withdraw.BankAccountNumber,
                    withdraw.IBanNumber,
                    withdraw.Date,
                    done: true,
                    accepted: true,
                    rejected: false,
                    transaction);

                await Db.InsertDeliveryActivityAsync(connection,
                    SaudiDateTimeHelper.GetCurrentTime(),
                    request.TotalValue,
                    (byte)DeliveryActivityType.Deductions,
                    withdraw.PartyId,
                    transaction);

                transaction.Commit();

                var wallet = await Db.GetDeliveryActivityTotalValueByDeliveryAsync(
                    withdraw.PartyId,
                    ((byte)DeliveryActivityType.Receives),
                    ((byte)DeliveryActivityType.Deductions),
                    readConnection);
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
                    withdraw.PartyId,
                    withdraw.BankAccountNumber,
                    withdraw.IBanNumber,
                    withdraw.Date,
                    done: true,
                    accepted: true,
                    rejected: false,
                    transaction);

                await Db.InsertProviderActivityAsync(connection,
                    SaudiDateTimeHelper.GetCurrentTime(),
                    request.TotalValue,
                    withdraw.PartyId,
                    (byte)DeliveryActivityType.Deductions,
                    transaction);

                transaction.Commit();

                var wallet = await Db.GetProviderActivityTotalValueByProviderAsync(
                    withdraw.PartyId,
                    ((byte)DeliveryActivityType.Receives),
                    ((byte)DeliveryActivityType.Deductions),
                    readConnection);
                return Success(new { ProviderWallet = wallet });
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Fail(new List<string> { e.Message });
            }
        }
    }

    // Rejects a withdraw request by updating its status accordingly.
    [HttpPost("reject/withdraw")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> RejectWithDraw([FromQuery] long WithdrawId)
    {
        using var readConnection = _dbFactory.CreateDbConnection();
        readConnection.Open();

        var withdraw = await Db.GetWithdrawDetailsVwByIdAsync(WithdrawId, readConnection);
        if (withdraw is null)
        {
            return DataNotFound("withdraw not found");
        }
        var user = await Db.GetMembershipUserByPartyAsync(readConnection, withdraw.PartyId);
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
                withdraw.PartyId,
                withdraw.BankAccountNumber,
                withdraw.IBanNumber,
                withdraw.Date,
                done: true,
                accepted: false,
                rejected: true,
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

// public class AcceptWithdrawRequest
// {
//     public long WithdrawId { get; set; }
//     public decimal TotalValue { get; set; }
// }
