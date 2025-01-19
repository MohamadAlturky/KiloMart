using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/product-categories")]
public class ProductCategoryAdminController(IDbFactory dbFactory,
    IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpPost("add")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Insert(ProductCategoryLocalizedRequest model)
    {
        var result = await ProductCategoryService.InsertProductWithLocalization(
            _dbFactory,
            _userContext.Get(),
            model);
        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    [HttpPost("add-without-localization")]
    public async Task<IActionResult> AddProductCategory([FromBody] ProductCategoryAddRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        INSERT INTO ProductCategory (Name, IsActive) 
        VALUES (@Name, @IsActive);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        int newId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            Name = request.Name,
            IsActive = request.IsActive
        });

        return Success(new { Id = newId }, "Product category added successfully.");
    }

    public class ProductCategoryAddRequest
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteProductCategory(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = "DELETE FROM ProductCategory WHERE Id = @id";
        int affectedRows = await connection.ExecuteAsync(sql, new { id });

        if (affectedRows == 0)
            return NotFound("Product category not found.");

        return Success("Product category deleted successfully.");
    }






    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditProductCategory(int id, [FromBody] ProductCategoryEditRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        UPDATE ProductCategory 
        SET Name = @Name, IsActive = @IsActive 
        WHERE Id = @Id";

        int affectedRows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Name = request.Name,
            IsActive = request.IsActive
        });

        if (affectedRows == 0)
            return NotFound("Product category not found.");

        return Success("Product category updated successfully.");
    }

    public class ProductCategoryEditRequest
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }





    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateProductCategory(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = "UPDATE ProductCategory SET IsActive = 0 WHERE Id = @id";
        int affectedRows = await connection.ExecuteAsync(sql, new { id });

        if (affectedRows == 0)
            return NotFound("Product category not found.");

        return Success("Product category deactivated successfully.");
    }








    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateProductCategory(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = "UPDATE ProductCategory SET IsActive = 1 WHERE Id = @id";
        int affectedRows = await connection.ExecuteAsync(sql, new { id });

        if (affectedRows == 0)
            return NotFound("Product category not found.");

        return Success("Product category activated successfully.");
    }









    [HttpGet("localized/{productCategoryId}")]
    public async Task<IActionResult> GetAllLocalizedProductCategories(int productCategoryId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        SELECT Name, Language, ProductCategory 
        FROM ProductCategoryLocalized 
        WHERE ProductCategory = @ProductCategory";

        var results = await connection.QueryAsync<ProductCategoryLocalizedResponse>(
            sql,
            new { ProductCategory = productCategoryId });

        return Success(results);
    }


    [HttpDelete("localized/{productCategoryId}/{language}")]
    public async Task<IActionResult> DeleteLocalizedProductCategory(int productCategoryId, byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        DELETE FROM ProductCategoryLocalized 
        WHERE ProductCategory = @ProductCategory AND Language = @Language";

        int affectedRows = await connection.ExecuteAsync(sql, new
        {
            ProductCategory = productCategoryId,
            Language = language
        });

        if (affectedRows == 0)
            return NotFound("Localized product category not found.");

        return Success("Localized product category deleted successfully.");
    }

    [HttpGet("localized/{productCategoryId}/{language}")]
    public async Task<IActionResult> GetLocalizedProductCategory(int productCategoryId, byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        SELECT Name, Language, ProductCategory 
        FROM ProductCategoryLocalized 
        WHERE ProductCategory = @ProductCategory AND Language = @Language";

        var result = await connection.QueryFirstOrDefaultAsync<ProductCategoryLocalizedResponse>(
            sql,
            new { ProductCategory = productCategoryId, Language = language });

        if (result == null)
            return NotFound("Localized product category not found.");

        return Success(result);
    }

    public class ProductCategoryLocalizedResponse
    {
        public string Name { get; set; } = null!;
        public byte Language { get; set; }
        public int ProductCategory { get; set; }
    }

    [HttpPut("localized/{productCategoryId}/{language}")]
    public async Task<IActionResult> UpdateLocalizedProductCategory(
        int productCategoryId,
        byte language,
        [FromBody] ProductCategoryLocalizedUpdateRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        UPDATE ProductCategoryLocalized 
        SET Name = @Name 
        WHERE ProductCategory = @ProductCategory AND Language = @Language";

        int affectedRows = await connection.ExecuteAsync(sql, new
        {
            Name = request.Name,
            ProductCategory = productCategoryId,
            Language = language
        });

        if (affectedRows == 0)
            return NotFound("Localized product category not found.");

        return Success("Localized product category updated successfully.");
    }

    public class ProductCategoryLocalizedUpdateRequest
    {
        public string Name { get; set; } = null!;
    }


    [HttpPost("localized")]
    public async Task<IActionResult> AddLocalizedProductCategory([FromBody] ProductCategoryLocalizedAddRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        string sql = @"
        INSERT INTO ProductCategoryLocalized (Name, Language, ProductCategory) 
        VALUES (@Name, @Language, @ProductCategory);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        int newId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            Name = request.Name,
            Language = request.Language,
            ProductCategory = request.ProductCategory
        });

        return Success(new { Id = newId }, "Localized product category added successfully.");
    }

    public class ProductCategoryLocalizedAddRequest
    {
        public string Name { get; set; } = null!;
        public byte Language { get; set; }
        public int ProductCategory { get; set; }
    }
}