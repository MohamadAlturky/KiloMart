using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/card")]
public class CardQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public CardQueryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    [HttpGet("{partyId}")]
    
    public async Task<IActionResult> GetByPartyId(int partyId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cards = await connection.QueryAsync<CardApiResponse>(
            "SELECT [Id], [HolderName], [Number], [SecurityCode], [ExpireDate], [Customer] FROM Card WHERE Customer = @partyId",
            new { partyId });
        return Ok(cards.ToArray());

    }
}