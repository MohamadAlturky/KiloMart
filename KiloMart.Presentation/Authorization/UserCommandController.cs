using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Login.Services;
using KiloMart.Domain.Register.Activate;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Authorization;

[ApiController]
[Route("api/user")]
public class UserCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;
    private readonly IUserContext _userContext;
    public UserCommandController(IDbFactory dbFactory, IConfiguration configuration, IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
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
        return Ok(decodedToken);
    }
    #endregion

    #region user payload
    [HttpGet("admin/user-payload")]
    public IActionResult Payload()
    {
        return Ok(_userContext.Get());
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
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }
    #endregion

    #region login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LogInRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return BadRequest(errors);

        var result = await LoginService.Login(request.Email, request.Password, _dbFactory, _configuration);
        return result.Success ?
            Ok(new
            {
                Success = true,
                result.Token,
                result.Email,
                result.UserName,
                result.Role
            })
        : Unauthorized(new { Success = false });
    }
    #endregion

    #region emails management
    [HttpPost("admin/activate/email")]
    public async Task<IActionResult> ActivateUserByEmail([FromBody] string email)
    {
        var result = await UserAccountService.ActivateUser(email, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }

    [HttpPost("admin/deactivate/email")]
    public async Task<IActionResult> DeactivateUserByEmail([FromBody] string email)
    {
        var result = await UserAccountService.DeActivateUser(email, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }

    [HttpPost("admin/activate/{id}")]
    public async Task<IActionResult> ActivateUserById(int id)
    {
        var result = await UserAccountService.ActivateUser(id, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }

    [HttpPost("admin/deactivate/{id}")]
    public async Task<IActionResult> DeactivateUserById(int id)
    {
        var result = await UserAccountService.DeActivateUser(id, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }
    #endregion
}

