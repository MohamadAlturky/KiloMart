using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/discountcode")]
public class DiscountCodeAdminController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpPost]
    public async Task<IActionResult> Insert(DiscountCodeInsertModel model)
    {
        var result = await DiscountCodeService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DiscountCodeUpdateModel model)
    {
        model.Id = id;

        var result = await DiscountCodeService.Update(_dbFactory, _userContext.Get(), model);

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
    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(int id)
    {


        var result = await DiscountCodeService.Deactivate(_dbFactory, _userContext.Get(), id);

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
    [HttpPut("activate/{id}")]
    public async Task<IActionResult> Activate(int id)
    {


        var result = await DiscountCodeService.Activate(_dbFactory, _userContext.Get(), id);

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
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (DiscountCodes, TotalCount) = await Query.GetDiscountCodesPaginated(connection, page, pageSize);
        if (DiscountCodes is null || DiscountCodes.Length == 0)
        {
            return DataNotFound();
        }
        return Success(new
        {
            Data = DiscountCodes,
            TotalCount = TotalCount
        });
    }
    [HttpGet("by-product-id")]
    public async Task<IActionResult> List([FromQuery] int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetDiscountCodesByProductAsync(connection,id);
        if (result is null)
        {
            return DataNotFound();
        }
        return Success(new
        {
            DiscountCodes = result,
        });
    }

    [HttpPost("link-product-with-discount-code")]
    public async Task<IActionResult> InsertProductDiscount([FromBody] InsertProductDiscountRequest request)
    {
        if (request == null || request.Product <= 0 || request.DiscountCode <= 0)
        {
            return ValidationError(new List<string> { "Invalid input data." });
        }

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        try
        {
            long newId = await Db.InsertProductDiscountAsync(connection, request.Product, request.DiscountCode, DateTime.Now);
            return Success(new { Id = newId });
        }
        catch (Exception ex)
        {
            return Fail(new List<string> { ex.Message });
        }
    }

}
public class InsertProductDiscountRequest
{
    public int Product { get; set; }
    public int DiscountCode { get; set; }
}