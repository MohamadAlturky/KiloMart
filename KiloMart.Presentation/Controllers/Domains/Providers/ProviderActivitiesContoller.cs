using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.ProductRequests.Add;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Commands.ProductRequests;
using KiloMart.Presentation.Services;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Providers;

[ApiController]
[Route("api/provider")]
public class ProviderActivitiesContoller : AppController
{
    public ProviderActivitiesContoller(IDbFactory dbFactory,
     IUserContext userContext,
     IWebHostEnvironment environment)
     : base(dbFactory, userContext)
    {
        _environment = environment;
    }
    private readonly IWebHostEnvironment _environment;

    #region product request

    [HttpPost("provider/product-request/add")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromForm] ProductRequestInsertWithFileModel request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return ValidationError(errors);
        if (request.ImageFile is null)
        {
            return ValidationError(new List<string> { "File is required" });
        }
        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveFileAsync(request.ImageFile,
            _environment.WebRootPath,
            fileName);


        var result = await ProductRequestService.Insert(_dbFactory, _userContext.Get(), new ProductRequestInsertModel()
        {
            Date = DateTime.Now,
            Description = request.Description,
            ImageUrl = filePath,
            Language = request.Language,
            MeasurementUnit = request.MeasurementUnit,
            Name = request.Name,
            OffPercentage = request.OffPercentage,
            Price = request.Price,
            ProductCategory = request.ProductCategory,
            Quantity = request.Quantity
        });
        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    #endregion

    #region my offers

    [HttpGet("products/my-offers/paginated")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProvider([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] byte language = 1)
    {
        // Validate the page and pageSize parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        var (offers, totalCount) = await Query.GetProductOffersByProvider(_dbFactory, providerId, page, pageSize, language);

        // Return a successful response with the offers and total count
        return Success(new
        {
            Offers = offers,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    [HttpGet("products/my-offers")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProvider([FromQuery] byte language = 1)
    {
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        List<ProductOfferDetails> result = await Query.GetProductOffersByProvider(_dbFactory, providerId, language);

        // Return a successful response with the offers and total count
        return Success(new
        {
            result
        });
    }
    [HttpGet("products/my-offers-by-category")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProviderAndCategory([FromQuery] int category, [FromQuery] byte language = 1)
    {
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        List<ProductOfferDetails> result = await Query.GetProductOffersByProviderAndCategory(_dbFactory, providerId, language, category);

        // Return a successful response with the offers and total count
        return Success(new
        {
            result
        });
    }
    #endregion

    #region orders
    [HttpPost("orders/accept")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> AcceptOrder([FromBody] long orderId)
    {
        var result = await AcceptOrderService.ProviderAccept(orderId, _userContext.Get(), _dbFactory);
        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    #endregion
}