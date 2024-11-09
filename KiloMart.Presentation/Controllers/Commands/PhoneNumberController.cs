using KiloMart.Core.Contracts;
using KiloMart.Domain.PhoneNumbers.Models;
using KiloMart.Domain.PhoneNumbers.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Commands;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public PhoneNumberController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CreatePhoneNumberRequest request)
    {
        var result = await PhoneNumberService.Insert(_dbFactory, request);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
