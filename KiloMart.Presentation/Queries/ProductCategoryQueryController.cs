using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/product-category")]
public class ProductCategoryQueryController : ControllerBase
{

    private readonly IDbFactory _dbFactory;
    public ProductCategoryQueryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var sql = "SELECT [Id], [Name] , [IsActive] FROM ProductCategory where IsActive = 1";
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>(sql);
        return Ok(categories.ToArray());
    }

    [HttpGet("admin/list")]
    public async Task<IActionResult> AdminList(bool? isActive)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>("SELECT [Id], [Name] , [IsActive] FROM ProductCategory");
        if (isActive is not null)
        {
            categories = await connection.QueryAsync<ProductCategoryApiResponse>("SELECT [Id], [Name] , [IsActive] FROM ProductCategory where IsActive = @isActive", new { isActive });
        }
        return Ok(categories.ToArray());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var category = await connection.QueryFirstOrDefaultAsync<ProductCategoryApiResponse>("SELECT [Id], [Name] , [IsActive] FROM ProductCategory where Id = @id", new { id });
        return Ok(category);
    }
    // get category localized
    [HttpGet("{id}/localized")]
    public async Task<IActionResult> GetLocalizedById(int id, byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        // SQL query with a left join to get category and localized information
        string sql = @"
                SELECT 
                    ProductCategory.[Id], 
                    ProductCategory.[Name], 
                    ProductCategory.[IsActive], 
                    ProductCategoryLocalized.[Name] AS LocalizedName, 
                    ProductCategoryLocalized.[Language]
                FROM ProductCategory
                LEFT JOIN ProductCategoryLocalized 
                    ON ProductCategory.Id = ProductCategoryLocalized.ProductCategory 
                    AND ProductCategoryLocalized.Language = @language
                WHERE ProductCategory.Id = @id";

        // Execute the query with Dapper, mapping to ProductCategoryDto
        var category = await connection.QueryAsync<ProductCategoryApiResponse>(
            sql,
            new { id, language });

        ProductCategoryApiResponse? result = category.FirstOrDefault();
        if (result is null)
        {
            return NotFound();
        }
        return Ok(new
        {
            id = result.Id,
            name = result.LocalizedName is not null ? result.LocalizedName : result.Name,
            isActive = result.IsActive,

        }); // Return the first match or null if none found

    }
    //like the localized but return all localized
    [HttpGet("localized/all")]
    public async Task<IActionResult> GetAllLocalized(byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        string sql = @"
                SELECT 
                    ProductCategory.[Id], 
                    ProductCategory.[Name], 
                    ProductCategory.[IsActive], 
                    ProductCategoryLocalized.[Name] AS LocalizedName, 
                    ProductCategoryLocalized.[Language]
                FROM ProductCategory
                LEFT JOIN ProductCategoryLocalized 
                    ON ProductCategory.Id = ProductCategoryLocalized.ProductCategory 
                    AND ProductCategoryLocalized.Language = @language";
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>(
            sql,
            new { language });
        List<dynamic> result = new();
        foreach (var category in categories)
        {
            result.Add(new
            {
                category.Id,
                Name = category.LocalizedName is not null ? category.LocalizedName : category.Name,
                category.IsActive,
            });
        }
        return Ok(result);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetAllLocalizedPaginated(
    [FromQuery] byte language,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        // Calculate OFFSET for pagination
        int offset = (page - 1) * pageSize;
        var countSql = "SELECT COUNT(*) FROM ProductCategory WITH (NOLOCK) WHERE IsActive = 1";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql);

        // SQL query with pagination, using OFFSET and FETCH
        string sql = @"
        SELECT 
            ProductCategory.[Id], 
            ProductCategory.[Name], 
            ProductCategory.[IsActive], 
            ProductCategoryLocalized.[Name] AS LocalizedName, 
            ProductCategoryLocalized.[Language]
        FROM ProductCategory WITH (NOLOCK)
        LEFT JOIN ProductCategoryLocalized WITH (NOLOCK)
            ON ProductCategory.Id = ProductCategoryLocalized.ProductCategory 
            AND ProductCategoryLocalized.Language = @language
            WHERE ProductCategory.IsActive = 1
        ORDER BY ProductCategory.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>(
            sql,
            new { language, offset, pageSize });

        // Transforming results
        List<ProductCategoryApiResponseDto> result = [];
        foreach (var category in categories)
        {
            result.Add(new()
            {
                Id = category.Id,
                Name = category.LocalizedName ?? category.Name,
                IsActive = category.IsActive,
            });
        }

        return Ok(new
        {
            data = result,
            totalCount = totalCount
        });
    }

    
    [HttpGet("paginated/all")]
    public async Task<IActionResult> GetAllLocalizedPaginatedForAdmin(
    [FromQuery] byte language,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool isActive = true)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        // Calculate OFFSET for pagination
        int offset = (page - 1) * pageSize;
        var countSql = "SELECT COUNT(*) FROM ProductCategory WITH (NOLOCK) WHERE IsActive = @isActive";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive });

        // SQL query with pagination, using OFFSET and FETCH
        string sql = @"
        SELECT 
            ProductCategory.[Id], 
            ProductCategory.[Name], 
            ProductCategory.[IsActive], 
            ProductCategoryLocalized.[Name] AS LocalizedName, 
            ProductCategoryLocalized.[Language]
        FROM ProductCategory WITH (NOLOCK)
        LEFT JOIN ProductCategoryLocalized WITH (NOLOCK)
            ON ProductCategory.Id = ProductCategoryLocalized.ProductCategory 
            AND ProductCategoryLocalized.Language = @language
            WHERE ProductCategory.IsActive = @isActive
        ORDER BY ProductCategory.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>(
            sql,
            new { language, offset, pageSize,isActive });

        // Transforming results
        List<ProductCategoryApiResponseDto> result = [];
        foreach (var category in categories)
        {
            result.Add(new()
            {
                Id = category.Id,
                Name = category.LocalizedName ?? category.Name,
                IsActive = category.IsActive,
            });
        }

        return Ok(new
        {
            data = result,
            totalCount = totalCount
        });
    }

}

public class ProductCategoryApiResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
