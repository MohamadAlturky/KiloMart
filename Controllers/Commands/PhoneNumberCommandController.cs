using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.PhoneNumbers.Models;
using KiloMart.Domain.PhoneNumbers.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public PhoneNumberCommandController(IDbFactory dbFactory, IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    [Guard([
        Roles.Customer, 
        Roles.Provider,
        Roles.Delivery])]
    public async Task<IActionResult> Add([FromBody] CreatePhoneNumberRequest request)
    {

        var result = await PhoneNumberService.Insert(_dbFactory, request,_userContext.Get().Party);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
