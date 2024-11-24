using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.Queries;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Customers;

[ApiController]
[Route("api/customers")]
public class CustomerActivitiesContoller(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("paginated-with-offer")]
    public async Task<IActionResult> GetProductsWithOfferPaginatedForCustomer(
    [FromQuery] int category,
    [FromQuery] byte language = 1,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var (Products, TotalCount) = await ProductQuery.GetProductsWithOfferPaginated(connection, category, language, page, pageSize);
        return Success(new { Products, TotalCount });
    }

    [HttpGet("get-best-deals-by-off-percentage")]
    public async Task<IActionResult> GetBestDealsByOffPercentage([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await ProductQuery.GetBestDealsByOffPercentage(connection, language);
        return Success(new { deals = result });
    }
    [HttpGet("get-best-deals-by-final-price")]
    public async Task<IActionResult> GetBestDealsByMultiply([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await ProductQuery.GetBestDealsByMultiply(connection, language);
        return Success(new { deals = result });
    }
    [HttpGet("get-min-order-value")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMinOrderValue([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await OrdersQuery.GetCheapestOrderByCustomerAndStatusAsync(connection, _userContext.Get().Party);
        if (result is null)
        {
            return DataNotFound();
        }
        var items = await OrdersQuery.GetOrderProductDetailsAsync(connection, result.Id, language);
        return Success(
            new
            {
                orderId = result.Id,
                price = result.TotalPrice,
                items
            });
    }
    [HttpGet("get-all-products")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetByIsActiveProductDetailsAsync([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await ProductQuery.GetByIsActiveProductDetailsAsync(connection, language, true);
        return Success(result);
    }
    [HttpGet("get-all-products-with-categories")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetByIsActiveProductDetailsWithCategoryAsync([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await ProductQuery.GetByIsActiveProductDetailsWithCategoryAsync(connection, language, true);
        return Success(result);
    }
    // [HttpGet("get-total-cart-value")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> GetTotalCartValue()
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await Query.GetProductPriceRangeForCustomer(connection, _userContext.Get().Party);
    //     return Success(result);
    // }
    [HttpGet("get-price-ranges-for-cart-products")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetTotalCartValue()
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await Query.GetProductPriceRangeForCustomer(connection, _userContext.Get().Party);
        return Success(result);
    }
}