using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Vehicles.Models;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Commands;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : AppController
{
    public VehicleController(IDbFactory dbFactory, IUserContext userContext) 
        : base(dbFactory, userContext)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(VehicleInsertModel vehicle)
    {
        var result = await VehicleService.Insert(
          _dbFactory,
         _userContext.Get(),
            vehicle
          );

        if (result.Success)
        {
            return CreatedAtAction(nameof(Create), new { id = result.Data.Id }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, VehicleUpdateModel vehicle)
    {
        vehicle.Id = id;

        var result = await VehicleService.Update(_dbFactory,_userContext.Get(),vehicle);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return NotFound();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
