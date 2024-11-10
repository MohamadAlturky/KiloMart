using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Locations.DataAccess;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/location")]
public class LocationQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public LocationQueryController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpGet("mine")]
    [Guard([Roles.Customer, Roles.Delivery, Roles.Provider])]
    public async Task<IActionResult> List()
    {
        var party = _userContext.Get().Party;
        try
        {
            var result = await LocationRepository.GetLocationsByParty(party,_dbFactory);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Errors = new List<string>() { e.Message } });
        }
    }
}
