using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KiloMart.Domain.OtpService;
using KiloMart.Presentation.Authentication.Models;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Presentation.Services;



public class OtpService : IOtpService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public OtpService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OtpSettings:ApiKey"] ?? throw new ArgumentNullException("OTP API Key is missing in configuration");
        _baseUrl = configuration["OtpSettings:BaseUrl"] ?? "https://api.authentica.sa/api/v1/";
    }

    public async Task<OtpSendResponse> SendOtp(string phoneNumber, int numberOfDigits = 6, string method = "whatsapp")
    {
        try
        {
            var request = new
            {
                phone = phoneNumber,
                method = method,
                number_of_digits = numberOfDigits,
                otp_format = "numeric",
                is_fallback_on = 0
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-Authorization", _apiKey);

            var response = await _httpClient.PostAsync($"{_baseUrl}send-otp", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<OtpSendResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new OtpSendResponse { Success = false, Errors = new[] { "Failed to parse response" } };
            }
            else
            {
                return new OtpSendResponse
                {
                    Success = false,
                    Errors = new[] { $"API Error: {response.StatusCode}", responseContent }
                };
            }
        }
        catch (Exception ex)
        {
            return new OtpSendResponse
            {
                Success = false,
                Errors = new[] { $"Exception: {ex.Message}" }
            };
        }
    }

    public async Task<OtpVerifyResponse> VerifyOtp(string phoneNumber, string otpCode)
    {
        try
        {
            var request = new
            {
                phone = phoneNumber,
                otp = otpCode
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-Authorization", _apiKey);

            var response = await _httpClient.PostAsync($"{_baseUrl}verify-otp", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<OtpVerifyResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new OtpVerifyResponse { Status = false, Errors = new[] { "Failed to parse response" } };
            }
            else
            {
                return new OtpVerifyResponse
                {
                    Status = false,
                    Errors = new[] { $"API Error: {response.StatusCode}", responseContent }
                };
            }
        }
        catch (Exception ex)
        {
            return new OtpVerifyResponse
            {
                Status = false,
                Errors = new[] { $"Exception: {ex.Message}" }
            };
        }
    }
}
