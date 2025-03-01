using System;
using System.Text.Json.Serialization;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Profiles;

[ApiController]
[Route("callbacks")]
public class PaymentsController : AppController
{
    public PaymentsController(
        IDbFactory dbFactory, 
        IUserContext userContext) 
            : base(dbFactory, userContext)
    {
    }

    [HttpPost("payments")]
    public async Task<IActionResult> Pay([FromForm] UnifiedPaymentTransactionResponse response)
    {
        var connection = _dbFactory.CreateDbConnection();
        connection.Open();

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
                RedirectParams = response.RedirectParams.ToString(),
                RedirectMethod = response.RedirectMethod,
                Status = response.Status,
                Result = response.Result,
                Action = response.Action,
                CreatedAt = DateTime.UtcNow
            }
        );
       
        return Ok();
    }

    [HttpGet("success")]
    public async Task<IActionResult> Success()
    {
        return Ok();
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
    [JsonPropertyName("action")]
    public string Action { get; set; } = "SALE";

    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("order_id")]
    public string OrderId { get; set; }

    [JsonPropertyName("trans_id")]
    public string TransactionId { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("trans_date")]
    public DateTime TransactionDate { get; set; }

    // Properties from PaymentTransactionResponse
    [JsonPropertyName("recurring_token")]
    public string RecurringToken { get; set; }

    [JsonPropertyName("schedule_id")]
    public string ScheduleId { get; set; }

    [JsonPropertyName("card_token")]
    public string CardToken { get; set; }

    [JsonPropertyName("card")]
    public string Card { get; set; }

    [JsonPropertyName("card_expiration_date")]
    public string CardExpirationDate { get; set; }

    [JsonPropertyName("descriptor")]
    public string Descriptor { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    // Property from PaymentTransactionDeclinedResponse
    [JsonPropertyName("decline_reason")]
    public string DeclineReason { get; set; }

    // Properties from PaymentTransactionRedirectResponse
    [JsonPropertyName("redirect_url")]
    public string RedirectUrl { get; set; }

    [JsonPropertyName("redirect_params")]
    public object RedirectParams { get; set; }

    [JsonPropertyName("redirect_method")]
    public string RedirectMethod { get; set; }
}