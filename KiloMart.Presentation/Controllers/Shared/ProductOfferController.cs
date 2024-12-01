using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/product-offer")]
public class ProductOfferController(
    IDbFactory dbFactory,
    IUserContext userContext) 
    : AppController(dbFactory, userContext)
{
    [HttpPost("provider/add")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromBody] ProductOfferInsertModel request)
    {

        var result = await ProductOfferService.Insert(_dbFactory, _userContext.Get(), request);
        return result.Success ? 
        Success( result.Data)
                : Fail(result.Errors);
    }

    [HttpPut("provider/{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Update(int id, ProductOfferUpdateModel model)
    {
        model.Id = id;

        var result = await ProductOfferService.Update(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }
    [HttpPut("provider/change-price/{id}/{price}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> ChangePrice(int id, decimal price)
    {
        var result = await ProductOfferService.ChangePrice(_dbFactory, _userContext.Get(), id,price);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }
    [HttpDelete("provider/{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await ProductOfferService.DeActivate(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] int provider, 
    [FromQuery] byte language = 1, 
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (ProductOffers, TotalCount) = await Query.GetProductOffersPaginated(connection, provider, language, page, pageSize);
        if (ProductOffers is null || ProductOffers.Length == 0)
        {
            return DataNotFound();
        }
        return Success(new
        {
            Data = ProductOffers.Select(e=>new
            {
                e.ProductId,
                e.ProductImageUrl,
                e.ProductIsActive,
                ProductDescription = e.ProductLocalizedDescription??e.ProductDescription,
                ProductMeasurementUnit = e.ProductLocalizedMeasurementUnit??e.ProductMeasurementUnit,
                ProductName = e.ProductLocalizedName??e.ProductName,
                e.ProductOfferId,
                e.ProductOfferFromDate,
                e.ProductOfferIsActive,
                e.ProductOfferOffPercentage,
                e.ProductOfferPrice,
                e.ProductOfferQuantity,
                e.ProductProductCategory,
                e.ProductCategoryName
            }).ToList(),

            TotalCount
        });
    }

    [HttpGet("provider/my-offers")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Mine(
    [FromQuery] byte language, 
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        
        int provider = _userContext.Get().Party;

        var (ProductOffers, TotalCount) = await Query.GetProductOffersPaginated(connection, provider, language, page, pageSize);
        if (ProductOffers is null || ProductOffers.Length == 0)
        {
            return DataNotFound();
        }
        return Success(
            new
        {
            Data = ProductOffers.Select(e=>new
            {
                e.ProductId,
                e.ProductImageUrl,
                e.ProductIsActive,
                ProductDescription = e.ProductLocalizedDescription??e.ProductDescription,
                ProductMeasurementUnit = e.ProductLocalizedMeasurementUnit??e.ProductMeasurementUnit,
                ProductName = e.ProductLocalizedName??e.ProductName,
                e.ProductOfferId,
                e.ProductOfferFromDate,
                e.ProductOfferIsActive,
                e.ProductOfferOffPercentage,
                e.ProductOfferPrice,
                e.ProductOfferQuantity,
                e.ProductProductCategory,
                e.ProductCategoryName
            }).ToList(),

            TotalCount
        });
    }
}
