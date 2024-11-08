using KiloMart.Api.Services;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Products.Add.Models;
using KiloMart.Domain.Products.Add.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers;

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
    public async Task<IActionResult> Insert([FromForm] CreateProductRequest product){
        var (success, errors) = product.Validate();
        if (!success)
            return BadRequest(errors);
        if(product.File is null)
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
                Language = product.ArabicData.LanguageId,
                MeasurementUnit = product.ArabicData.MeasurementUnit,
            },
            new ProductLocalizedDto
            {
                Name = product.EnglishData.Name,
                Description = product.EnglishData.Description,
                Language = product.EnglishData.LanguageId,
                MeasurementUnit = product.EnglishData.MeasurementUnit,
            }]
        };
        var result = ProductService.Insert(_dbFactory, productDto);
        return result.Success ? Ok(result.Data) : StatusCode(500,result.Errors);
    }
}

public class CreateProductRequest
{
    public IFormFile? File { get; set; }
    public int CategoryId { get; set; }
    public CreateProductLocalizationRequest? ArabicData { get; set; }
    public CreateProductLocalizationRequest? EnglishData { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (CategoryId == 0)
            errors.Add("Category is required");
        if (ArabicData is null)
            errors.Add("Arabic data is required");
        if (EnglishData is null)
            errors.Add("English data is required");
        if(ArabicData?.Validate().Success == false)
            errors.AddRange(ArabicData.Validate().Errors);
        if(EnglishData?.Validate().Success == false)
            errors.AddRange(EnglishData.Validate().Errors);
        if (File is null)
            errors.Add("File is required");
        return (errors.Count == 0, errors.ToArray());
    }
}

public class CreateProductLocalizationRequest
{
    public string Name { get; set; }= string.Empty;
    public string Description { get; set; }= string.Empty;
    public byte LanguageId { get; set; }
    public string MeasurementUnit { get; set; } = string.Empty;
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(Name))
            errors.Add("Name is required");
        if (string.IsNullOrEmpty(Description))
            errors.Add("Description is required");
        if (LanguageId == 0)
            errors.Add("Language is required");
        if (string.IsNullOrEmpty(MeasurementUnit))
            errors.Add("Measurement unit is required");
        return (errors.Count == 0, errors.ToArray());
    }
}
