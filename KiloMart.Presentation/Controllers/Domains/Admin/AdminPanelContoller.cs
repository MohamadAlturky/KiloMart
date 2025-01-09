using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Admin;

[ApiController]
[Route("api/admin-panel")]
public class AdminPanelContoller(IDbFactory dbFactory,
 IUserContext userContext,
 IWebHostEnvironment environment) : AppController(dbFactory, userContext)
{
    private readonly IWebHostEnvironment _environment = environment;



}