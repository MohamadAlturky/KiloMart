using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/card")]
public class CardQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public CardQueryController(IDbFactory dbFactory, IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
    }

    [HttpGet("mine")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMine()
    {
        var partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cards = await connection.QueryAsync<CardApiResponse>(
            "SELECT [Id], [HolderName], [Number], [SecurityCode], [ExpireDate], [Customer] FROM Card WHERE Customer = @partyId AND IsActive = 1;",
            new { partyId });
        return Ok(cards.ToArray());

    }


}