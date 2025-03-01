using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdfaPayApi.Core.Interfaces;
using EdfaPayApi.Core.Models;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using Microsoft.Extensions.Configuration;

namespace EdfaPayApi.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IDbFactory _dbFactory;
    private string _merchantPassword;
    private readonly string _clientKey;

    public PaymentService(HttpClient httpClient, IConfiguration configuration, IDbFactory dbFactory)
    {
        _httpClient = httpClient;
        //_merchantPassword = configuration["PaymentGateway:MerchantPassword"];
        _clientKey = configuration["PaymentGateway:MerchantKey"];
        _httpClient.BaseAddress = new Uri("https://api.edfapay.com/");
        _dbFactory = dbFactory;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        _merchantPassword = request.MerchantPassword;
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["payer_country"] = request.PayerCountry,
            ["payer_address"] = request.PayerAddress,
            ["order_amount"] = request.OrderAmount.ToString("F2"),
            ["action"] = request.Action,
            ["card_cvv2"] = request.CardCvv2,
            ["payer_zip"] = request.PayerZip,
            ["order_id"] = request.OrderId,
            ["payer_ip"] = request.PayerIp,
            ["order_currency"] = request.OrderCurrency,
            ["payer_first_name"] = request.PayerFirstName,
            ["card_exp_month"] = request.CardExpMonth,
            ["payer_city"] = request.PayerCity,
            ["auth"] = request.Auth,
            ["card_exp_year"] = request.CardExpYear,
            ["payer_last_name"] = request.PayerLastName,
            ["payer_phone"] = request.PayerPhone,
            ["order_description"] = request.OrderDescription,
            ["payer_email"] = request.PayerEmail,
            ["card_number"] = request.CardNumber,
            ["term_url_3ds"] = request.TermUrl3ds,
            ["hash"] = request.Hash,
            ["client_key"] = _clientKey,
            ["recurring_init"] = request.RecurringInit,
            ["req_token"] = request.ReqToken
        });

        var response = await _httpClient.PostAsync("payment/post", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        System.Console.WriteLine($"API Response: {responseContent}");

        var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseContent, new JsonSerializerOptions
        {
            // PropertyNameCaseInsensitive = true,
        });
        if(paymentResponse is null)
        {
            throw new Exception("Payment response is null");
        }
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var requestId = await Db.InsertPaymentRequestAsync(connection, new PaymentRequestTable
        {
            Action = request.Action,
            OrderAmount = request.OrderAmount,
            OrderCurrency = request.OrderCurrency,
            OrderId = request.OrderId,
            PayerAddress = request.PayerAddress,
            PayerCity = request.PayerCity,
            PayerCountry = request.PayerCountry,
            PayerEmail = request.PayerEmail,
            PayerFirstName = request.PayerFirstName,
            PayerLastName = request.PayerLastName,
            PayerPhone = request.PayerPhone,
            PayerZip = request.PayerZip,
            RecurringInit = request.RecurringInit,
            ReqToken = request.ReqToken,
            TermUrl3ds = request.TermUrl3ds,
            CardExpMonth = request.CardExpMonth,
            CardExpYear = request.CardExpYear,
            CardCvv2 = request.CardCvv2,
            Hash = request.Hash,
            PayerIp = request.PayerIp,
            OrderDescription = request.OrderDescription,
            CardNumber = request.CardNumber,
            CreatedAt = DateTime.UtcNow,
            Auth = request.Auth,
            OrderIdInSystem = request.OrderIdInSystem
        }, transaction);
        var responseId = await Db.InsertPaymentResponseAsync(connection, new PaymentResponseTable
        {
            PaymentRequestId = requestId,
            Action = paymentResponse.action,
            Result = paymentResponse.result,
            Status = paymentResponse.status,
            OrderId = paymentResponse.order_id,
            TransId = paymentResponse.trans_id,
            TransDate = paymentResponse.trans_date,
            Amount = paymentResponse.amount,
            Currency = paymentResponse.currency,
            RedirectUrl = paymentResponse.redirect_url,
            RedirectParams = paymentResponse.redirect_params.body,
            RedirectMethod = paymentResponse.redirect_method,
            ErrorCode = paymentResponse.error_code,
            ErrorMessage = paymentResponse.error_message,
            Errors = JsonSerializer.Serialize(paymentResponse.errors),
            Body = responseContent
        }, transaction);

        if (paymentResponse.IsError)
        {
            // Log the error details
            // System.Console.WriteLine($"Payment Error: {paymentResponse.ErrorMessage}");
            // if (paymentResponse.errors?.Any() == true)
            // {
            //     foreach (var error in paymentResponse.errors)
            //     {
            //         System.Console.WriteLine($"- {error.error_message} (Code: {error.error_code})");
            //     }
            // }
        }

        return paymentResponse;
    }

    public string GenerateHash(string email, string cardNumber, string merchantPassword)
    {
        var reversedEmail = new string(email.Reverse().ToArray());
        var cardPart = cardNumber[..6] + cardNumber[^4..];
        var reversedCardPart = new string(cardPart.Reverse().ToArray());

        var finalString = (reversedEmail + merchantPassword + reversedCardPart).ToUpper();

        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(finalString);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }
}