using EdfaPayApi.Core.Models;

namespace EdfaPayApi.Core.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    string GenerateHash(string email, string cardNumber, string merchantPassword);
} 