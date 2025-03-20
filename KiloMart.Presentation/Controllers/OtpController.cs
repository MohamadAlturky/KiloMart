using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.OtpService;
using KiloMart.Presentation.Controllers;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/otp")]
public class OtpController : AppController
{
    private readonly IOtpService _otpService;

    public OtpController(IDbFactory dbFactory, IOtpService otpService, IUserContext userContext) 
        : base(dbFactory, userContext)
    {
        _otpService = otpService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrEmpty(request.PhoneNumber))
        {
            return ValidationError(new[] { "Phone number is required" });
        }

        // Ensure phone number is in international format
        var phoneNumber = EnsureInternationalFormat(request.PhoneNumber);
        
        var result = await _otpService.SendOtp(
            phoneNumber);

        if (result.Success)
        {
            return Success(new { 
                Message = "OTP sent successfully", 
                RequestId = result.RequestId 
            });
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.OtpCode))
        {
            return ValidationError(new[] { "Phone number and OTP code are required" });
        }

        // Ensure phone number is in international format
        var phoneNumber = EnsureInternationalFormat(request.PhoneNumber);
        
        var result = await _otpService.VerifyOtp(phoneNumber, request.OtpCode);
        // return Success(result);
        if (result.Status)
        {
            return Success(new { Message = "OTP verified successfully" });
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    private string EnsureInternationalFormat(string phoneNumber)
    {
        // Remove any spaces, dashes, or parentheses
        phoneNumber = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());

        // If the number doesn't start with +, add it
        if (!phoneNumber.StartsWith("+"))
        {
            // If the number starts with 00, replace with +
            if (phoneNumber.StartsWith("00"))
            {
                phoneNumber = "+" + phoneNumber.Substring(2);
            }
            // If it's a Saudi number without country code
            else if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10) // Saudi mobile numbers are 10 digits including the leading 0
            {
                phoneNumber = "+966" + phoneNumber.Substring(1); // Replace 0 with +966 (Saudi country code)
            }
            // Otherwise just add the + prefix
            else
            {
                phoneNumber = "+" + phoneNumber;
            }
        }

        return phoneNumber;
    }

    public class SendOtpRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
} 