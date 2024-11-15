using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.Step1;
using KiloMart.Domain.Orders.Step2;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Commands;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : AppController
{
    public OrderController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }
    [HttpPost("init")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Init(CreateOrderRequest model)
    {
        var result = await InitOrderService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return CreatedAtAction(nameof(Init), new { id = result.Data.Order.Id }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
    [HttpPost("cancel")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Cancel(long id)
    {
        var result = await CancelOrderService.Cancel(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Ok(new { Status = true });
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("accept")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Accept([FromQuery] long id)
    {
        var result = await AcceptOrderService.Accept(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Ok(new { Status = true });
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
    [HttpPost("reject")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Reject([FromQuery] long id)
    {
        var result = await RejectOrderService.Reject(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Ok(new { Status = true });
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
}