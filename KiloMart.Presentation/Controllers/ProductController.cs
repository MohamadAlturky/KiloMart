using KiloMart.Core.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Products.Add.Models;
using KiloMart.Domain.Products.Add.Services;
using KiloMart.Presentation.Models.Commands.Products;
using KiloMart.Presentation.Services;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IWebHostEnvironment _environment;

    public ProductController(IDbFactory dbFactory, IWebHostEnvironment environment)
    {
        _dbFactory = dbFactory;
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromForm] CreateProductRequest product)
    {
        var (success, errors) = product.Validate();
        if (!success)
            return BadRequest(errors);

        if (product.File is null)
            return BadRequest("File is required");

        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveFileAsync(product.File, _environment.WebRootPath, fileName);

        var productDto = new ProductDto
        {
            CategoryId = product.CategoryId,
            ImageUrl = filePath,
            IsActive = true,
            Description = product.ArabicData.Description,
            MeasurementUnit = product.ArabicData.MeasurementUnit,
            Name = product.ArabicData.Name,
            Localizations = [
                new ProductLocalizedDto
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
                }
            ]
        };
        var result = ProductService.Insert(_dbFactory, productDto);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }

    [HttpGet("admin/paginated")]
    public async Task<IActionResult> GetAllLocalizedPaginatedForAdmin(
        [FromQuery] byte language = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isActive = true)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var (data, totalCount) = await Query.GetProductsPaginated(connection, language, page, pageSize, isActive);
        return Ok(new { data, totalCount });
    }

    [HttpGet("customer/paginated")]
    public async Task<IActionResult> GetAllLocalizedPaginatedForCustomer(
        [FromQuery] int category,
        [FromQuery] byte language = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var (data, totalCount) = await Query.GetProductsPaginatedByCategory(connection, category, language, page, pageSize, true);
        return Ok(new { data, totalCount });
    }

    [HttpGet("admin/paginated-with-offer")]
    public async Task<IActionResult> GetProductsWithOfferPaginatedForAdmin(
        [FromQuery] byte language = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isActive = true)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var (data, totalCount) = await Query.GetProductsWithOfferPaginated(connection, language, page, pageSize, isActive);
        return Ok(new { data, totalCount });
    }

    [HttpGet("customer/paginated-with-offer")]
    public async Task<IActionResult> GetProductsWithOfferPaginatedForCustomer(
        [FromQuery] int category,
        [FromQuery] byte language = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var (data, totalCount) = await Query.GetProductsWithOfferPaginatedByCategory(connection, category, language, page, pageSize, true);
        return Ok(new { data, totalCount });
    }
}
