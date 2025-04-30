using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Sql;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/tests")]
public class AController(
    IDbFactory dbFactory,
    IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        return Ok("The service is up and running");
    }
}