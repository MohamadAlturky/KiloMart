namespace EdfaPayApi.Core.Models;

public class PaymentResponse
{
    public string action { get; set; }
    public string result { get; set; }
    public string status { get; set; }
    public string order_id { get; set; }
    public string trans_id { get; set; }
    public string trans_date { get; set; }
    public string amount { get; set; }
    public string currency { get; set; }
    public string redirect_url { get; set; }
    public RedirectParams redirect_params { get; set; }
    public string redirect_method { get; set; }
    
    // Error fields
    public int? error_code { get; set; }
    public string error_message { get; set; }
    public List<PaymentError> errors { get; set; }
    
    public bool IsError => result?.ToUpper() == "ERROR";
}

public class RedirectParams
{
    public string body { get; set; }
}

public class PaymentError
{
    public int error_code { get; set; }
    public string error_message { get; set; }
} 
