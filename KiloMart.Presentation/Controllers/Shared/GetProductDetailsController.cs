using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;


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
    [HttpGet("with-pricing-by-category/list/paginated")]
    public async Task<IActionResult> GetProductDetailsWithPricingAsync(
       [FromQuery] byte language,
       [FromQuery] int pageNumber,
       [FromQuery] int category,
       [FromQuery] int pageSize)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsWithPricingByCategoryAsync(language, pageNumber, pageSize, category, connection);

        return Success(new
        {
            Items = result.Items,
            result.TotalPages,
            result.TotalCount,
            result.PageSize,
            result.PageNumber
        });
    }
    [HttpGet("list/paginated-with-filters")]
    public async Task<IActionResult> GetProductDetailsFilteredAsync(
       [FromQuery] byte language,
       [FromQuery] int? productCategoryId = null,
       [FromQuery] bool? productIsActive = null,
       [FromQuery] bool? dealIsActive = null,
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsFilteredAsync(connection,
            language,
            productCategoryId,
            productIsActive,
            dealIsActive,
            pageNumber,
            pageSize);
        var count = await Db.GetCOUNTProductDetailsFilteredAsync(connection,
            language,
            productCategoryId,
            productIsActive,
            dealIsActive);
        return Success(new
        {
            items = result,
            count
        });
    }
}

