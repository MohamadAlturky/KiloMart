using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.Step1;
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
    public async Task<IActionResult> Init(CreateOrderRequest model)
    {
        var result = await InitOrderService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return CreatedAtAction(nameof(Init), new { id = result.Data.Id }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
}