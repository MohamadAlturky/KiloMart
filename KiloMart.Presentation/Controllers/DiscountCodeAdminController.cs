using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
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
            return CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DiscountCodeUpdateModel model)
    {
        model.Id = id;

        var result = await DiscountCodeService.Update(_dbFactory, _userContext.Get(), model);

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
    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        

        var result = await DiscountCodeService.Deactivate(_dbFactory, _userContext.Get(), id);

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
    [HttpPut("activate/{id}")]
    public async Task<IActionResult> Activate(int id)
    {


        var result = await DiscountCodeService.Activate(_dbFactory, _userContext.Get(), id);

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
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (DiscountCodes, TotalCount) = await Query.GetDiscountCodesPaginated(connection, page, pageSize);
        if (DiscountCodes is null || DiscountCodes.Length == 0)
        {
            return NotFound();
        }
        return Ok(new
        {
            Data = DiscountCodes,
            TotalCount = TotalCount
        });
    }
}
