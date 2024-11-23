using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/provider")]
public class ProviderActivitiesContoller : AppController
{
    public ProviderActivitiesContoller(IDbFactory dbFactory, IUserContext userContext)
     : base(dbFactory, userContext)
    {
    }

    [HttpGet("products/my-offers/paginated")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProvider([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] byte language = 1)
    {
        // Validate the page and pageSize parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        int providerId = _userContext.Get().Party; 

        // Fetch the product offers and total count
        var (offers, totalCount) = await Query.GetProductOffersByProvider(_dbFactory, providerId, page, pageSize,language);

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
    public async Task<IActionResult> GetOffersByProviderAndCategory([FromQuery] int category,[FromQuery] byte language = 1)
    {
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        List<ProductOfferDetails> result = await Query.GetProductOffersByProviderAndCategory(_dbFactory, providerId, language,category);

        // Return a successful response with the offers and total count
        return Success(new 
        {
            result
        });
    }
}