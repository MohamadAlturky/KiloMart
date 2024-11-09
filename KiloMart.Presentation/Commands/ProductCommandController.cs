using KiloMart.Core.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Products.Add.Models;
using KiloMart.Domain.Products.Add.Services;
using KiloMart.Domain.Products.Offers.Models;
using KiloMart.Domain.Products.Offers.Services;
using KiloMart.Presentation.Models.Commands.Products;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/product")]
public class ProductCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IWebHostEnvironment _environment;

    public ProductCommandController(IDbFactory dbFactory, IWebHostEnvironment environment)
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

    [HttpPost("offer")]
    public IActionResult CreateOffer([FromBody] CreateProductOfferRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return BadRequest(errors);

        var productOffer = new ProductOfferDto
        {
            Product = request.ProductId,
            Price = request.Price,
            OffPercentage = request.OffPercentage,
            FromDate = request.FromDate,
            ToDate = null,
            Quantity = request.Quantity,
            Provider = request.ProviderId,
            IsActive = true
        };

        var result = ProductOfferService.Insert(_dbFactory, productOffer);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}

