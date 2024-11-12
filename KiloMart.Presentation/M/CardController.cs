using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands
{
    [ApiController]
    [Route("api/[controller]")]
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

            if (result.Success)
            {
                return CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CardUpdateModel model)
        {
            model.Id = id;

            var result = await CardService.Update(_dbFactory, _userContext.Get(), model);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                if (result.Errors.Contains("Not Found"))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
        }
    }
}
