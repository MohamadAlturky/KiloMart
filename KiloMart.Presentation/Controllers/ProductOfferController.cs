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
public class ProductOfferController : AppController
{
    public ProductOfferController(IDbFactory dbFactory, IUserContext userContext) : base(dbFactory, userContext)
    {
    }

    [HttpPost("add")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromBody] ProductOfferInsertModel request)
    {

        var result = await ProductOfferService.Insert(_dbFactory, _userContext.Get(), request);
        return result.Success ? 
        CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data)
                : StatusCode(500, result.Errors);
    }

    [HttpPut("{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Update(int id, ProductOfferUpdateModel model)
    {
        model.Id = id;

        var result = await ProductOfferService.Update(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return NotFound();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
    [HttpPut("change-price/{id}/{price}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> ChangePrice(int id, decimal price)
    {
        var result = await ProductOfferService.ChangePrice(_dbFactory, _userContext.Get(), id,price);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return NotFound();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
    [HttpDelete("{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await ProductOfferService.DeActivate(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return NotFound();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] int provider, [FromQuery] byte language, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetProductOffersPaginated(connection, provider, language, page, pageSize);
        if (result.ProductOffers is null || result.ProductOffers.Length == 0)
        {
            return NotFound();
        }
        return Ok(new
        {
            Data = result.ProductOffers.Select(e=>new
            {
                ProductId = e.ProductId,
                ProductImageUrl =e.ProductImageUrl,
                ProductIsActive =  e.ProductIsActive,
                ProductDescription = (e.ProductLocalizedDescription??e.ProductDescription),
                ProductMeasurementUnit = (e.ProductLocalizedMeasurementUnit??e.ProductMeasurementUnit),
                ProductName = (e.ProductLocalizedName??e.ProductName),
                e.ProductOfferId,
                e.ProductOfferFromDate,
                e.ProductOfferIsActive,
                e.ProductOfferOffPercentage,
                e.ProductOfferPrice,
                e.ProductOfferQuantity,
                e.ProductProductCategory,
                e.ProductCategoryName
            }).ToList(),

            TotalCount = result.TotalCount
        });
    }
}
