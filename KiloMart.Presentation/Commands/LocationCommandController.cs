using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Locations.Add.Models;
using KiloMart.Domain.Locations.Add.Services;
using KiloMart.Domain.Locations.Details.Models;
using KiloMart.Domain.Locations.Details.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/location")]
public class LocationCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;

    public LocationCommandController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpPost("add")]
    [Guard([Roles.Customer, Roles.Delivery, Roles.Provider])]
    public async Task<IActionResult> Add([FromBody] CreateLocationRequest request)
    {
        var party = _userContext.Get().Party;
        var result = await LocationService.Insert(_dbFactory, request,party);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }


    [HttpPost("details")]
    public async Task<IActionResult> AddDetails([FromBody] CreateLocationDetailsRequest request)
    {
        var result = await LocationDetailsService.InsertLocationDetails(_dbFactory, request);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
