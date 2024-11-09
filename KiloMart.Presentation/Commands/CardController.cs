using KiloMart.Core.Contracts;
using KiloMart.Domain.Cards.Models;
using KiloMart.Domain.Cards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/card")]
[Authorize]
public class CardController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public CardController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    public IActionResult CreateCard([FromBody] CardDto card)
    {
        var result = CardService.Insert(_dbFactory, card);

        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
