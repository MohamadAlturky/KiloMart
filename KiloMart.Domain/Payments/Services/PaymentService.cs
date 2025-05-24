using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdfaPayApi.Core.Interfaces;
using EdfaPayApi.Core.Models;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EdfaPayApi.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IDbFactory _dbFactory;
    private string _merchantPassword;
    private string _clientKey;
    public PaymentService(HttpClient httpClient, IConfiguration configuration, IDbFactory dbFactory)
    {
        _httpClient = httpClient;
        _merchantPassword = configuration["PaymentGateway:MerchantPassword"]??throw new Exception("Merchant Password is not set");
        _clientKey = configuration["PaymentGateway:MerchantKey"]??throw new Exception("Merchant Key is not set");
        _httpClient.BaseAddress = new Uri("https://api.edfapay.com/");
        _dbFactory = dbFactory;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        // _merchantPassword = request.MerchantPassword;
        // _clientKey = request.MerchantKey;
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
            ["term_url_3ds"] = request.TermUrl3ds + request.OrderId,
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
            RedirectParams = paymentResponse.redirect_params?.body,
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
        transaction.Commit();
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

    public async Task<Core.Interfaces.PaymentStatusEnum> GetPaymentStatusAsync(Core.Interfaces.PaymentRequestResponseDto? transactionData)
    {
        if(transactionData is null || transactionData.TransId is null)
        {
            return MapStatusToEnum("DECLINED");
        }
        var hash = GenerateTransactionStatusHash(transactionData.PayerEmail, transactionData.CardNumber, transactionData.TransId);

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["action"] = "GET_TRANS_STATUS",
            ["client_key"] = _clientKey,
            ["trans_id"] = transactionData.TransId,
            ["hash"] = hash
        });

        var response = await _httpClient.PostAsync("payment/post", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var statusResponse = JsonSerializer.Deserialize<PaymentStatusResponse>(responseContent);
        if (statusResponse == null)
        {
            throw new Exception("Failed to deserialize payment status response");
        }

        return MapStatusToEnum(statusResponse.status);
    }

    private string GenerateTransactionStatusHash(string email, string cardNumber, string transId)
    {
        var reversedEmail = new string(email.Reverse().ToArray());
        var cardPart = cardNumber[..6] + cardNumber[^4..];
        var reversedCardPart = new string(cardPart.Reverse().ToArray());

        var finalString = (reversedEmail + _merchantPassword + transId + reversedCardPart).ToUpper();

        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(finalString);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }

    private Core.Interfaces.PaymentStatusEnum MapStatusToEnum(string status)
    {
        return status.ToUpper() switch
        {
            "3DS" => PaymentStatusEnum.ThreeDSecure,
            "REDIRECT" => PaymentStatusEnum.Redirect,
            "PENDING" => PaymentStatusEnum.Pending,
            "PREPARE" => PaymentStatusEnum.Prepare,
            "DECLINED" => PaymentStatusEnum.Declined,
            "SETTLED" => PaymentStatusEnum.Settled,
            "REVERSAL" => PaymentStatusEnum.Reversal,
            "REFUND" => PaymentStatusEnum.Refund,
            "CHARGEBACK" => PaymentStatusEnum.Chargeback,
            _ => PaymentStatusEnum.Unknown
        };
    }



    private class PaymentStatusResponse
    {
        public string action { get; set; }
        public string result { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public string trans_id { get; set; }
        public string decline_reason { get; set; }
        public string recurring_token { get; set; }
    }
}