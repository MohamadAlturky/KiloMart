using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Queries;

[ApiController]
[Route("api/[controller]")]
public class PhoneNumberController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public PhoneNumberController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    [HttpGet("{partyId}")]
    public async Task<IActionResult> GetByPartyId(int partyId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponse>(
            "SELECT [Id], [Value],[Party] FROM PhoneNumber WHERE Party = @partyId",
            new { partyId });
        return Ok(phoneNumbers.ToArray());
    }
}