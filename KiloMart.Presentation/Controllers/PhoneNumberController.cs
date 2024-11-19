using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberController : AppController
{
    public PhoneNumberController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Insert(PhoneNumberInsertModel model)
    {
        var result = await PhoneNumberService.Insert(_dbFactory, _userContext.Get(), model);

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
    public async Task<IActionResult> Update(int id, PhoneNumberUpdateModel model)
    {
        model.Id = id;

        var result = await PhoneNumberService.Update(_dbFactory, _userContext.Get(), model);

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
    public async Task<IActionResult> Delete(int id)
    {

        var result = await PhoneNumberService.Delete(_dbFactory, _userContext.Get(), id);

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
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> GetByPartyId()
    {
        int partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponseForMine>(
            "SELECT [Id], [Value] FROM PhoneNumber WHERE Party = @partyId AND IsActive = 1",
            new { partyId });
        return Ok(phoneNumbers.ToArray());
    }

}

