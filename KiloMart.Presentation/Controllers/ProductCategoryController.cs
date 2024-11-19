using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoryController(IDbFactory dbFactory,
 IUserContext userContext)
     : AppController(dbFactory, userContext)
{
    [HttpGet("paginated")]
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
            new { language, offset, pageSize, isActive });

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
            totalCount
        });
    }
}
public class ProductCategoryApiResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
public class ProductCategoryApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    // Localized properties
    public string? LocalizedName { get; set; }
    public byte? Language { get; set; }
}
