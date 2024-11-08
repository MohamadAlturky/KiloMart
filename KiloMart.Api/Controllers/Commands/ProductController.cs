using KiloMart.Api.Models.Commands.Products;
using KiloMart.Api.Services;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Products.Add.Models;
using KiloMart.Domain.Products.Add.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Commands;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IWebHostEnvironment _environment;

    public ProductController(IDbFactory dbFactory, IWebHostEnvironment environment)
    {
        _dbFactory = dbFactory;
        _environment = environment;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Insert([FromForm] CreateProductRequest product)
    {
        var (success, errors) = product.Validate();
        if (!success)
            return BadRequest(errors);
        if (product.File is null)
        {
            return BadRequest("File is required");
        }
        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveFileAsync(product.File,
            _environment.WebRootPath,
            fileName);

        var productDto = new ProductDto
        {
            CategoryId = product.CategoryId,
            ImageUrl = filePath,
            IsActive = true,
            Description = product.ArabicData.Description,
            MeasurementUnit = product.ArabicData.MeasurementUnit,
            Name = product.ArabicData.Name,
            Localizations = [new ProductLocalizedDto
            {
                Name = product.ArabicData.Name,
                Description = product.ArabicData.Description,
                Language = (byte)Language.Arabic,
                MeasurementUnit = product.ArabicData.MeasurementUnit,
            },
            new ProductLocalizedDto
            {
                Name = product.EnglishData.Name,
                Description = product.EnglishData.Description,
                Language = (byte)Language.English,
                MeasurementUnit = product.EnglishData.MeasurementUnit,
            }]
        };
        var result = ProductService.Insert(_dbFactory, productDto);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
