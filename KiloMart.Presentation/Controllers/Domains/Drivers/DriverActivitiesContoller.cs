using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Repositories;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Drivers;

[ApiController]
[Route("api/drivers")]
public partial class DriverActivitiesContoller(IDbFactory dbFactory, IUserContext userContext)
 : AppController(dbFactory, userContext)
{
    #region Orders reading

    [HttpGet("orders/ready-to-deliver")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetReadyForDeliver()
    {
        var result = await ReadOrderService.GetReadyForDeliverAsync(
            _userContext,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpGet("orders/completed")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetCompletedForDeliveryAsync()
    {
        var result = await ReadOrderService.GetCompletedForDeliveryAsync(
            _userContext,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpGet("orders/min-all-orders-by-status")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetMine([FromQuery] byte status)
    {
        using var connection = _dbFactory.CreateDbConnection();

        var result = await OrderRepository.GetOrderDetailsForDeliveryAsync(
            connection,
            _userContext.Get().Party,
            status);

        return Success(result.AsList());
    }
    #endregion

    #region Activities

    [HttpGet("activities/by-date-range")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
    {
        int deliveryId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetDeliveryActivitiesByDateBetweenAndDeliveryAsync(startDate, endDate, deliveryId, connection);
        return Success(activities);
    }

    [HttpGet("activities/by-date-bigger")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetByDateBigger(
        [FromQuery] DateTime date)
    {
        int deliveryId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetDeliveryActivitiesByDateBiggerAndDeliveryAsync(date, deliveryId, connection);
        return Success(activities);
    }
    [HttpGet("activities/all")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetMin()
    {
        int deliveryId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetDeliveryActivitiesByDeliveryIdAsync(deliveryId, connection);
        return Success(activities);
    }

    #endregion

    #region Wallets
    [HttpGet("wallet/mine")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetByDeliveryId()
    {
        int deliveryId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var wallet = await Db.GetDeliveryWalletByDeliveryIdAsync(deliveryId, connection);
        if (wallet == null)
        {
            return DataNotFound("No Wallet For This Delivery");
        }
        return Success(wallet);
    }
    #endregion

    #region orders actions
    [HttpPost("orders/accept")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> AcceptOrder([FromBody] long orderId)
    {
        var result = await AcceptOrderService.DeliveryAccept(orderId, _userContext.Get(), _dbFactory);
        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpPost("orders/cancel")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> Cancel([FromBody] long orderId)
    {
        var result = await OrderCancelService.DeliveryCancel(
            orderId,
            _userContext.Get(),
            _dbFactory
            );
        if (result.Success)
        {
            return Success(new
            {
                Message = "order canceled successfully",
                Order = result.Data
            });
        }
        return Fail(result.Errors);
    }
    #endregion

    #region Withdrawal Actions

    [HttpPost("Withdraw/create")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> InsertWithdrawAsync([FromBody] InsertWithdrawRequest request)
    {
        int deliveryID = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var id = await Db.InsertWithdrawAsync(
            connection,
            deliveryID,
            request.BankAccountNumber,
            request.IbanNumber,
            DateTime.Now,
            false);

        return Success(new { Id = id });
    }

    [HttpPut("Withdraw/update/{id}")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> UpdateWithdrawAsync(long id, [FromBody] UpdateWithdrawRequest request)
    {
        int deliveryID = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        Withdraw? withdraw = await Db.GetWithdrawByIdAsync(id, connection);
        if (withdraw is null)
        {
            return DataNotFound();
        }
        if (withdraw.Delivery != deliveryID)
        {
            return Fail("Un Authorized this withdraw isn't for you");
        }
        if (withdraw.Done)
        {
            return Fail("this withdraw is done!! can't edit it");
        }
        var success = await Db.UpdateWithdrawAsync(
            connection,
            id,
            withdraw.Delivery,
            request.BankAccountNumber ?? withdraw.BankAccountNumber,
            request.IbanNumber ?? withdraw.IBanNumber,
            withdraw.Date,
            withdraw.Done);

        return success ? Success() : Fail("Update failed.");
    }

    [HttpDelete("Withdraw/delete/{id}")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> DeleteWithdrawAsync(long id)
    {
        int deliveryID = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        Withdraw? withdraw = await Db.GetWithdrawByIdAsync(id, connection);
        if (withdraw is null)
        {
            return DataNotFound();
        }
        if (withdraw.Delivery != deliveryID)
        {
            return Fail("Un Authorized this withdraw isn't for you");
        }
        var success = await Db.DeleteWithdrawAsync(connection, id);
        return success ? Success() : Fail("Deletion failed.");
    }

    [HttpGet("Withdraw/{id}")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetWithdrawByIdAsync(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraw = await Db.GetWithdrawByIdAsync(id, connection);
        return withdraw is not null ? Success(withdraw) : DataNotFound("Withdrawal not found.");
    }
    [HttpGet("Withdraw/mine")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAsync()
    {
        int deliveryId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByDeliveryAsync(deliveryId, connection);
        return Success(withdraws);
    }
    #endregion
}

// Request models for Insert and Update actions
public class InsertWithdrawRequest
{

    public string BankAccountNumber { get; set; }

    public string IbanNumber { get; set; }

}

public class UpdateWithdrawRequest
{

    public string? BankAccountNumber { get; set; }

    public string? IbanNumber { get; set; }

}
