using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
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
    [HttpGet("get-all-products-by-category")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetAllProductsByCategory([FromQuery] int category,
    [FromQuery] byte language, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await Query.GetProductsPaginatedByCategory(connection,
        category, language, page, pageSize);
        return Success(result);
    }

    [HttpGet("get-price-ranges-for-cart-products")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetTotalCartValue()
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await Query.GetProductPriceRangeForCustomer(connection, _userContext.Get().Party);
        return Success(result);
    }
    [HttpGet("get-monthly-price-summary")]
    //[Guard([Roles.Customer])]
    public async Task<IActionResult> GetMonthlyPriceSummary([FromQuery] int product,
    [FromQuery] byte numberOfMonths)
    {
        if (numberOfMonths > 24)
        {
            return ValidationError(new List<string> { "number of months should not be more than 24" });
        }
        using var connection = _dbFactory.CreateDbConnection();
        var result = await Query.GetMonthlyPriceSummary(connection, numberOfMonths, product);
        return Success(result);
    }


    #region Favorites

    [HttpPost("add-favorite-product")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> AddFavoriteProduct([FromBody] AddFavoriteProductRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        var favoriteProductId = await Db.InsertFavoriteProductAsync(connection, customerId, request.ProductId);
        return Success(new { id = favoriteProductId });
    }

    [HttpDelete("remove-favorite-product/{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> RemoveFavoriteProduct(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        FavoriteProduct? favorite = await Db.GetFavoriteProductsByIdAsync(id,connection);
        if(favorite is null)
        {
            return DataNotFound("Favorite product not found.");
        }
        if(favorite.Customer != _userContext.Get().Party)
        {
            return Fail("un authorized");
        }
        var deleted = await Db.DeleteFavoriteProductAsync(connection, id);
        if (!deleted)
        {
            return DataNotFound("Favorite product not found.");
        }

        return Success();
    }

    [HttpGet("get-favorite-products")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetFavoriteProducts()
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        var favoriteProducts = await Db.GetFavoriteProductsByCustomerIdAsync(customerId, connection);
        return Success(favoriteProducts);
    }
    [HttpGet("get-favorite-products-with-details")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetFavoriteProducts([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;
        var favoriteProducts = await Db.GetFavoriteProductDetailsByCustomerIdAsync(customerId, language, connection);
        return Success(favoriteProducts);
    }

    public class AddFavoriteProductRequest
    {
        public int ProductId { get; set; }
    }
    #endregion
}