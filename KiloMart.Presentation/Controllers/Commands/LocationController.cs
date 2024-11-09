
using KiloMart.Core.Contracts;
using KiloMart.Domain.Locations.Add.Services;
using KiloMart.Domain.Locations.Details.Models;
using KiloMart.Domain.Locations.Details.Services;
using KiloMart.Domain.Locations.Models;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Commands;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public LocationController(IDbFactory dbFactory)
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
