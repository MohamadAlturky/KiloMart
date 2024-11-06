namespace KiloMart.Domain.Register.Provider.Models;

public class RegisterResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int? UserId { get; set; }
    public int? PartyId { get; set; }
}
