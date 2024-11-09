using KiloMart.Core.Contracts;
using KiloMart.Domain.Locations.Add.Models;
using KiloMart.Domain.Locations.Add.Services;
using KiloMart.Domain.Locations.Details.Models;
using KiloMart.Domain.Locations.Details.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/location")]
public class LocationCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public LocationCommandController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CreateLocationRequest request)
    {
        var result = await LocationService.Insert(_dbFactory, request);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }


    [HttpPost("details")]
    public async Task<IActionResult> AddDetails([FromBody] CreateLocationDetailsRequest request)
    {
        var result = await LocationDetailsService.InsertLocationDetails(_dbFactory, request);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
