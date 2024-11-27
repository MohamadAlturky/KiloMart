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
public partial class CustomerActivitiesContoller(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    
    #region Get best deals
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
    #endregion
 
    #region  min order value
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
    #endregion

    #region products
    [HttpGet("products-paginated-with-offer")]
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
    #endregion

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
        FavoriteProduct? favorite = await Db.GetFavoriteProductsByIdAsync(id, connection);
        if (favorite is null)
        {
            return DataNotFound("Favorite product not found.");
        }
        if (favorite.Customer != _userContext.Get().Party)
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

    #region Search History
    [HttpPost("search")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Search([FromBody] AddSearchTermRequest request)
    {
        var (IsSuccess, Errors) = request.Validate();
        if (!IsSuccess)
        {
            return ValidationError(Errors);
        }
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        // Insert the search term into the database
        var searchId = await Db.InsertSearchHistoryAsync(connection, customerId, request.Term);
        var productsInfos = await Db.SearchProductsAsync(connection, request.Term, 5);
        return Success(new { Id = searchId, });
    }

    [HttpGet("get-last-searches/{count}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetLastSearches(int count)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        // Retrieve the last 'count' searches for the customer
        var lastSearches = await Db.GetLastSearchesByCustomerAsync(connection, customerId, count);

        return Success(lastSearches);
    }

    [HttpDelete("remove-search-term/{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> RemoveSearchTerm(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Retrieve the search history record by ID
        SearchHistory? searchHistory = await Db.GetSearchHistoryByIdAsync(id, connection);

        if (searchHistory is null)
        {
            return DataNotFound("Search term not found.");
        }

        if (searchHistory.Customer != _userContext.Get().Party)
        {
            return Fail("Unauthorized");
        }

        // Delete the search history record
        var deleted = await Db.DeleteSearchHistoryAsync(connection, id);

        if (!deleted)
        {
            return DataNotFound("Failed to delete search term.");
        }

        return Success();
    }


    public class AddSearchTermRequest
    {
        public string Term { get; set; } = null!;

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (Term.Length < 2)
                errors.Add("Search Term shouldn't be less than 2");

            return (errors.Count == 0, errors.ToArray());
        }
    }
    #endregion
}