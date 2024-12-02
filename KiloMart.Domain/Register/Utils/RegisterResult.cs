namespace KiloMart.Domain.Register.Utils;

public class RegisterResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int? UserId { get; set; }
    public int? PartyId { get; set; }
    public byte? Language { get; set; }
    public string? VerificationToken { get; set; }
}