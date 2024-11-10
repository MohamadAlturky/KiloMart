using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Cards.Models;
using KiloMart.Domain.Cards.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
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
    [Guard([Roles.Customer])]
    public IActionResult CreateCard([FromBody] CardDto card)
    {
        var (Success, Errors) = card.Validate();
        if (!Success)
        {
            return StatusCode(400, Errors);
        }
        var result = CardService.Insert(_dbFactory, card, _userContext.Get().Party);

        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Id = result.Data });
    }


    [HttpPut("{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> UpdateCard([FromBody] UpdateCardDto dto, int id)
    {
        var party = _userContext.Get().Party;
        var result = await UpdateCardService.Update(_dbFactory, dto, id, party);
        if (!result.Success)
        {
            return StatusCode(500, result.Errors);
        }
        return Ok(new 
        {
            Success = true,
            Data = $"card with id = {id} updated successfully."
        });
    }
}
