using KiloMart.Core.Contracts;
using KiloMart.Domain.DiscountCodes.Models;
using KiloMart.Domain.DiscountCodes.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/discount")]
public class DiscountCodeCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public DiscountCodeCommandController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    public IActionResult Create([FromBody] CreateDiscountCodeRequest request)
    {
        var result = DiscountCodeService.Insert(_dbFactory, request);

        if (!result.Success)
            return StatusCode(500, result.Errors);

        return Ok(result.Data);

    }
}
