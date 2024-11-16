using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
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

    [HttpPut]
    public async Task<IActionResult> Update(VehicleUpdateModel vehicle)
    {
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
    [HttpGet("mine")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetMine()
    {
        var partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cards = await connection.QueryAsync<VehicleApiResponse>(
            @"
            SELECT TOP (1000) [Id]
                ,[Number]
                ,[Model]
                ,[Type]
                ,[Year]
            FROM [KiloMartMasterDb].[dbo].[Vehicle]
            WHERE [Delivary] = @Delivary
            ",
            new { Delivary = partyId });
        return Ok(cards.ToArray());

    }
}
public class VehicleApiResponse
{
    public int Id { get; set; }
    public string Number { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Year { get; set; }
}