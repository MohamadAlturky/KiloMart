using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Drivers;

[ApiController]
[Route("api/drivers")]
public partial class DriverActivitiesContoller(IDbFactory dbFactory, IUserContext userContext)
 : AppController(dbFactory, userContext)
{

}