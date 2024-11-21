using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
public class AppController(IDbFactory dbFactory,
IUserContext userContext) : ControllerBase
{
    protected readonly IDbFactory _dbFactory = dbFactory;
    protected readonly IUserContext _userContext = userContext;

    protected IActionResult Success<T>(T data, string? message = null)
    {
        return Ok(new 
        { 
            Status = true,
            Data = data,
            Message = message??"Task Completed Successfully" 
        });
    }
    protected IActionResult Fail(string[] errors, string? message = null)
    {
        return StatusCode(500, new 
        { 
            Status = false,
            Errors = errors,
            Message = message??"Task Failed" 
        });
    }
    protected IActionResult ValidationError(string[] errors, string? message = null)
    {
        return StatusCode(400, new 
        { 
            Status = false,
            Errors = errors,
            Message = message??"Validation Error" 
        });
    }
}
