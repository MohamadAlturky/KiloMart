using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
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

        return  Success(result.AsList());
    }
    #endregion

    #region Orders Commands
    

    #endregion
}