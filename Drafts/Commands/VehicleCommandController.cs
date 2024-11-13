using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Domain.Vehicles.DataAccess;
using KiloMart.Domain.Vehicles.Models;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/vehicle")]
public class VehicleCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;

    public VehicleCommandController(IDbFactory dbFactory, IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest vehicle)
    {
        var (isValid, errors) = vehicle.Validate();
        if (!isValid)
            return BadRequest(errors);
        var party = _userContext.Get().Party;
        try
        {
            var vehicleModel = new Vehicle()
            {
                Delivary = party,
                Model = vehicle.Model,
                Number = vehicle.Number,
                Type = vehicle.Type,
                Year = vehicle.Year
            };
            await VehicleRepository.InsertVehicle(vehicleModel, _dbFactory);
        }
        catch (Exception e)
        {
            return StatusCode(500, new List<string>() { e.Message });
        }

        return Ok(new { Sucess = true });
    }

    [HttpPut("update")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> Update([FromBody] UpdateVehicleRequest vehicle, [FromQuery] int id)
    {
        var existingVehicle = await VehicleRepository.GetVehicleById(id, _dbFactory);
        var party = _userContext.Get().Party;

        if (existingVehicle is null)
            return NotFound("Vehicle not found");

        if (party != existingVehicle.Delivary)
        {
            return StatusCode(400, "this Vehicle is not for you.");

        }
        var updatedVehicle = new Vehicle
        {
            Id = existingVehicle.Id,
            Number = vehicle.Number ?? existingVehicle.Number,
            Model = vehicle.Model ?? existingVehicle.Model,
            Type = vehicle.Type ?? existingVehicle.Type,
            Year = vehicle.Year ?? existingVehicle.Year,
            Delivary = party
        };

        await VehicleRepository.UpdateVehicle(updatedVehicle, _dbFactory);
        return Ok(updatedVehicle);
    }
}
