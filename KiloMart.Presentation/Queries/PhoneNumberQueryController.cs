using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public PhoneNumberQueryController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }
    [HttpGet("mine")]
    [Guard([Roles.Customer,Roles.Provider,Roles.Delivery])]
    public async Task<IActionResult> GetByPartyId()
    {
        int partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponse>(
            "SELECT [Id], [Value], [Party] FROM PhoneNumber WHERE Party = @partyId",
            new { partyId });
        return Ok(phoneNumbers.ToArray());
    }
}