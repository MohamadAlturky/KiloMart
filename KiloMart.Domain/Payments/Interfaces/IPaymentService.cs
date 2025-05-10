using EdfaPayApi.Core.Models;
using Twilio.Rest.Preview.Wireless;

namespace EdfaPayApi.Core.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    string GenerateHash(string email, string cardNumber, string merchantPassword);
    Task<PaymentStatusEnum> GetPaymentStatusAsync(PaymentRequestResponseDto transactionData);
}

public enum PaymentStatusEnum
{
    Unknown,
    ThreeDSecure,
    Redirect,
    Pending,
    Prepare,
    Declined,
    Settled,
    Reversal,
    Refund,
    Chargeback
}


  public class PaymentRequestResponseDto
    {
// PaymentRequest fields
    public int RequestId { get; set; }
    public string PayerCountry { get; set; }
    public string PayerAddress { get; set; }
    public string RequestAction { get; set; }
    public string PayerZip { get; set; }
    public string PayerIp { get; set; }
    public string OrderCurrency { get; set; }
    public string PayerFirstName { get; set; }
    public string PayerCity { get; set; }
    public string Auth { get; set; }
    public string PayerLastName { get; set; }
    public string PayerPhone { get; set; }
    public string PayerEmail { get; set; }
    public string ReqToken { get; set; }
    public string RecurringInit { get; set; }
    public string TermUrl3ds { get; set; }
    public int CardExpYear { get; set; }
    public int CardExpMonth { get; set; }
    public string OrderId { get; set; }
    public string OrderIdInSystem { get; set; }
    public string CardCvv2 { get; set; }
    public string OrderDescription { get; set; }
    public string CardNumber { get; set; }
    public string Hash { get; set; }
    public decimal OrderAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    // PaymentResponse fields
    // public int ResponseId { get; set; }
    // public int PaymentRequestId { get; set; }
    // public string ResponseAction { get; set; }
    // public string Result { get; set; }
    // public string Status { get; set; }
    public string TransId { get; set; }
    // public DateTime? TransDate { get; set; }
    // public decimal Amount { get; set; }
    // public string Currency { get; set; }
    // public string RedirectUrl { get; set; }
    // public string RedirectParams { get; set; }
    // public string RedirectMethod { get; set; }
    // public string ErrorCode { get; set; }
    // public string ErrorMessage { get; set; }
    // public string Errors { get; set; }
    // public string Body { get; set; }
    }
