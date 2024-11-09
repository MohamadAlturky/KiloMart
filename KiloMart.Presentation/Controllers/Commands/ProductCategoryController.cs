using KiloMart.Core.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.ProductCategories.Models;
using KiloMart.Domain.ProductCategories.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Commands;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public ProductCategoryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost("create")]
    public IActionResult Insert([FromBody] List<CreateProductCategoryRequest> categoryinfos)
    {
        var category = new ProductCategoryDto
        {
            IsActive = true,
            Localizations = []
        };
        foreach (var info in categoryinfos)
        {
            category.Name = info.Name;
            category.Localizations.Add(new ProductCategoryLocalizedDto()
            {
                Language = (byte)info.Language,
                Name = info.Name
            });
        }
        var (success, errors) = category.Validate();
        if (!success)
            return BadRequest(errors);

        var result = ProductCategoryService.Insert(_dbFactory, category);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
