using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/customer-and-provider/location")]
public class LocationController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{

    [HttpPost("provider-and-customer")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> Insert(LocationInsertModel model)
    {
        var result = await LocationService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPut("{id}")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> Update(int id, LocationUpdateModel model)
    {
        model.Id = id;

        var result = await LocationService.Update(_dbFactory, _userContext.Get(), model);

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
    [HttpDelete("{id}")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await LocationService.DeActivate(_dbFactory, _userContext.Get(), id);

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
    [HttpGet("mine")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> GetMine()
    {
        var party = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[Location] WHERE [Party] = @Party AND IsActive = 1;";
        var result = await connection.QueryAsync<Location>(query, new { Party = party });
        return Ok(result.ToList());
    }
}