using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Login.Services;
using KiloMart.Domain.Register.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Commands;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;
    public UserController(IDbFactory dbFactory, IConfiguration configuration)
    {
        _dbFactory = dbFactory;
        _configuration = configuration;
    }
    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateUserRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return BadRequest(errors);

        var result = await ActivateUserService.ActivateUser(request.Email, request.VerificationToken, _dbFactory);
        return result ? Ok(new { Success = true }) : StatusCode(500, new { Success = false });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LogInRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return BadRequest(errors);

        var result = await LoginService.Login(request.Email, request.Password, _dbFactory, _configuration);
        return result.Success ? 
            Ok(new { Success = true, Token = result.Token }) 
        : Unauthorized(new { Success = false });
    }
}

