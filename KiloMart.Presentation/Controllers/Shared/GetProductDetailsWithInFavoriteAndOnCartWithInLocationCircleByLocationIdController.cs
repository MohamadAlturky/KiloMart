using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/products-with-in-favorite-and-in-cart-with-location-circle-by-location-id")]
public class GetProductDetailsWithInFavoriteAndOnCartWithInLocationCircleByLocationIdController : AppController
{
    public GetProductDetailsWithInFavoriteAndOnCartWithInLocationCircleByLocationIdController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }

    [HttpGet("best-deals-with-favorite-and-cart")]
    public async Task<IActionResult> GetBestDealsWithInFavoriteAndOnCartAsync(
        [FromQuery] byte language,
        [FromQuery] int totalCount,
        [FromQuery] int? locationId)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

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

        var result = await Db.GetBestDealsWithInFavoriteAndOnCartWithInLocationCircle(
            language,
            totalCount,
            customer,
            distanceInKm,
            longitude,
            latitude,
            connection);

        return Ok(result);
    }

    [HttpGet("top-selling-with-favorite-and-cart")]
    public async Task<IActionResult> GetTopSellingProductDetailsWithInFavoriteAndOnCartAsync(
        [FromQuery] byte language,
        [FromQuery] int count,
        [FromQuery] int? locationId)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();


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

        var result = await Db.GetTopSellingProductDetailsWithInLocationCircleAsync(
            language,
            customer,
            count,
            distanceInKm,
            longitude,
            latitude,
            connection);

        return Ok(result);
    }

    [HttpGet("with-pricing/list/paginated-with-location")]
    public async Task<IActionResult> GetPaginatedProductDetailsWithPricingAndLocationAsync(
        [FromQuery] byte language,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] int? locationId)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();



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

        var result = await Db.GetProductDetailsWithPricingWithInFavoriteAndOnCartWithInLocationCircleAsync(
            language,
            pageNumber,
            pageSize,
            customer,
            distanceInKm,
            longitude,
            latitude,
            connection);

        return Ok(new
        {
            Items = result.Items,
            result.TotalPages,
            result.TotalCount,
            result.PageSize,
            result.PageNumber
        });
    }

    [HttpGet("with-pricing-by-category/list/paginated-with-location")]
    public async Task<IActionResult> GetProductDetailsByCategoryWithPricingAndLocationAsync(
        [FromQuery] byte language,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] int? categoryId,
        [FromQuery] int? locationId)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();



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

        var result = await Db.GetProductDetailsWithPricingByCategoryWithInFavoriteAndOnCartWithInLocationCircleAsync(
            language,
            pageNumber,
            pageSize,
            categoryId,
            customer,
            distanceInKm,
            longitude,
            latitude,
            connection);

        return Ok(new
        {
            Items = result.Items,
            result.TotalPages,
            result.TotalCount,
            result.PageSize,
            result.PageNumber
        });
    }

    [HttpGet("search-with-favorite-and-cart")]
    public async Task<IActionResult> SearchProductDetailsForCustomerWithInFavoriteAndOnCartAsync(
        [FromQuery] int top,
        [FromQuery] byte language,
        [FromQuery] string? searchTerm,
        [FromQuery] int? locationId)
    {
        var customer = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();


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


        var result = await Db.SearchProductDetailsForCustomerWithInLocationCircleAsync(
            top,
            language,
            customer,
            searchTerm,
            distanceInKm,
            longitude,
            latitude,
            connection);

        return Ok(result);
    }
}