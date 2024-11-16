using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/card")]
public class CardController : AppController
{
      public CardController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
      {
      }

      [HttpPost]
      public async Task<IActionResult> Insert(CardInsertModel model)
      {
            var result = await CardService.Insert(_dbFactory, _userContext.Get(), model);
            return result.Success ?
                  CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data) :
                  BadRequest(result.Errors);
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> Update(int id, CardUpdateModel model)
      {
            model.Id = id;
            var result = await CardService.Update(_dbFactory, _userContext.Get(), model);
            if (result.Success)
                  return Ok(result.Data);

            return result.Errors.Contains("Not Found") ? NotFound() : BadRequest(result.Errors);
      }

      [HttpGet("mine")]
      [Guard([Roles.Customer])]
      public async Task<IActionResult> GetMine()
      {
            var partyId = _userContext.Get().Party;
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var cards = await Query.GetCustomerCards(connection, partyId);
            return Ok(cards);
      }

      [HttpGet("list")]
      public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
      {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var cards = await Query.GetAllCardsPaginated(connection, page, pageSize);
            return Ok(cards);
      }
}