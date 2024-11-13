using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Products.Offers.Services;
using KiloMart.Presentation.Models.Commands.Products;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/product-offer")]
public class ProductOfferCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public ProductOfferCommandController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpPost("add")]
    public IActionResult Insert([FromBody] CreateProductOfferRequest request)
    {

        var (success, errors) = request.Validate();
        var party = _userContext.Get().Party;
        if (!success)
        {
            return BadRequest(errors);
        }

        var result = ProductOfferService.Insert(_dbFactory, new Domain.Products.Offers.Models.ProductOfferDto()
        {
            IsActive = true,
            FromDate = request.FromDate,
            Price = request.Price,
            OffPercentage = request.OffPercentage,
            Provider = party,
            Product = request.ProductId,
            Quantity = request.Quantity,
            ToDate = null
        });
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }
}
