using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Provider.Profile.Models;
using KiloMart.Domain.Provider.Profile.Services;
using KiloMart.Domain.Register.Provider.Models;
using KiloMart.Domain.Register.Provider.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/provider")]
public class ProviderController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;

    public ProviderController(IDbFactory dbFactory, IConfiguration configuration)
    {
        _dbFactory = dbFactory;
        _configuration = configuration;
    }

    // register a provider
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterProviderDto dto)
    {
        var (success, errors) = dto.Validate();

        if (!success)
        {
            return BadRequest(errors);
        }

        var result = await new RegisterProviderService().Register(
            _dbFactory,
            _configuration,
            dto.Email,
            dto.Password,
            dto.DisplayName,
            dto.Language);
        return Ok(result);
    }

    [HttpPost("profile/create")]
    public async Task<IActionResult> CreateProfile(CreateProviderProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
        {
            return BadRequest(errors);
        }

        var result = await ProviderProfileService.InsertAsync(_dbFactory,
        new CreateProviderProfileRequest
        {
            Provider = request.Provider,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalApprovalId = request.NationalApprovalId,
            CompanyName = request.CompanyName,
            OwnerName = request.OwnerName,
            OwnerNationalId = request.OwnerNationalId,
        }, new CreateProviderProfileLocalizedRequest
        {
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            Language = request.LanguageId,
            CompanyName = request.CompanyName,
            OwnerName = request.OwnerName
        });
        return result.Success ? Ok(result) : StatusCode(500, result.Errors);
    }
}
