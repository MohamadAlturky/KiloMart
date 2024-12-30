using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/date")]
public class DateController(
    IDbFactory dbFactory,
    IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(DateTime.Now);
    }
}