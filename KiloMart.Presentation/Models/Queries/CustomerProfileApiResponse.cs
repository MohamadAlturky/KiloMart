namespace KiloMart.Api.Models;

public class CustomerProfileApiResponse
{
    public int Id { get; set; }
    public int Customer { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    //Localized properties
    public string? LocalizedFirstName { get; set; }
    public string? LocalizedSecondName { get; set; }
    public string? LocalizedNationalName { get; set; }
    public byte? Language { get; set; }
}
