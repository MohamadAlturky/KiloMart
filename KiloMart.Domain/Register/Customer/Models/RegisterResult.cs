namespace KiloMart.Domain.Register.Customer.Models;

public class RegisterResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int? UserId { get; set; }
    public int? PartyId { get; set; }
}
