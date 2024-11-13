using Dapper;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;



[ApiController]
[Route("api/customer")]
public class ProductQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public ProductQueryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet("paginated/by-category")]
    public async Task<IActionResult> GetAllLocalizedPaginatedForAdmin(
    [FromQuery] int category,
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
                p.[Id] AS Id,
                p.[ImageUrl] AS ImageUrl,
                p.[IsActive] AS IsActive,
                COALESCE(pl.[MeasurementUnit], p.[MeasurementUnit]) AS MeasurementUnit,
                COALESCE(pl.[Description], p.[Description]) AS Description,
                COALESCE(pl.[Name], p.[Name]) AS Name
            FROM Product p
                LEFT JOIN ProductLocalized pl ON p.Id = pl.Product AND pl.Language = @Language
            WHERE p.IsActive = @isActive AND p.[ProductCategory] = @category
            ORDER BY p.[Id]
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Query execution with pagination parameters
        var products = await connection.QueryAsync<ProductApiResponse>(
            sql,
            new { Language = language, offset, pageSize, isActive,category });
        

        return Ok(new
        {
            data = products.ToList(),
            totalCount = totalCount
        });
    }

    public class ProductApiResponse
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }= null!;
        public bool IsActive { get; set; }
        public string MeasurementUnit { get; set; }= null!;
        public string Description { get; set; } = null!;
        public string Name { get; set; }= null!;
    }
}