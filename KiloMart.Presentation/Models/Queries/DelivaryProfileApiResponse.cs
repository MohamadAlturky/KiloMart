namespace KiloMart.Api.Models;

public class DelivaryProfileApiResponse
{
    public int Id { get; set; }
    public int Delivary { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = string.Empty;

    //localized fields
    public int Language { get; set; }
    public string FirstNameLocalized { get; set; } = string.Empty;
    public string SecondNameLocalized { get; set; } = string.Empty;
    public string NationalNameLocalized { get; set; } = string.Empty;
}