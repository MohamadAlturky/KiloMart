using Dapper;
using KiloMart.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Queries;

[ApiController]
[Route("api/[controller]")]
public class CardController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public CardController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    [HttpGet("{partyId}")]
    public async Task<IActionResult> GetByPartyId(int partyId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cards = await connection.QueryAsync<CardDto>(
            "SELECT [Id], [HolderName], [Number], [SecurityCode], [ExpireDate], [Customer] FROM Card WHERE Customer = @partyId",
            new { partyId });
        return Ok(cards.ToArray());

    }
}