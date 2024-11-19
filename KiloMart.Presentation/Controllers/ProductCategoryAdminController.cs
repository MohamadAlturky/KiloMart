using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/product-categories")]
public class ProductCategoryAdminController(IDbFactory dbFactory,
    IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpPost("add")]
    public async Task<IActionResult> Insert(ProductCategoryLocalizedRequest model)
    {
        var result = await ProductCategoryService.InsertProductWithLocalization(
            _dbFactory,
            _userContext.Get(),
            model);
        if (result.Success)
        {
            return CreatedAtAction(nameof(Insert),
            new
            {
                id = result.Data.ProductCategory.Id,
                result.Data
            }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
}