using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoryController : AppController
{
    public ProductCategoryController(IDbFactory dbFactory, IUserContext userContext) : base(dbFactory, userContext)
    {
    }
    [HttpPost]
    public async Task<IActionResult> Insert(ProductCategoryLocalizedRequest model)
    {
        var result = await ProductCategoryService.InsertProductWithLocalization(_dbFactory, _userContext.Get(), model);
        if (result.Success)
        {
            return CreatedAtAction(nameof(Insert), new { id = result.Data.ProductCategory.Id, Data = result.Data }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }


}