using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Products.Add.Models;
using KiloMart.Domain.Products.Add.Services;
using KiloMart.Presentation.Models.Commands.Products;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

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

    [HttpGet("admin/paginated")]
    public async Task<IActionResult> GetAllLocalizedPaginatedForAdmin(
        [FromQuery] byte language = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isActive = true)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        // Calculate OFFSET for pagination
        int offset = (page - 1) * pageSize;

        var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive });

        // SQL query with pagination, using OFFSET and FETCH
        string sql = @"
        SELECT
            p.[Id]
            , p.[ImageUrl]
            , p.[ProductCategory]
            , p.[IsActive]
            , p.[MeasurementUnit]
            , p.[Description]
            , p.[Name]
            , pl.[Language]
            , pl.[Product]
            , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
            , pl.[Description] AS LocalizedDescription
            , pl.[Name] AS LocalizedName
        FROM Product p WITH (NOLOCK)
        LEFT JOIN ProductLocalized pl WITH (NOLOCK)
            ON p.Id = pl.Product AND pl.Language = @language
        WHERE p.IsActive = @isActive
        ORDER BY p.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var products = await connection.QueryAsync<ProductApiResponse>(
            sql,
            new { language, offset, pageSize, isActive }
        );

        // Transforming results
        List<ProductApiResponseDto> result = [];
        foreach (var product in products)
        {
            result.Add(new()
            {
                Id = product.Id,
                Name = product.LocalizedName ?? product.Name,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl
            });
        }

        return Ok(new
        {
            data = result,
            totalCount
        });
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
        var isActive = true;
        // Calculate OFFSET for pagination
        int offset = (page - 1) * pageSize;

        var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive AND ProductCategory = @category";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive, category });

        // SQL query with pagination, using OFFSET and FETCH
        string sql = @"
        SELECT
            p.[Id]
            , p.[ImageUrl]
            , p.[ProductCategory]
            , p.[IsActive]
            , p.[MeasurementUnit]
            , p.[Description]
            , p.[Name]
            , pl.[Language]
            , pl.[Product]
            , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
            , pl.[Description] AS LocalizedDescription
            , pl.[Name] AS LocalizedName
        FROM Product p WITH (NOLOCK)
        LEFT JOIN ProductLocalized pl WITH (NOLOCK)
            ON p.Id = pl.Product AND pl.Language = @language
        WHERE p.IsActive = @isActive AND p.ProductCategory = @category 
        ORDER BY p.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var products = await connection.QueryAsync<ProductApiResponse>(
            sql,
            new { language, offset, pageSize, isActive, category }
        );

        // Transforming results
        List<ProductApiResponseDto> result = [];
        foreach (var product in products)
        {
            result.Add(new()
            {
                Id = product.Id,
                Name = product.LocalizedName ?? product.Name,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl
            });
        }

        return Ok(new
        {
            data = result,
            totalCount
        });
    }

    public class ProductApiResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ProductApiResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        // Localized properties
        public byte? Language { get; set; }
        public int? Product { get; set; }
        public string? LocalizedMeasurementUnit { get; set; }
        public string? LocalizedDescription { get; set; }
        public string? LocalizedName { get; set; }
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

        // Calculate OFFSET for pagination
        int offset = (page - 1) * pageSize;

        var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive });

        // SQL query with pagination, using OFFSET and FETCH
        string sql = @"
                SELECT
                    p.[Id]
                    , p.[ImageUrl]
                    , p.[ProductCategory]
                    , p.[IsActive]
                    , p.[MeasurementUnit]
                    , p.[Description]
                    , p.[Name]
                    , pl.[Language]
                    , pl.[Product]
                    , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
                    , pl.[Description] AS LocalizedDescription
                    , pl.[Name] AS LocalizedName
                    , po.MaxPrice
                    , po.MinPrice
                FROM Product p WITH (NOLOCK)
                LEFT JOIN ProductLocalized pl WITH (NOLOCK)
                    ON p.Id = pl.Product AND pl.Language = @language
                INNER JOIN (
                    SELECT Product, MAX(Price) AS MaxPrice, MIN(Price) AS MinPrice
                    FROM ProductOffer
                    GROUP BY Product
                ) po
                ON p.Id = po.Product
                WHERE p.IsActive = @isActive
                ORDER BY p.[Id]
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var products = await connection.QueryAsync<ProductApiResponseWithOffer>(
            sql,
            new { language, offset, pageSize, isActive }
        );

        // Transforming results
        List<ProductApiResponseWithOfferDto> result = new();
        foreach (var product in products)
        {
            result.Add(new()
            {
                Id = product.Id,
                Name = product.LocalizedName ?? product.Name,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                MinPrice = product.MinPrice,
                MaxPrice = product.MaxPrice,
            });
        }

        return Ok(new
        {
            data = result,
            totalCount
        });
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
        bool isActive = true;
        // Calculate OFFSET for pagination
        int offset = (page - 1) * pageSize;

        var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive AND ProductCategory = @category";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive, category });

        // SQL query with pagination, using OFFSET and FETCH
        string sql = @"
                SELECT
                    p.[Id]
                    , p.[ImageUrl]
                    , p.[ProductCategory]
                    , p.[IsActive]
                    , p.[MeasurementUnit]
                    , p.[Description]
                    , p.[Name]
                    , pl.[Language]
                    , pl.[Product]
                    , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
                    , pl.[Description] AS LocalizedDescription
                    , pl.[Name] AS LocalizedName
                    , po.MaxPrice
                    , po.MinPrice
                FROM Product p WITH (NOLOCK)
                LEFT JOIN ProductLocalized pl WITH (NOLOCK)
                    ON p.Id = pl.Product AND pl.Language = @language
                INNER JOIN (
                    SELECT Product, MAX(Price) AS MaxPrice, MIN(Price) AS MinPrice
                    FROM ProductOffer
                    GROUP BY Product
                ) po
                ON p.Id = po.Product
                WHERE p.IsActive = @isActive AND ProductCategory = @category
                ORDER BY p.[Id]
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var products = await connection.QueryAsync<ProductApiResponseWithOffer>(
            sql,
            new { language, offset, pageSize, isActive, category }
        );

        // Transforming results
        List<ProductApiResponseWithOfferDto> result = new();
        foreach (var product in products)
        {
            result.Add(new()
            {
                Id = product.Id,
                Name = product.LocalizedName ?? product.Name,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                MinPrice = product.MinPrice,
                MaxPrice = product.MaxPrice,
            });
        }

        return Ok(new
        {
            data = result,
            totalCount
        });
    }

    public class ProductApiResponseWithOffer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        // Localized properties
        public byte? Language { get; set; }
        public int? Product { get; set; }
        public string? LocalizedMeasurementUnit { get; set; }
        public string? LocalizedDescription { get; set; }
        public string? LocalizedName { get; set; }
        // ProductOffer properties
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class ProductApiResponseWithOfferDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}

