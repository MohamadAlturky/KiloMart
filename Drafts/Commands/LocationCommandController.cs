using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Locations.Add.Models;
using KiloMart.Domain.Locations.Add.Services;
using KiloMart.Domain.Locations.DataAccess;
using KiloMart.Domain.Locations.Details.Models;
using KiloMart.Domain.Locations.Details.Services;
using KiloMart.Domain.Locations.Models;
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
        var result = await LocationService.Insert(_dbFactory, request, party);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }


    [HttpPost("details/add")]
    public async Task<IActionResult> AddDetails([FromBody] CreateLocationDetailsRequest request)
    {
        var result = await LocationDetailsService.InsertLocationDetails(_dbFactory, request);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }


    [HttpPut("update")]
    [Guard([Roles.Customer, Roles.Delivery, Roles.Provider])]
    public async Task<IActionResult> Update(
    [FromBody] LocationUpdateRequest request,
    [FromQuery] int id)
    {
        var location = await LocationRepository.GetLocationById(id, _dbFactory);
        var party = _userContext.Get().Party;

        if (location is null)
        {
            return NotFound("Location not found");
        }

        if (party != location.Party)
        {
            return StatusCode(400,"this Location is not for you.");
        }
        var updatedLocation = new Location
        {
            Id = location.Id,
            Party = location.Party,
            IsActive = location.IsActive
        };

        updatedLocation.Latitude = request.Latitude ?? updatedLocation.Latitude;
        updatedLocation.Name = request.Name ?? updatedLocation.Name;
        updatedLocation.Longitude = request.Longitude ?? updatedLocation.Longitude;

        await LocationRepository.UpdateLocation(updatedLocation, _dbFactory);
        return Ok(updatedLocation);
    }

}
public class LocationUpdateRequest
{
    public float? Longitude { get; set; }
    public float? Latitude { get; set; }
    public string? Name { get; set; }
}