using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Customers.Profile.Models;
using KiloMart.Domain.Customers.Profile.Services;
using KiloMart.Domain.Register.Customer.Models;
using KiloMart.Domain.Register.Customer.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Commands.Customers;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/customer")]
public class CustomerCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;
    private readonly IUserContext _userContext;
    public CustomerCommandController(IDbFactory dbFactory,
    IConfiguration configuration,
    IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
        _configuration = configuration;
    }
    #region register
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
                            dto.DisplayName);
        return Ok(result);
    }
    #endregion

    #region profile
    [HttpPost("profile/add")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> CreateProfile(CreateCustomerProfileApiRequest request)
    {

        var (success, errors) = request.Validate();
        if (!success)
        {
            return BadRequest(errors);
        }
        var customer = _userContext.Get().Party;


        var result = await CustomerProfileService.InsertAsync(_dbFactory,
        new CreateCustomerProfileRequest
        {
            Customer = customer,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalId = request.NationalId,
            NationalName = request.NationalName

        });

        return result.Success ? Ok(result) : StatusCode(500, result.Errors);
    }
    #endregion
}
