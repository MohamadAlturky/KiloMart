using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
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

    [HttpPost("language/change")]
    [Guard([
        Roles.Admin,
        Roles.Delivery,
        Roles.Provider,
        Roles.Customer])]
    public async Task<IActionResult> ChangeLanguage([FromBody] LanguageDto request)
    {
        int membershipUserId = _userContext.Get().Id;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        if (request.Language != 2 && request.Language != 1)
        {
            return ValidationError(new List<string> { "Invalid language" });
        }

        // Prepare and execute the SQL query using Dapper
        await connection.ExecuteAsync(
           @"
           UPDATE [dbo].[MembershipUser]
            SET Language = @Language 
            WHERE Id = @Id",
           new { Id = membershipUserId, Language = request.Language }
       );
        return Success(new { TheNewLanguage = request.Language, UserId = membershipUserId });
    }
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


    #region reset password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        string email = _userContext.Get().Email;
        // Validate the request
        var (success, errors) = request.Validate();
        if (!success)
            return ValidationError(errors);

        // Call the service to reset the password
        var result = await ResetPasswordService.ChangePassword(
            email,
            request.Code,
            _userContext.Get().Id,
            request.OldPassword,
            request.NewPassword,
            _dbFactory);

        return result.Success ?
            Success("Password has been successfully reset.")
            : Fail(result.Errors);
    }
    [HttpPost("send-reset-password-token")]
    public async Task<IActionResult> SendResetPasswordToken()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        int userId = _userContext.Get().Id;
        string code = GenerateRandomString(4);
        DateTime date = DateTime.Now;

        await Db.InsertResetPasswordCodeAsync(
            connection,
            code,
            userId,
            date);
        return Success(new { Code = code });
    }
    public static string GenerateRandomString(int length)
    {
        const string chars = "0123456789";
        Random random = new();
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
    }

    public class ResetPasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string NewPassword { get; set; } = null!;

        public (bool success, List<string> errors) Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(OldPassword))
                errors.Add("OldPassword is required.");

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
                errors.Add("New password must be at least 6 characters long.");

            return errors.Count == 0 ? (true, errors) : (false, errors);
        }
    }
    #endregion

    #region deactivate user
    [HttpPost("deactivate-user")]
    public async Task<IActionResult> DeactivateUser()
    {
        int userId = _userContext.Get().Id;
        int partyId = _userContext.Get().Party;
        // Call the service to deactivate the user
        var result = await DeActivateUserService.DeactivateUser(userId, partyId, _dbFactory);

        return result.Success ?
            Success("User has been successfully deactivated.")
            : Fail(result.Errors);
    }
    #endregion

    #region logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Success("Just Delete the Token From The Client Side");
    }
    #endregion
}

