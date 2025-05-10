using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapper;
using EdfaPayApi.Core.Interfaces;
using EdfaPayApi.Core.Models;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    public async Task<IActionResult> Success([FromQuery] int order_id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var sql = @"
            SELECT 
                prq.[Id] AS RequestId,
                prq.[PayerCountry],
                prq.[PayerAddress],
                prq.[Action] AS RequestAction,
                prq.[PayerZip],
                prq.[PayerIp],
                prq.[OrderCurrency],
                prq.[PayerFirstName],
                prq.[PayerCity],
                prq.[Auth],
                prq.[PayerLastName],
                prq.[PayerPhone],
                prq.[PayerEmail],
                prq.[ReqToken],
                prq.[RecurringInit],
                prq.[TermUrl3ds],
                prq.[CardExpYear],
                prq.[CardExpMonth],
                prq.[OrderId],
                prq.[OrderIdInSystem],
                prq.[CardCvv2],
                prq.[OrderDescription],
                prq.[CardNumber],
                prq.[Hash],
                prq.[OrderAmount],
                prq.[CreatedAt],

                -- prs.[Id] AS ResponseId,
                -- prs.[PaymentRequestId],
                -- prs.[Action] AS ResponseAction,
                -- prs.[Result],
                -- prs.[Status],
                prs.[TransId]
                --,
                -- prs.[TransDate],
                -- prs.[Amount],
                -- prs.[Currency],
                -- prs.[RedirectUrl],
                -- prs.[RedirectParams],
                -- prs.[RedirectMethod],
                -- prs.[ErrorCode],
                -- prs.[ErrorMessage],
                -- prs.[Errors],
                -- prs.[Body]

            FROM [dbo].[PaymentRequests] prq
            LEFT JOIN [dbo].[PaymentResponses] prs
            ON prq.[Id] = prs.[PaymentRequestId]
            WHERE prq.[OrderId] = @orderId
        ";
        PaymentStatusEnum paymentStatus = PaymentStatusEnum.Declined;
        var PaymentRequestResponseDto = await connection.QueryFirstOrDefaultAsync<PaymentRequestResponseDto>(sql, new { orderId = order_id });
        
        if (PaymentRequestResponseDto is not null)
        {
            paymentStatus = await _paymentService.GetPaymentStatusAsync(PaymentRequestResponseDto);
        }

        string statusBadgeColor = paymentStatus switch
        {
            PaymentStatusEnum.Settled => "#E8F5E9;color: #2E7D32",
            PaymentStatusEnum.Declined => "#FFEBEE;color: #C62828",
            PaymentStatusEnum.Pending => "#FFF8E1;color: #F57F17",
            PaymentStatusEnum.Redirect => "#E3F2FD;color: #1565C0",
            PaymentStatusEnum.ThreeDSecure => "#E8EAF6;color: #3949AB",
            PaymentStatusEnum.Prepare => "#E0F2F1;color: #00796B",
            PaymentStatusEnum.Reversal => "#F3E5F5;color: #7B1FA2",
            PaymentStatusEnum.Refund => "#FCE4EC;color: #C2185B",
            PaymentStatusEnum.Chargeback => "#EFEBE9;color: #5D4037",
            _ => "#ECEFF1;color: #546E7A" // Unknown or any other status
        };

        string statusText = paymentStatus.ToString();
        string pageTitle = paymentStatus == PaymentStatusEnum.Settled ? "Payment Completed" : $"Payment {statusText}";
        string successMessage = paymentStatus == PaymentStatusEnum.Settled
            ? "Your payment has been completed successfully. Thank you for your purchase!"
            : $"Your payment has a status of {statusText}. Please check your order details for more information.";

        string html = $@"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{pageTitle} - KiloMart</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }}

        body {{
            background-color: #f5f5f5;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}

        .success-container {{
            background: white;
            padding: 3rem;
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            text-align: center;
            max-width: 500px;
            width: 90%;
            animation: fadeIn 0.5s ease-in-out;
        }}

        .success-icon {{
            width: 80px;
            height: 80px;
            background-color: #4CAF50;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 1.5rem;
            animation: scaleIn 0.5s ease-out;
        }}

        .success-icon svg {{
            width: 40px;
            height: 40px;
            fill: white;
        }}

        h1 {{
            color: #2c3e50;
            margin-bottom: 1rem;
            font-size: 2rem;
        }}

        p {{
            color: #666;
            margin-bottom: 1rem;
            line-height: 1.6;
        }}

        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #4CAF50;
            color: white;
            text-decoration: none;
            border-radius: 6px;
            transition: background-color 0.3s ease;
            margin-top: 1rem;
        }}

        .button:hover {{
            background-color: #45a049;
        }}

        @keyframes fadeIn {{
            from {{
                opacity: 0;
                transform: translateY(20px);
            }}
            to {{
                opacity: 1;
                transform: translateY(0);
            }}
        }}

        @keyframes scaleIn {{
            from {{
                transform: scale(0);
            }}
            to {{
                transform: scale(1);
            }}
        }}

        .order-details {{
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 1px solid #eee;
        }}

        .order-details p {{
            margin-bottom: 0.5rem;
        }}

        .status-badge {{
            display: inline-block;
            padding: 6px 12px;
            background-color: {statusBadgeColor.Split(';')[0]};
            color: {statusBadgeColor.Split(';')[1]};
            border-radius: 20px;
            font-size: 0.9rem;
            margin: 1rem 0;
        }}
    </style>
</head>

<body>
    <div class=""success-container"">
        <div class=""success-icon"">
            <svg viewBox=""0 0 24 24"">
                <path d=""M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41L9 16.17z""/>
            </svg>
        </div>
        <h1>{pageTitle}</h1>
        <div class=""status-badge"">{statusText}</div>
        <p>{successMessage}</p>
        <div class=""order-details"">
            <p>To view your order details:</p>
            <p>1. Open the KiloMart app</p>
            <p>2. Go to ""My Orders"" section</p>
            <p>3. Find your order #{order_id} in the list</p>
        </div>
    </div>
</body>

</html>";

        return Content(html, "text/html");
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