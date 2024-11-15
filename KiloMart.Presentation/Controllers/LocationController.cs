using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Commands;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/location")]
public class LocationController : AppController
{
      public LocationController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
      {
      }

      [HttpPost]
      public async Task<IActionResult> Insert(LocationInsertModel model)
      {
            var result = await LocationService.Insert(_dbFactory, _userContext.Get(), model);
            return result.Success ?
                  CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data) :
                  BadRequest(result.Errors);
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> Update(int id, LocationUpdateModel model)
      {
            model.Id = id;
            var result = await LocationService.Update(_dbFactory, _userContext.Get(), model);
            if (result.Success)
                  return Ok(result.Data);

            return result.Errors.Contains("Not Found") ? NotFound() : BadRequest(result.Errors);
      }

      [HttpGet("mine")]
      [Guard([Roles.Customer])]
      public async Task<IActionResult> GetMine()
      {
            var party = _userContext.Get().Party;
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var locations = await Query.GetLocationsByParty(connection, party);
            return Ok(locations);
      }
}