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
    // [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetReadyForDeliver()
    {
        var result = await ReadOrderService.GetReadyForDeliverAsync(
            _userContext,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpGet("orders/mine")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetMine()
    {
        using var connection = _dbFactory.CreateDbConnection();

        var result = await OrderRepository.GetOrderDetailsForDeliveryAsync(
            connection,
            _userContext.Get().Party);

        return Success(result.AsList());
    }
    #endregion

    #region Orders Commands

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
}
