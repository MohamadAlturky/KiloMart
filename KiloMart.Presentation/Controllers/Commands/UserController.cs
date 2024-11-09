using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Login.Services;
using KiloMart.Domain.Register.Activate;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Commands;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;
    private readonly IUserContext _userContext;
    public UserController(IDbFactory dbFactory, IConfiguration configuration, IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
        _configuration = configuration;
    }
    #region decode token
    [HttpGet("decode-token")]
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
    [HttpGet("user-payload")]
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
            Ok(new { Success = true, result.Token })
        : Unauthorized(new { Success = false });
    }
    #endregion

    #region emails management
    [HttpPost("activate/email")]
    public async Task<IActionResult> ActivateUserByEmail([FromBody] string email)
    {
        var result = await UserAccountService.ActivateUser(email, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }

    [HttpPost("deactivate/email")]
    public async Task<IActionResult> DeactivateUserByEmail([FromBody] string email)
    {
        var result = await UserAccountService.DeActivateUser(email, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }

    [HttpPost("activate/{id}")]
    public async Task<IActionResult> ActivateUserById(int id)
    {
        var result = await UserAccountService.ActivateUser(id, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }

    [HttpPost("deactivate/{id}")]
    public async Task<IActionResult> DeactivateUserById(int id)
    {
        var result = await UserAccountService.DeActivateUser(id, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }
    #endregion
}

