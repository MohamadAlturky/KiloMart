using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Customers.List.Services;
using KiloMart.Domain.Register.Customer.Models;
using KiloMart.Domain.Register.Customer.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/customer")]
public class CustomerController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;

    public CustomerController(IDbFactory dbFactory, IConfiguration configuration)
    {
        _dbFactory = dbFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customers = await CustomerService.List(_dbFactory);
        return Ok(customers);
    }

    // register a customer
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCustomerDto dto)
    {
        var (success, errors) = dto.Validate();
        if(!success)
        {
            return BadRequest(errors);
        }
        
        var result = await new RegisterCustomerService().Register(_dbFactory,
                            _configuration,
                            dto.Email,
                            dto.Password, 
                            dto.DisplayName, 
                            dto.Language);
        return Ok(result);
    }
}
