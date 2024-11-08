using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.CustomerProfiles.Services;
using KiloMart.Domain.Customers.Profile.Models;
using KiloMart.Domain.Languages.Models;
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

    [HttpPost("profile/create")]
    public async Task<IActionResult> CreateProfile(CreateCustomerProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if(!success)
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

        },new CreateCustomerProfileLocalizedRequest
        {
            FirstName = request.FirstName,
            Language = (int)request.LanguageId,
            NationalName = request.NationalName,
            SecondName = request.SecondName
        });
        
        return Ok(result);
    }
}

public class CreateCustomerProfileApiRequest
{
    
    public int Id { get; set; }
    public int Customer { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public Language LanguageId {get;set;}

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Customer <= 0)
            errors.Add("Customer is required");
        if (string.IsNullOrEmpty(FirstName))
            errors.Add("First name is required");
        if (string.IsNullOrEmpty(SecondName))
            errors.Add("Second name is required");
        if (string.IsNullOrEmpty(NationalName))
            errors.Add("National name is required");
        if (string.IsNullOrEmpty(NationalId))
            errors.Add("National ID is required");
        if(LanguageId == 0)
        {
            errors.Add("Language ID is required");
        }
        return (errors.Count == 0, errors.ToArray());
    }


}
