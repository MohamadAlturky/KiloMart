using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/products")]
public class GetProductDetailsController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("list")]
    public async Task<IActionResult> GetProductDetailsList(
        [FromQuery] byte language
    )
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsAsync(language, connection);

        return Success(result);
    }
    [HttpGet("with-pricing/list")]
    public async Task<IActionResult> GetAllProductDetailsWithPricing(
            [FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsWithPricingAsync(language, connection);

        return Success(result);
    }

    // Helper method to standardize success responses.
    private IActionResult Success(object data)
    {
        return Ok(new
        {
            Success = true,
            Data = data
        });
    }
    [HttpGet("with-pricing/list/paginated")]
    public async Task<IActionResult> GetPaginatedProductDetailsWithPricing(
       [FromQuery] byte language,
       [FromQuery] int pageNumber,
       [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsWithPricingAsync(language, pageNumber, pageSize, connection);

        return Success(new
        {
            Items = result.Items,
            result.TotalPages,
            result.TotalCount,
            result.PageSize,
            result.PageNumber
        });
    }
}

