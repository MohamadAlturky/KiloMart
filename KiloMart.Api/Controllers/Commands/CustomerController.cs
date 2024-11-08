using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.CustomerProfiles.Services;
using KiloMart.Domain.Customers.Profile.Models;
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
    // register a customer
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCustomerDto dto)
    {
        var (success, errors) = dto.Validate();
        if (!success)
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

    [HttpPost("profile/create")]
    public async Task<IActionResult> CreateProfile(CreateCustomerProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
        {
            return BadRequest(errors);
        }

        var result = await CustomerProfileService.InsertAsync(_dbFactory,
        new CreateCustomerProfileRequest
        {
            Customer = request.Customer,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalId = request.NationalId,
            NationalName = request.NationalName

        }, new CreateCustomerProfileLocalizedRequest
        {
            FirstName = request.FirstName,
            Language = (int)request.LanguageId,
            NationalName = request.NationalName,
            SecondName = request.SecondName
        });

        return result.Success ? Ok(result) : StatusCode(500, result.Errors);
    }
}
