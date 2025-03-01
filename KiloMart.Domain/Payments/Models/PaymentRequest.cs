namespace EdfaPayApi.Core.Models;

public class PaymentRequest
{
       public string MerchantPassword { get; set; }
    public string MerchantKey { get; set; }
    public string PayerCountry { get; set; } = "SA";
    public string PayerAddress { get; set; } = "adnanh@expresspay.sa";
    public string Action { get; set; } = "SALE";
    public string PayerZip { get; set; } = "123221";
    public string PayerIp { get; set; } = "176.44.76.222";
    public string OrderCurrency { get; set; } = "SAR";
    public string PayerFirstName { get; set; } = "Adnan";
    public string PayerCity { get; set; } = "Riyadh";
    public string Auth { get; set; } = "N";
    public string PayerLastName { get; set; } = "Hashmi";
    public string PayerPhone { get; set; } = "966565897862";
    public string PayerEmail { get; set; } = "adnanh@expresspay.sa";
    public string ReqToken { get; set; } = "N";
    public string RecurringInit { get; set; } = "N";
    public string TermUrl3ds { get; set; } = "http://kilomart-001-site1.ptempurl.com/callbacks/success";
    // public string ClientKey { get; set; }
    public string CardExpYear { get; set; }
    public string CardExpMonth { get; set; }
    public string OrderId { get; set; } 
    public long OrderIdInSystem { get; set; }
    public string CardCvv2 { get; set; }
    public string OrderDescription { get; set; }
    public string CardNumber { get; set; }
    public string Hash { get; set; }
    public decimal OrderAmount { get; set; }
} 


public class PaymentRequestMini
{
    public string MerchantPassword { get; set; }
    public string MerchantKey { get; set; }
    public string CardExpYear { get; set; }
    public string CardExpMonth { get; set; }
    public string OrderId { get; set; } 
    public string CardCvv2 { get; set; }
    public string OrderDescription { get; set; }
    public string CardNumber { get; set; }
    public decimal OrderAmount { get; set; }
}


public static class PaymentRequestExtensions
{
    public static PaymentRequest ToPaymentRequest(this PaymentRequestMini mini)
    {
        return new PaymentRequest
        {
            CardExpYear = mini.CardExpYear,
            OrderId = mini.OrderId,
            CardCvv2 = mini.CardCvv2,
            OrderDescription = mini.OrderDescription,
            CardNumber = mini.CardNumber,
            OrderAmount = mini.OrderAmount,
            CardExpMonth = mini.CardExpMonth,
            MerchantPassword = mini.MerchantPassword,
            MerchantKey = mini.MerchantKey
        };
    }

    public static PaymentRequestMini ToMini(this PaymentRequest request)
    {
        return new PaymentRequestMini
        {
            CardExpYear = request.CardExpYear,
            OrderId = request.OrderId,
            CardCvv2 = request.CardCvv2,
            OrderDescription = request.OrderDescription,
            CardNumber = request.CardNumber,
            OrderAmount = request.OrderAmount,
            CardExpMonth = request.CardExpMonth,
            MerchantPassword = request.MerchantPassword,
            MerchantKey = request.MerchantKey
        };
    }
}