using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
public class AppController : ControllerBase
{
    protected readonly IDbFactory _dbFactory;
    protected readonly IUserContext _userContext;

    public AppController(IDbFactory dbFactory,
    IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }
}
