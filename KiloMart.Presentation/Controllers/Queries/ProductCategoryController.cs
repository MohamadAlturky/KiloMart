using Dapper;
using KiloMart.Api.Models;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.ProductCategories.Models;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Queries;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoryController : ControllerBase
{

    private readonly IDbFactory _dbFactory;
    public ProductCategoryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>("SELECT [Id], [Name] , [IsActive] FROM ProductCategory where IsActive = 1");
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
        if(result is null)
        {
            return NotFound();
        }
        return Ok(new{
            id=result.Id,
            name=result.LocalizedName is not null ? result.LocalizedName : result.Name,
            isActive=result.IsActive,
            
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
                    where ProductCategoryLocalized.Language = @language";
        var categories = await connection.QueryAsync<ProductCategoryApiResponse>(
            sql, 
            new { language });
        List<dynamic> result = new();
        foreach (var category in categories)
        {
            result.Add(new {
                Id=category.Id,
                Name=category.LocalizedName is not null ? category.LocalizedName : category.Name,
                IsActive=category.IsActive,
            });
        }
        return Ok(result);
    }
}
