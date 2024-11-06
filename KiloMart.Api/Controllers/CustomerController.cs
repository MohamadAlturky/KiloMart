using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Customers.List.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/customer")]
public class CustomerController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public CustomerController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customers = await CustomerService.List(_dbFactory);
        return Ok(customers);
    }
}