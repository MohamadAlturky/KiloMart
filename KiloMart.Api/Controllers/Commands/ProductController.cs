using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Products.Add.Models;
using KiloMart.Domain.Products.Add.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public ProductController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost("add")]
    public IActionResult Insert(ProductDto product){
        var (success, errors) = product.Validate();
        if (!success)
            return BadRequest(errors);

        var result = ProductService.Insert(_dbFactory, product);
        return result.Success ? Ok(result.Data) : BadRequest();
    }
}
