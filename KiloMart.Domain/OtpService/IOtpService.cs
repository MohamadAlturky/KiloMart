namespace KiloMart.Domain.OtpService;

public interface IOtpService
{
    Task<OtpSendResponse> SendOtp(string phoneNumber, int numberOfDigits = 6, string method = "whatsapp");
    Task<OtpVerifyResponse> VerifyOtp(string phoneNumber, string otpCode);
}

public class OtpSendResponse
{
    public bool Success { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public string? RequestId { get; set; }
    public string? Message { get; set; }
}

public class OtpVerifyResponse
{
    public bool Status { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public string? Message { get; set; }
} 