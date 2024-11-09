using KiloMart.Core.Contracts;
using KiloMart.Domain.Vehicles.Models;
using KiloMart.Domain.Vehicles.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Commands;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly VehicleService _vehicleService;

    public VehicleController(IDbFactory dbFactory, VehicleService vehicleService)
    {
        _dbFactory = dbFactory;
        _vehicleService = vehicleService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest vehicle)
    {
        var (isValid, errors) = vehicle.Validate();
        if (!isValid)
            return BadRequest(errors);

        var result = await _vehicleService.Create(vehicle, _dbFactory);

        if (!result.Success)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }
}
