using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;  // For the Db static class and ProductLocalizedData
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/admin/product")]
public class ProductAdminController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    // POST api/admin/product/activate/{id}
    [HttpPost("activate/{id:int}")]
    [Guard(new[] { Roles.Admin })]
    public async Task<IActionResult> ActivateProduct([FromRoute] int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        bool activated = await Db.ActivateProductAsync(connection, id);
        if (activated)
            return Success("Product activated successfully.");
        return Fail("Failed to activate product.");
    }

    // POST api/admin/product/deactivate/{id}
    [HttpPost("deactivate/{id:int}")]
    [Guard(new[] { Roles.Admin })]
    public async Task<IActionResult> DeactivateProduct([FromRoute] int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        bool deactivated = await Db.DeactivateProductAsync(connection, id);
        if (deactivated)
            return Success("Product deactivated successfully.");
        return Fail("Failed to deactivate product.");
    }

  
    // POST api/admin/product/{id}/localized
    [HttpPost("{productId:int}/localized")]
    [Guard(new[] { Roles.Admin })]
    public async Task<IActionResult> InsertProductLocalized([FromRoute] int productId, [FromBody] ProductLocalizedRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var productLocalizedData = await Db.GetProductLocalizedAsync(connection, request.Language, productId);
        if (productLocalizedData is not null)
        {
            bool updated = await Db.UpdateProductLocalizedAsync(
                connection,
                request.Language,
                productId,
                request.MeasurementUnit,
                request.Description,
                request.Name
            );
            return Success("Localized product data edited successfully.");
        }
        else
        {
            await Db.InsertProductLocalizedAsync(
                connection,
                request.Language,
                productId,
                request.MeasurementUnit,
                request.Description,
                request.Name
            );

            return Success("Localized product data inserted successfully.");
        }
    }

    // // PUT api/admin/product/{id}/localized/{language}
    // [HttpPut("{id:int}/localized/{language}")]
    // [Guard(new[] { Roles.Admin })]
    // public async Task<IActionResult> UpdateProductLocalized([FromRoute] int productId, [FromRoute] string language, [FromBody] ProductLocalizedRequest request)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     bool updated = await Db.UpdateProductLocalizedAsync(
    //         connection,
    //         language,
    //         productId,
    //         request.MeasurementUnit,
    //         request.Description,
    //         request.Name
    //     );

    //     if (updated)
    //         return Success("Localized product data updated successfully.");
    //     return Fail("Failed to update localized product data.");
    // }

    // DELETE api/admin/product/{id}/localized/{language}
    [HttpDelete("{productId:int}/localized/{language}")]
    [Guard(new[] { Roles.Admin })]
    public async Task<IActionResult> DeleteProductLocalized([FromRoute] int productId, [FromRoute] string language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        bool deleted = await Db.DeleteProductLocalizedAsync(connection, language, productId);
        if (deleted)
            return Success("Localized product data deleted successfully.");
        return Fail("Failed to delete localized product data.");
    }

    // GET api/admin/product/{id}/localized/{language}
    [HttpGet("{productId:int}/localized/{language}")]
    [Guard(new[] { Roles.Admin })]
    public async Task<IActionResult> GetProductLocalized([FromRoute] int productId, [FromRoute] int language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var localizedData = await Db.GetProductLocalizedAsync(connection, language, productId);
        if (localizedData is not null)
            return Success(localizedData);
        return Fail("Localized product data not found.");
    }
    // GET api/admin/product/{id}/localized
    [HttpGet("{productId:int}/localized-values")]
    [Guard(new[] { Roles.Admin })]
    public async Task<IActionResult> GetProductLocalizedValuesAsync([FromRoute] int productId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var localizedData = await Db.GetProductLocalizedValuesAsync(connection, productId);
        return Success(localizedData);
    }
}


// Request DTO for inserting/updating localized product data
public class ProductLocalizedRequest
{
    public int Language { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
}
