using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Cards.Models;
using KiloMart.Domain.Cards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/card")]
[Authorize]
public class CardCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;

    public CardCommandController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpPost("add")]
    public IActionResult CreateCard([FromBody] CardDto card)
    {
        var (Success,Errors) = card.Validate();
        if (!Success)
        {
            return StatusCode(400, Errors);
        }
        var result = CardService.Insert(_dbFactory, card,_userContext.Get().Party);

        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new {Id = result.Data});
    }
}
