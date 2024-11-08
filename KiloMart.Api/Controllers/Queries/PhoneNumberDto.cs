namespace KiloMart.Api.Controllers.Queries;

public class PhoneNumberDto
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int PartyId { get; set; }
}