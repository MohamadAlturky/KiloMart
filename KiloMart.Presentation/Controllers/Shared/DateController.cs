using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.EFCore.Data;
using KiloMart.DataAccess.EFCore.Services;
using KiloMart.Domain.DateServices;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/date")]
public class DateController(
    IDbFactory dbFactory,
    IUserContext userContext, KilomartDbContext context)
    : AppController(dbFactory, userContext)
{
    [HttpGet]
    public IActionResult Get()
    {

        return Ok(new { Now = SaudiDateTimeHelper.GetCurrentTime(), SaudiDateTime = SaudiDateTimeHelper.GetCurrentTime() });
    }
    [HttpGet("order")]
    public async Task<IActionResult> GetOrder([FromQuery] long id)
    {
        var order = await context.GetOrderDtoByExpressionAsync(e => e.Id == id);
        return Ok(order);
    }
    // [HttpPost("send-otp")]
    // public async Task<IActionResult> Send([FromBody] string phoneNumber)
    // {
    //     var otpService = new TwilioOtpService(useTestCredentials: true);

    //     // Generate 4-digit OTP
    //     var otp = otpService.GenerateOtp();


    //     await otpService.SendOtpAsync(phoneNumber, otp);
    //     return Ok("Success");
    // }
}