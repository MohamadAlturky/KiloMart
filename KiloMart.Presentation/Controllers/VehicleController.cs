using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Commands;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/vehicle")]
public class VehicleController : AppController
{
      public VehicleController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
      {
      }

      [HttpPost]
      public async Task<IActionResult> Create(VehicleInsertModel vehicle)
      {
            var result = await VehicleService.Insert(_dbFactory, _userContext.Get(), vehicle);
            return result.Success ?
                  CreatedAtAction(nameof(Create), new { id = result.Data.Id }, result.Data) :
                  BadRequest(result.Errors);
      }

      [HttpPut]
      public async Task<IActionResult> Update(VehicleUpdateModel vehicle)
      {
            var result = await VehicleService.Update(_dbFactory, _userContext.Get(), vehicle);
            if (result.Success)
                  return Ok(result.Data);

            return result.Errors.Contains("Not Found") ? NotFound() : BadRequest(result.Errors);
      }

      [HttpGet("mine")]
      [Guard([Roles.Delivery])]
      public async Task<IActionResult> GetMine()
      {
            var partyId = _userContext.Get().Party;
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var vehicles = await Query.GetVehiclesByDelivery(connection, partyId);
            return Ok(vehicles);
      }
}