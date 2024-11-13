using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/product-offer")]
public class ProductOfferController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public ProductOfferController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpPost("add")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromBody] ProductOfferInsertModel request)
    {

        var result = await ProductOfferService.Insert(_dbFactory, _userContext.Get(), request);
        return result.Success ? 
        CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data)
                : StatusCode(500, result.Errors);
    }

    [HttpPut("{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Update(int id, ProductOfferUpdateModel model)
    {
        model.Id = id;

        var result = await ProductOfferService.Update(_dbFactory, _userContext.Get(), model);

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
