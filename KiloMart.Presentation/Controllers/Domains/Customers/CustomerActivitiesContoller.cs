using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.Queries;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Customers;

[ApiController]
[Route("api/customers")]
public partial class CustomerActivitiesContoller(IDbFactory dbFactory,
 IUserContext userContext) : AppController(dbFactory, userContext)
{

    #region Get best deals
    // [HttpGet("get-best-deals-by-off-percentage")]
    // public async Task<IActionResult> GetBestDealsByOffPercentage([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await ProductQuery.GetBestDealsByOffPercentage(connection, language);
    //     return Success(new { deals = result });
    // }

    // [HttpGet("get-best-deals-by-final-price")]
    // public async Task<IActionResult> GetBestDealsByMultiply([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await ProductQuery.GetBestDealsByMultiply(connection, language);
    //     return Success(new { deals = result });
    // }


    [HttpGet("get-best-deals")]
    public async Task<IActionResult> GetBestDeals([FromQuery] byte language = 1, [FromQuery] int numberOfReturnedRecords = 5)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await Db.GetBestDeals(language, numberOfReturnedRecords, connection);
        return Success(new { deals = result });
    }

    #endregion

    #region min order value
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
    // [HttpGet("products-paginated-with-offer")]
    // public async Task<IActionResult> GetProductsWithOfferPaginatedForCustomer(
    // [FromQuery] int category,
    // [FromQuery] byte language = 1,
    // [FromQuery] int page = 1,
    // [FromQuery] int pageSize = 10)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var (Products, TotalCount) = await ProductQuery.GetProductsWithOfferPaginated(connection, category, language, page, pageSize);
    //     return Success(new { Products, TotalCount });
    // }
    [HttpGet("products-paginated-with-offer")]
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
    // [HttpGet("get-all-products")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> GetByIsActiveProductDetailsAsync([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await ProductQuery.GetByIsActiveProductDetailsAsync(connection, language, true);
    //     return Success(result);
    // }
    // [HttpGet("get-all-products-with-categories")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> GetByIsActiveProductDetailsWithCategoryAsync([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await ProductQuery.GetByIsActiveProductDetailsWithCategoryAsync(connection, language, true);
    //     return Success(result);
    // }
    // [HttpGet("get-all-products-by-category")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> GetAllProductsByCategory([FromQuery] int category,
    // [FromQuery] byte language, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var result = await Query.GetProductsPaginatedByCategory(connection,
    //     category, language, page, pageSize);
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


    [HttpGet("get-monthly-price-summary")]
    [Guard([Roles.Customer])]
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

    // [HttpPost("add-favorite-product")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> AddFavoriteProduct([FromBody] AddFavoriteProductRequest request)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var customerId = _userContext.Get().Party;
    //     var exists = await Db.GetFavoriteProductsByCustomerAndProductAsync(customerId, request.ProductId, connection);
    //     if (exists is not null)
    //     {
    //         return Success(new { id = 0, message = "Product already exists in favorites" });
    //     }
    //     var favoriteProductId = await Db.InsertFavoriteProductAsync(connection, customerId, request.ProductId);
    //     return Success(new { id = favoriteProductId });
    // }

    // [HttpDelete("remove-favorite-product/{id}")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> RemoveFavoriteProduct(long id)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     FavoriteProduct? favorite = await Db.GetFavoriteProductsByIdAsync(id, connection);
    //     if (favorite is null)
    //     {
    //         return DataNotFound("Favorite product not found.");
    //     }
    //     if (favorite.Customer != _userContext.Get().Party)
    //     {
    //         return Fail("un authorized");
    //     }
    //     var deleted = await Db.DeleteFavoriteProductAsync(connection, id);
    //     if (!deleted)
    //     {
    //         return DataNotFound("Favorite product not found.");
    //     }

    //     return Success();
    // }



    public class ToggleFavoriteProductRequest
    {
        public int ProductId { get; set; }
    }

    [HttpPost("toggle-favorite-product")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> ToggleFavoriteProduct([FromBody] ToggleFavoriteProductRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        // Check if the product is already in the favorites
        var exists = await Db.GetFavoriteProductsByCustomerAndProductAsync(customerId, request.ProductId, connection);

        if (exists is not null)
        {
            // If it exists, remove it from favorites
            var deleted = await Db.DeleteFavoriteProductAsync(connection, exists.Id);
            if (!deleted)
            {
                return DataNotFound("Failed to remove favorite product.");
            }

            return Success(new { message = "Product removed from favorites" });
        }
        else
        {
            // If it does not exist, add it to favorites
            var favoriteProductId = await Db.InsertFavoriteProductAsync(connection, customerId, request.ProductId);

            return Success(new { id = favoriteProductId, message = "Product added to favorites" });
        }
    }


    [HttpGet("get-favorite-products")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetFavoriteProductsInfo([FromQuery] byte language = 1)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        var favoriteProducts = await Db.GetFavoriteProductsByCustomerWithInfoAndPricingAsync(customerId, language, 12, null, null, connection);
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

    // [HttpPost("search")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> Search([FromBody] AddSearchTermRequest request)
    // {
    //     var (IsSuccess, Errors) = request.Validate();
    //     if (!IsSuccess)
    //     {
    //         return ValidationError(Errors);
    //     }
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var customerId = _userContext.Get().Party;

    //     // Insert the search term into the database
    //     var searchId = await Db.InsertSearchHistoryAsync(connection, customerId, request.Term);
    //     var productsInfos = await Db.SearchProductsAsync(connection, request.Term, 5);
    //     return Success(new { Id = searchId, });
    // }

    // [HttpGet("get-last-searches/{count}")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> GetLastSearches(int count)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var customerId = _userContext.Get().Party;

    //     // Retrieve the last 'count' searches for the customer
    //     var lastSearches = await Db.GetLastSearchesByCustomerAsync(connection, customerId, count);

    //     return Success(lastSearches);
    // }

    // [HttpDelete("remove-search-term/{id}")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> RemoveSearchTerm(long id)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();

    //     // Retrieve the search history record by ID
    //     SearchHistory? searchHistory = await Db.GetSearchHistoryByIdAsync(id, connection);

    //     if (searchHistory is null)
    //     {
    //         return DataNotFound("Search term not found.");
    //     }

    //     if (searchHistory.Customer != _userContext.Get().Party)
    //     {
    //         return Fail("Unauthorized");
    //     }

    //     // Delete the search history record
    //     var deleted = await Db.DeleteSearchHistoryAsync(connection, id);

    //     if (!deleted)
    //     {
    //         return DataNotFound("Failed to delete search term.");
    //     }

    //     return Success();
    // }


    // public class AddSearchTermRequest
    // {
    //     public string Term { get; set; } = null!;

    //     public (bool Success, string[] Errors) Validate()
    //     {
    //         var errors = new List<string>();

    //         if (Term.Length < 2)
    //             errors.Add("Search Term shouldn't be less than 2");

    //         return (errors.Count == 0, errors.ToArray());
    //     }
    // }
    #endregion

    #region cart
    [HttpPost("cart/add")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> AddCartItem([FromBody] CartItemRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        Cart? cart = await Db.GetCartByCustomerAndProductIdAsync(_userContext.Get().Party, request.Product, connection);
        if (cart is not null)
        {
            await Db.UpdateCartAsync(
                connection,
                cart.Id,
                cart.Product,
                cart.Quantity + request.Quantity,
                cart.Customer);
            return Success(new { id = cart.Id, Status = "Quantity Increased" });

        }
        var cartId = await Db.InsertCartAsync(connection, request.Product, request.Quantity, _userContext.Get().Party);
        return Success(new { id = cartId, Status = "New Cart Item Inserted" });
    }

    [HttpPut("cart/edit/{id}")]
    [Guard([Roles.Customer])]

    public async Task<IActionResult> UpdateCartItem(long id, [FromBody] UpdateCartItemRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cart = await Db.GetCartByIdAsync(id, connection);
        if (cart is null)
        {
            return DataNotFound();

        }
        if (cart.Customer != _userContext.Get().Party)
        {
            return Fail("UnAuthorized");
        }
        cart.Product = request.Product ?? cart.Product;
        cart.Quantity = request.Quantity ?? cart.Quantity;

        var updated = await Db.UpdateCartAsync(connection, id, cart.Product, cart.Quantity, _userContext.Get().Party);

        if (!updated)
            return DataNotFound();

        return Success();
    }

    [HttpDelete("cart/delete/{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> DeleteCartItem(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cart = await Db.GetCartByIdAsync(id, connection);
        if (cart is null)
        {
            return DataNotFound();

        }
        if (cart.Customer != _userContext.Get().Party)
        {
            return Fail("Unauthorized");
        }
        var deleted = await Db.DeleteCartAsync(connection, id);

        if (!deleted)
            return DataNotFound();

        return Success();
    }
    [HttpGet("cart/price-summary")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetCartPriceSummaryForMe()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open(); // Use await to open the connection asynchronously

        // Retrieve the price summary for the specified customer
        int customerId = _userContext.Get().Party;
        var priceSummary = await Query.GetPriceSummaryByCustomerAsync(connection, customerId);

        // Check if price summary is null
        if (priceSummary is null || priceSummary.MinValue is null || priceSummary.MaxValue is null)
        {
            return DataNotFound("No cart items found for this customer.");
        }
        var systemSettings = await Db.GetSystemSettingsByIdAsync(0, connection);

        if (systemSettings is null)
        {
            return DataNotFound("System settings couldn't be found");
        }

        // Return success with the price summary
        return Success(new { priceSummary, systemSettings.DeliveryOrderFee });
    }

    // [HttpGet("cart/mine")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> Min()
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open(); // Use await to open the connection asynchronously
    //     var result = await Db.GetCartsByCustomerAsync(_userContext.Get().Party, connection);
    //     return Success(result);

    // }

    // [HttpGet("cart/mine-with-info")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> CartMinWithInfo([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open(); // Use await to open the connection asynchronously
    //     var result = await Db.GetCartsByCustomerWithProductsInfoAsync(_userContext.Get().Party, language, connection);
    //     return Success(result);

    // }
    // [HttpGet("cart/mine-with-info-and-pricing")]
    // [Guard([Roles.Customer])]
    // public async Task<IActionResult> CartMinWithInfoWithPricing([FromQuery] byte language)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open(); // Use await to open the connection asynchronously
    //     var result = await Db.GetCartsByCustomerWithProductsInfoAndPricingAsync(_userContext.Get().Party, language, connection);
    //     return Success(result);

    // }

    [HttpGet("cart/mine-with-info-and-pricing-by-location")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> CartMinWithInfoWithPricingByLocation(
    [FromQuery] byte language,
    [FromQuery] decimal? longitude,
    [FromQuery] decimal? latitude
    )
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open(); // Use await to open the connection asynchronously


        var settings = await Db.GetSystemSettingsByIdAsync(0, connection);

        if (settings is null)
        {
            return Fail("system settings not found");
        }

        decimal distanceInKm = settings.RaduisForGetProducts;

        var result = await Db.GetCartsByCustomerWithProductsInfoAndPricingWithLocationAsync(
            _userContext.Get().Party,
            language,
            distanceInKm,
            longitude,
            latitude,
            connection);
        return Success(result);

    }
    [HttpGet("cart/mine-with-info-and-pricing-by-location-id")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> CartMinWithInfoWithPricingByLocationId(
        [FromQuery] byte language,
        [FromQuery] int? locationId
    )
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open(); // Use await to open the connection asynchronously


        var settings = await Db.GetSystemSettingsByIdAsync(0, connection);

        if (settings is null)
        {
            return Fail("system settings not found");
        }
        decimal? longitude = null;
        decimal? latitude = null;
        if (locationId.HasValue)
        {
            var location = await Db.GetLocationByIdAsync(locationId.Value, connection);
            if (location is not null)
            {
                longitude = location.Longitude;
                latitude = location.Latitude;
            }
        }

        decimal distanceInKm = settings.RaduisForGetProducts;

        var result = await Db.GetCartsByCustomerWithProductsInfoAndPricingWithLocationAsync(
            _userContext.Get().Party,
            language,
            distanceInKm,
            longitude,
            latitude,
            connection);
        return Success(result);

    }
    #endregion

    #region card
    [HttpPost("card/add")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Insert(CardInsertModel model)
    {
        var result = await CardService.Insert(_dbFactory, _userContext.Get(), model);
        return result.Success ?
              Success(result.Data) :
              Fail(result.Errors);
    }

    [HttpPut("card/edit/{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Update(int id, CardUpdateModel model)
    {
        model.Id = id;
        var result = await CardService.Update(_dbFactory, _userContext.Get(), model);
        if (result.Success)
            return Success(result.Data);

        return result.Errors.Contains("Not Found") ? DataNotFound() : Fail(result.Errors);
    }

    [HttpGet("card/mine")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMine()
    {
        var partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cards = await Query.GetCustomerCards(connection, partyId);
        return Success(cards);
    }
    #endregion

    #region Orders
    [HttpGet("orders/mine-by-status")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMineByStatus([FromQuery] byte language,
    [FromQuery] byte status)
    {
        var result = await ReadOrderService.GetMineByStatusAsync(language,
            status,
            _userContext,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpGet("orders/mine-by-status-test")]
    public async Task<IActionResult> GetMineByStatusTest([FromQuery] byte language,
    [FromQuery] byte status, [FromQuery] int user)
    {
        var result = await ReadOrderService.GetMineByStatusAsync(language,
            status,
            user,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }

    [HttpPost("orders/create")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> CreateOrderRequest(
        [FromBody] CreateOrderRequestModel model)
    {
        if (model is null)
        {
            return ValidationError(new List<string> { "Invalid request model." });
        }

        var userPayload = _userContext.Get();

        var result = await RequestOrderService.Insert(model, userPayload, _dbFactory);

        if (result.Success)
        {
            return Success(result.Data);
        }
        return Fail(result.Errors);
    }
    [HttpPost("orders/change-payment-type")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> UpdateOrderPaymentType(
            [FromBody] UpdateOrderPaymentRequestModel model)
    {
        if (model is null)
        {
            return ValidationError(new List<string> { "Invalid request model." });
        }

        var userPayload = _userContext.Get();

        var result = await ChangeOrderPaymentTypeService.UpdateOrderPaymentType(model, userPayload, _dbFactory);

        if (result.Success)
        {
            return Success(result.Data);
        }
        return Fail(result.Errors);
    }

    [HttpPost("orders/cancel")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Cancel([FromBody] CancelOrderRequest cancelOrderRequest)
    {
        var result = await OrderCancelService.CustomerCancel(
            cancelOrderRequest.OrderId,
            _userContext.Get(),
            _dbFactory
            );
        if (result.Success)
        {
            return Success(new
            {
                Message = "order canceled successfully",
                Order = result.Data
            });
        }
        return Fail(result.Errors);
    }
    #endregion
}