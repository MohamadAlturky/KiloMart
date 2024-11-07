namespace KiloMart.Domain.PhoneNumbers.Models;

public class CreatePhoneNumberResponse
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Party { get; set; }
}