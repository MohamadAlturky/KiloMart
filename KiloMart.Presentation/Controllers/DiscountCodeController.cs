using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Commands;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/discountcode")]
public class DiscountCodeController : AppController
{
      public DiscountCodeController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
      {
      }

      [HttpPost]
      public async Task<IActionResult> Insert(DiscountCodeInsertModel model)
      {
            var result = await DiscountCodeService.Insert(_dbFactory, _userContext.Get(), model);
            return result.Success ? 
                  CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data) : 
                  BadRequest(result.Errors);
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> Update(int id, DiscountCodeUpdateModel model)
      {
            model.Id = id;
            var result = await DiscountCodeService.Update(_dbFactory, _userContext.Get(), model);
            if (result.Success)
                  return Ok(result.Data);
            
            return result.Errors.Contains("Not Found") ? NotFound() : BadRequest(result.Errors);
      }

      [HttpGet("list")]
      public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
      {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var discountCodes = await Query.GetAllDiscountCodesPaginated(connection, page, pageSize);
            return Ok(discountCodes);
      }
}