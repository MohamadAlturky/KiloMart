using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.ProductCategories.Models;
using KiloMart.Domain.ProductCategories.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public ProductCategoriesController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    
    [HttpPost("add")]
    public IActionResult Insert(ProductCategoryDto category)
    {
        var (success, errors) = category.Validate();
        if (!success)
            return BadRequest(errors);

        var result = ProductCategoryService.Insert(_dbFactory, category);
        return result.Success ? Ok(result.Data) : BadRequest();
    }
}
