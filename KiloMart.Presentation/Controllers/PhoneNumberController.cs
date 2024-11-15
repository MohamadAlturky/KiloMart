using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Commands;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberController : AppController
{
      public PhoneNumberController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
      {
      }

      [HttpPost]
      public async Task<IActionResult> Insert(PhoneNumberInsertModel model)
      {
            var result = await PhoneNumberService.Insert(_dbFactory, _userContext.Get(), model);
            return result.Success ?
                  CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data) :
                  BadRequest(result.Errors);
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> Update(int id, PhoneNumberUpdateModel model)
      {
            model.Id = id;
            var result = await PhoneNumberService.Update(_dbFactory, _userContext.Get(), model);
            if (result.Success)
                  return Ok(result.Data);

            return result.Errors.Contains("Not Found") ? NotFound() : BadRequest(result.Errors);
      }

      [HttpGet("mine")]
      [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
      public async Task<IActionResult> GetByPartyId()
      {
            int partyId = _userContext.Get().Party;
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var phoneNumbers = await Query.GetPhoneNumbersByParty(connection, partyId);
            return Ok(phoneNumbers);
      }

      [HttpGet("list")]
      public async Task<IActionResult> GetPhoneNumbers(int page = 1, int pageSize = 10)
      {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var (data, totalCount) = await Query.GetAllPhoneNumbersPaginated(connection, page, pageSize);
            return Ok(new { data, totalCount });
      }
}