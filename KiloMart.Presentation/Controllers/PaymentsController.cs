using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using EdfaPayApi.Core.Interfaces;
using EdfaPayApi.Core.Models;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Profiles;

[ApiController]
[Route("callbacks")]
public class PaymentsController : AppController
{

    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    public PaymentsController(
        IDbFactory dbFactory,
        IConfiguration configuration,
        IUserContext userContext,
        IPaymentService paymentService)
            : base(dbFactory, userContext)
    {
        _paymentService = paymentService;
        _configuration = configuration;
    }
    [HttpGet("test-delete-orders")]
    public async Task<IActionResult> TestDeleteOrders()
    {
        var orderDeleteService = new OrderDeleteService(_dbFactory);
        await orderDeleteService.Cancel();
        return Ok();
    }

    [HttpPost("sale")]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment(PaymentRequestMini requestMini)
    {
        var request = requestMini.ToPaymentRequest();
        // Generate hash
        request.Hash = _paymentService.GenerateHash(
            request.PayerEmail,
            request.CardNumber,
            _configuration["PaymentGateway:MerchantPassword"]
        );

        var response = await _paymentService.ProcessPaymentAsync(request);
        return Ok(response);
    }

    // public string GenerateHash(string email, string cardNumber, string merchantPassword)
    // {
    //     var reversedEmail = new string(email.Reverse().ToArray());
    //     var cardPart = cardNumber[..6] + cardNumber[^4..];
    //     var reversedCardPart = new string(cardPart.Reverse().ToArray());

    //     var finalString = (reversedEmail + merchantPassword + reversedCardPart).ToUpper();

    //     using var md5 = MD5.Create();
    //     var inputBytes = Encoding.ASCII.GetBytes(finalString);
    //     var hashBytes = md5.ComputeHash(inputBytes);

    //     return Convert.ToHexString(hashBytes).ToLower();
    // }
    private string CalculateHash(string email, string password, string transId, string cardNumber)
    {
        // Reverse the email and convert to uppercase
        string reversedEmail = new string(email.Reverse().ToArray()).ToUpper();

        // Substring of card number: first 6 digits and last 4 digits
        string cardFirstSix = cardNumber.Substring(0, 6);
        string cardLastFour = cardNumber.Substring(cardNumber.Length - 4);

        // Concatenate components as per the formula
        string hashInput = reversedEmail + password + transId + cardFirstSix + cardLastFour;

        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(hashInput);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();

        // // Calculate MD5 hash
        // using (MD5 md5 = MD5.Create())
        // {
        //     byte[] inputBytes = Encoding.ASCII.GetBytes(hashInput);
        //     byte[] hashBytes = md5.ComputeHash(inputBytes);

        //     // Convert byte array to hexadecimal string
        //     StringBuilder sb = new StringBuilder();
        //     for (int i = 0; i < hashBytes.Length; i++)
        //     {
        //         sb.Append(hashBytes[i].ToString("X2"));
        //     }
        //     return sb.ToString();
        // }
    }

    [HttpPost("payments")]
    public async Task<IActionResult> Pay([FromForm] UnifiedPaymentTransactionResponse response)
    {
        var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        // Calculate hash
        var _merchantPassword = _configuration["PaymentGateway:MerchantPassword"] ?? throw new Exception("Merchant Password is not set");
        var _clientKey = _configuration["PaymentGateway:MerchantKey"] ?? throw new Exception("Merchant Key is not set");

        string calculatedHash = CalculateHash(
            "adnanh@expresspay.sa",
            _merchantPassword,
            response.TransactionId,
            response.Card
        );

        // Validate the received hash with the calculated hash
        if (!string.Equals(calculatedHash, response.Hash, StringComparison.OrdinalIgnoreCase))
        {
            // Hash validation failed
            return BadRequest("Hash validation failed.");
        }

        var id = await Db.InsertPaymentTransactionAsync(
            connection,
            new PaymentTransactionTable
            {
                OrderId = response.OrderId,
                TransactionId = response.TransactionId,
                Hash = response.Hash,
                TransactionDate = response.TransactionDate,
                RecurringToken = response.RecurringToken,
                ScheduleId = response.ScheduleId,
                CardToken = response.CardToken,
                Card = response.Card,
                CardExpirationDate = response.CardExpirationDate,
                Descriptor = response.Descriptor,
                Amount = response.Amount,
                Currency = response.Currency,
                DeclineReason = response.DeclineReason,
                RedirectUrl = response.RedirectUrl,
                RedirectParams = response.RedirectParams?.ToString() ?? "",
                RedirectMethod = response.RedirectMethod,
                Status = response.Status,
                Result = response.Result,
                Action = response.Action ?? "",
                CreatedAt = DateTime.UtcNow
            }
        );

        if (response.Status == "SUCCESS")
        {
            try
            {
                await OrdersDb.UpdateOrderIsPaidAsync(connection,
                    long.Parse(response.OrderId!),
                    true);
            }
            catch (Exception)
            {

            }
        }
        return Ok();
    }
    // [HttpPost("payments")]
    // public async Task<IActionResult> Pay([FromForm] string response)
    // {
    //     var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var id = await Db.InsertPaymentTransactionAsync(
    //         connection,
    //         new PaymentTransactionTable
    //         {
    //             OrderId = "1",
    //             TransactionId = "response.TransactionId",
    //             Hash = "response.Hash",
    //             TransactionDate = "response.TransactionDate",
    //             RecurringToken = "response.RecurringToken",
    //             ScheduleId = "response.ScheduleId",
    //             CardToken = "response.CardToken",
    //             Card = "response.Card",
    //             CardExpirationDate = "response.CardExpirationDate",
    //             Descriptor = "response.Descriptor",
    //             Amount = 10,
    //             Currency = "response.Currency",
    //             DeclineReason = "response.DeclineReason",
    //             RedirectUrl = "response.RedirectUrl",
    //             RedirectParams = response,
    //             RedirectMethod = "response.RedirectMethod",
    //             Status = "response.Status",
    //             Result = "response.Result",
    //             Action = "response.Action",
    //             CreatedAt = DateTime.UtcNow
    //         }
    //     );

    //     return Ok();
    // }

    [HttpGet("success")]
    public async Task<IActionResult> Success()
    {
        return Ok();
    }


    [HttpGet("test/orders-with-available-stock")]
    public async Task<IActionResult> GetOrdersWithAvailableStock([FromQuery] int providerId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var result = await Db.GetOrdersWithAvailableStockAsync(
            connection,
            providerId);

        return Success(new { Result = result });
    }

}

// public abstract class PaymentTransactionBaseResponse
// {
//     [JsonPropertyName("action")]
//     public string Action { get; set; } = "SALE";

//     [JsonPropertyName("result")]
//     public string Result { get; set; }

//     [JsonPropertyName("status")]
//     public string Status { get; set; }

//     [JsonPropertyName("order_id")]
//     public string OrderId { get; set; }

//     [JsonPropertyName("trans_id")]
//     public string TransactionId { get; set; }

//     [JsonPropertyName("hash")]
//     public string Hash { get; set; }

//     [JsonPropertyName("trans_date")]
//     public DateTime TransactionDate { get; set; }
// }

// public class PaymentTransactionResponse : PaymentTransactionBaseResponse
// {
//     [JsonPropertyName("recurring_token")]
//     public string RecurringToken { get; set; }

//     [JsonPropertyName("schedule_id")]
//     public string ScheduleId { get; set; }

//     [JsonPropertyName("card_token")]
//     public string CardToken { get; set; }

//     [JsonPropertyName("card")]
//     public string Card { get; set; }

//     [JsonPropertyName("card_expiration_date")]
//     public string CardExpirationDate { get; set; }

//     [JsonPropertyName("descriptor")]
//     public string Descriptor { get; set; }

//     [JsonPropertyName("amount")]
//     public decimal Amount { get; set; }

//     [JsonPropertyName("currency")]
//     public string Currency { get; set; }
// }

// public class PaymentTransactionDeclinedResponse : PaymentTransactionBaseResponse
// {
//     [JsonPropertyName("decline_reason")]
//     public string DeclineReason { get; set; }
// }

// public class PaymentTransactionRedirectResponse : PaymentTransactionBaseResponse
// {
//     [JsonPropertyName("descriptor")]
//     public string Descriptor { get; set; }

//     [JsonPropertyName("amount")]
//     public decimal Amount { get; set; }

//     [JsonPropertyName("currency")]
//     public string Currency { get; set; }

//     [JsonPropertyName("redirect_url")]
//     public string RedirectUrl { get; set; }

//     [JsonPropertyName("redirect_params")]
//     public object RedirectParams { get; set; }

//     [JsonPropertyName("redirect_method")]
//     public string RedirectMethod { get; set; }
// }

public class UnifiedPaymentTransactionResponse
{
    [FromForm(Name = "action")]
    public string? Action { get; set; } = "SALE";

    [FromForm(Name = "result")]
    public string? Result { get; set; }

    [FromForm(Name = "status")]
    public string? Status { get; set; }

    [FromForm(Name = "order_id")]
    public string? OrderId { get; set; }

    [FromForm(Name = "trans_id")]
    public string? TransactionId { get; set; }

    [FromForm(Name = "hash")]
    public string? Hash { get; set; }

    [FromForm(Name = "trans_date")]
    public string? TransactionDate { get; set; }

    // Properties from PaymentTransactionResponse
    [FromForm(Name = "recurring_token")]
    public string? RecurringToken { get; set; }

    [FromForm(Name = "schedule_id")]
    public string? ScheduleId { get; set; }

    [FromForm(Name = "card_token")]
    public string? CardToken { get; set; }

    [FromForm(Name = "card")]
    public string? Card { get; set; }

    [FromForm(Name = "card_expiration_date")]
    public string? CardExpirationDate { get; set; }

    [FromForm(Name = "descriptor")]
    public string? Descriptor { get; set; }

    [FromForm(Name = "amount")]
    public decimal? Amount { get; set; }

    [FromForm(Name = "currency")]
    public string? Currency { get; set; }

    [FromForm(Name = "decline_reason")]
    public string? DeclineReason { get; set; }

    [FromForm(Name = "redirect_url")]
    public string? RedirectUrl { get; set; }

    [FromForm(Name = "redirect_params")]
    public string? RedirectParams { get; set; }

    [FromForm(Name = "redirect_method")]
    public string? RedirectMethod { get; set; }
}