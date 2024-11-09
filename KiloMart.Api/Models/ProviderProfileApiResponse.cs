namespace KiloMart.Api.Models;

public class ProviderProfileApiResponse
{
    public int Id { get; set; }
    public int Provider { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string OwnerNationalId { get; set; } = string.Empty;
    public string NationalApprovalId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;

    // Localized fields
    public int Language { get; set; }
    public string FirstNameLocalized { get; set; } = string.Empty;
    public string SecondNameLocalized { get; set; } = string.Empty;
    public string CompanyNameLocalized { get; set; } = string.Empty;
    public string OwnerNameLocalized { get; set; } = string.Empty;
}
