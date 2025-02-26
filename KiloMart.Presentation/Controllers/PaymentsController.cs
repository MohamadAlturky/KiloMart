using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Profiles;

[ApiController]
[Route("callbacks")]
public class PaymentsController : AppController
{
    public PaymentsController(
        IDbFactory dbFactory, 
        IUserContext userContext) 
            : base(dbFactory, userContext)
    {
    }

    [HttpPost("payments")]
    public async Task<IActionResult> Pay()
    {

        return Ok();
    }

    [HttpGet("success")]
    public async Task<IActionResult> Success()
    {
        return Ok();
    }
}