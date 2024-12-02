using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Login.Services;
using KiloMart.Domain.Register.Activate;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Authorization;

[ApiController]
[Route("api/user")]
public class UserCommandController : AppController
{
    private readonly IConfiguration _configuration;

    public UserCommandController(IConfiguration configuration, IDbFactory dbFactory, IUserContext userContext) : base(dbFactory, userContext)
    {
        _configuration = configuration;
    }


    #region decode token
    [HttpGet("admin/decode-token")]
    public IActionResult Decode(string token)
    {
        JwtTokenValidator.ValidateToken(token,
            _configuration["Jwt:Key"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!,
            out var decodedToken);
        return Success(decodedToken);
    }
    #endregion

    #region user payload
    [HttpGet("admin/user-payload")]
    public IActionResult Payload()
    {
        return Success(_userContext.Get());
    }
    #endregion

    #region verify email
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] ActivateUserRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return BadRequest(errors);

        var result = await VerifyUserEmailService.VerifyEmail(request.Email, request.VerificationToken, _dbFactory);
        return result ? Success() : Fail();
    }
    #endregion

    #region login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LogInRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return ValidationError(errors);

        var result = await LoginService.Login(request.Email, request.Password, _dbFactory, _configuration);
        return result.Success ?
            Success(new
            {
                result.Token,
                result.Email,
                result.UserName,
                result.Role,
                result.Language
            })
        : Fail("User Name Or Password Is Not Valid");
    }
    #endregion

    #region language
    public class LanguageDto
    {
        public byte Language { get; set; }
    }
    [HttpGet("language/mine")]
    [Guard([
        Roles.Admin,
        Roles.Delivery,
        Roles.Provider,
        Roles.Customer])]
    public async Task<IActionResult> LanguageMine()
    {
        int membershipUserId = _userContext.Get().Id;

        using var connection = _dbFactory.CreateDbConnection();

        try
        {
            // Open the connection
            connection.Open();

            // Prepare and execute the SQL query using Dapper
            var languageDto = await connection.QuerySingleOrDefaultAsync<LanguageDto>(
               "SELECT Language FROM [KiloMartMasterDb].[dbo].[MembershipUser] WHERE Id = @Id",
               new { Id = membershipUserId }
           );

            // Check if a language was found
            if (languageDto is null)
            {
                return DataNotFound("Language preference not found.");
            }
            else
            {
                return Success(new { Language = languageDto.Language });
            }
        }
        catch (Exception ex)
        {
            return Fail(new string[] { ex.Message });
        }
    }
    #endregion

    #region emails management
    [HttpPost("admin/activate/email")]
    public async Task<IActionResult> ActivateUserByEmail([FromBody] string email)
    {
        var result = await UserAccountService.ActivateUser(email, _dbFactory);
        return result ? Success() : Fail();
    }

    [HttpPost("admin/deactivate/email")]
    public async Task<IActionResult> DeactivateUserByEmail([FromBody] string email)
    {
        var result = await UserAccountService.DeActivateUser(email, _dbFactory);
        return result ? Success() : Fail();
    }

    [HttpPost("admin/activate/{id}")]
    public async Task<IActionResult> ActivateUserById(int id)
    {
        var result = await UserAccountService.ActivateUser(id, _dbFactory);
        return result ? Success() : Fail();
    }

    [HttpPost("admin/deactivate/{id}")]
    public async Task<IActionResult> DeactivateUserById(int id)
    {
        var result = await UserAccountService.DeActivateUser(id, _dbFactory);
        return result ? Success() : Fail();
    }
    #endregion
}

