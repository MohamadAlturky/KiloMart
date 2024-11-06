using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Customers.List.Services;
using KiloMart.Domain.Register.Delivery.Models;
using KiloMart.Domain.Register.Delivery.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/delivery")]
public class DeliveryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;

    public DeliveryController(IDbFactory dbFactory, IConfiguration configuration)
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

    // register a provider
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDeliveryDto dto)
    {
        var (success, errors) = dto.Validate();
        
        if(!success)
        {
            return BadRequest(errors);
        }
        
        var result = await RegisterDeliveryService.Register(
            _dbFactory,
            _configuration,
            dto.Email,
            dto.Password, 
            dto.DisplayName, 
            dto.Language);
        return Ok(result);
    }
}
