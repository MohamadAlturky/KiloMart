using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
public class AppController(IDbFactory dbFactory,
IUserContext userContext) : ControllerBase
{
    protected readonly IDbFactory _dbFactory = dbFactory;
    protected readonly IUserContext _userContext = userContext;
}
