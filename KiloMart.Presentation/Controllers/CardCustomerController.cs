using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/customer/card")]
public class CardCustomerController(IDbFactory dbFactory, IUserContext userContext)
      : AppController(dbFactory, userContext)
{
      [HttpPost]
      [Guard([Roles.Customer])]
      public async Task<IActionResult> Insert(CardInsertModel model)
      {
            var result = await CardService.Insert(_dbFactory, _userContext.Get(), model);
            return result.Success ?
                  CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data) :
                  BadRequest(result.Errors);
      }

      [HttpPut("{id}")]
      [Guard([Roles.Customer])]
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
}