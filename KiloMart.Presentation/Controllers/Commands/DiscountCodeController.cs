using KiloMart.Core.Contracts;
using KiloMart.Domain.DiscountCodes.Models;
using KiloMart.Domain.DiscountCodes.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Commands;

[ApiController]
[Route("api/[controller]")]
public class DiscountCodeController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public DiscountCodeController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateDiscountCodeRequest request)
    {
        var result = DiscountCodeService.Insert(_dbFactory, request);

        if (!result.Success)
            return StatusCode(500, result.Errors);

        return Ok(result.Data);

    }
}
