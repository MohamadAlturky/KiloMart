using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
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
        var (Products, TotalCount) = await ProductQuery.GetProductsWithOfferPaginated(connection,category,language,page,pageSize);
        return Success(new { Products , TotalCount });
    }

    [HttpGet("get-best-deals-by-off-percentage")]
    public async Task<IActionResult> GetBestDealsByOffPercentage([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await ProductQuery.GetBestDealsByOffPercentage(connection,language);
        return Success(new {deals = result});
    }
    [HttpGet("get-best-deals-by-final-price")]
    public async Task<IActionResult> GetBestDealsByMultiply([FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var result = await ProductQuery.GetBestDealsByMultiply(connection,language);
        return Success(new {deals = result});
    }
}