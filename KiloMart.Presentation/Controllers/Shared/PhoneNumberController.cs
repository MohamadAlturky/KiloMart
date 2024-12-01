using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/customer-provider-delivery/phone-number")]
public class PhoneNumberController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpPost]
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> Insert(PhoneNumberInsertModel model)
    {
        var result = await PhoneNumberService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    [HttpPut("{id}")]
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> Update(int id, PhoneNumberUpdateModel model)
    {
        model.Id = id;

        var result = await PhoneNumberService.Update(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }
    
    [HttpDelete("{id}")]
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> Delete(int id)
    {

        var result = await PhoneNumberService.Delete(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
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
        return Success(phoneNumbers.ToArray());
    }
}

