using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Providers.Profile;
using KiloMart.Domain.Register.Provider.Models;
using KiloMart.Domain.Register.Provider.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/provider")]
public class ProviderCommandController(IConfiguration configuration,
 IDbFactory dbFactory,
  IUserContext userContext) : AppController(dbFactory, userContext)
{
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterProviderDto dto)
    {
        var (success, errors) = dto.Validate();

        if (!success)
        {
            return ValidationError(errors);
        }

        var result = await new RegisterProviderService().Register(
            _dbFactory,
            _configuration,
            dto.Email,
            dto.Password,
            dto.DisplayName);
        return Success(result);
    }

    [HttpPost("profile/create")]
    public async Task<IActionResult> CreateProfile(CreateProviderProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
        {
            return ValidationError(errors);
        }
        var provider = _userContext.Get().Party;

        var result = await ProviderProfileService.InsertAsync(_dbFactory,
        new CreateProviderProfileRequest
        {
            Provider = provider,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalApprovalId = request.NationalApprovalId,
            CompanyName = request.CompanyName,
            OwnerName = request.OwnerName,
            OwnerNationalId = request.OwnerNationalId,
        });
        return result.Success ? Success(result) : Fail(result.Errors);
    }

    #region profile
    [HttpPost("profile/edit")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> EditProfile(UpdateProviderProfileRequest request)
    {

        var result = await ProviderProfileService.UpdateAsync(_dbFactory,
            _userContext.Get(), request);

        return result.Success ? Success(result) : Fail(result.Errors);
    }
    #endregion

    [HttpGet("mine")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetMine()
    {
        var provider = _userContext.Get().Party; // Assuming you have a way to get the current provider
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[ProviderProfile] WHERE [Provider] = @Provider";
        var result = await connection.QueryFirstOrDefaultAsync<ProviderProfile>(query, new { Provider = provider });
        if (result is null)
        {
            return DataNotFound();
        }
        return Success(result);
    }

    // Define a class for ProviderProfile
    public class ProviderProfile
    {
        public int Id { get; set; }
        public int Provider { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string OwnerNationalId { get; set; } = null!;
        public string NationalApprovalId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
    }

    [HttpGet("admin/list")]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetProviderProfilesWithUserInfoPaginated(connection, page, pageSize);
        if (result.Profiles is null || result.Profiles.Length == 0)
        {
            return DataNotFound();
        }
        return Success(
            new
        {
            Data = result.Profiles,
            TotalCount = result.TotalCount
        });
    }
}
