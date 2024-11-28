using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Drivers;

[ApiController]
[Route("api/constants")]
public partial class ConstantsController(IDbFactory dbFactory, IUserContext userContext)
 : AppController(dbFactory, userContext)
{
    [HttpGet("OrderActivityType")]
    public async Task<IActionResult> GetOrderActivityTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("OrderActivityType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("DiscountType")]
    public async Task<IActionResult> GetDiscountTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("DiscountType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("DocumentType")]
    public async Task<IActionResult> GetDocumentTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("DocumentType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("Language")]
    public async Task<IActionResult> GetLanguages()
    {
        var result = await DatabaseHelper.SelectFromTable("Language", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("OrderRequestStatus")]
    public async Task<IActionResult> GetOrderRequestStatuses()
    {
        var result = await DatabaseHelper.SelectFromTable("OrderRequestStatus", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("OrderStatus")]
    public async Task<IActionResult> GetOrderStatuses()
    {
        var result = await DatabaseHelper.SelectFromTable("OrderStatus", _dbFactory.CreateDbConnection());
        return Success(result);
    }
     [HttpGet("ProductRequestStatus")]
    public async Task<IActionResult> GetProductRequestStatuses()
    {
        var result = await DatabaseHelper.SelectFromTable("ProductRequestStatus", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("Role")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await DatabaseHelper.SelectFromTable("Role", _dbFactory.CreateDbConnection());
        return Success(result);
    }
}