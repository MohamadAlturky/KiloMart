using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;


namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/products-with-in-favorite-and-in-cart")]
public class GetProductDetailsWithInFavoriteAndOnCartController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("list")]
    public async Task<IActionResult> GetProductDetailsWithInFavoriteAndOnCartList(
        [FromQuery] byte language
    )
    {
        var customer = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsWithInFavoriteAndOnCartAsync(
            language,
            customer,
            connection);

        return Success(result);
    }
    [HttpGet("most-ordered")]
    public async Task<IActionResult> GetTopSellingProductDetailsAsync(
        [FromQuery] byte language,
        [FromQuery] int count

    )
    {
        var customer = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetTopSellingProductDetailsAsync(
            language,
            customer,
            count,
            connection);

        return Success(result);
    }

    [HttpGet("with-pricing/list/paginated")]
    public async Task<IActionResult> GetPaginatedProductDetailsWithPricing(
       [FromQuery] byte language,
       [FromQuery] int pageNumber,
       [FromQuery] int pageSize)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsWithPricingWithInFavoriteAndOnCartAsync(
            language,
            pageNumber,
            pageSize,
            customer,
            connection);

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
    public async Task<IActionResult> GetProductDetailsWithPricingWithInFavoriteAndOnCartAsync(
       [FromQuery] byte language,
       [FromQuery] int pageNumber,
       [FromQuery] int category,
       [FromQuery] int pageSize)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsWithPricingByCategoryWithInFavoriteAndOnCartAsync(
            language,
            pageNumber,
            pageSize,
            category,
            customer,
            connection);

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
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.GetProductDetailsFilteredWithInFavoriteAndOnCartAsync(connection,
            language,
            customer,
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

