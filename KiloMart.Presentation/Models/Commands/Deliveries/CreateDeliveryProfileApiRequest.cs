namespace KiloMart.Presentation.Models.Commands.Deliveries;
public class CreateDeliveryProfileApiRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public byte LanguageId { get; set; }

    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = string.Empty;
    public DateTime DrivingLicenseExpiredDate { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(FirstName))
            errors.Add("First name is required");
        if (string.IsNullOrEmpty(SecondName))
            errors.Add("Second name is required");
        if (string.IsNullOrEmpty(NationalName))
            errors.Add("National name is required");
        if (string.IsNullOrEmpty(NationalId))
            errors.Add("National ID is required");
        if (LanguageId == 0)
            errors.Add("Language ID is required");
        if (string.IsNullOrEmpty(LicenseNumber))
            errors.Add("License number is required");
        if (LicenseExpiredDate == default)
            errors.Add("License expired date is required");
        if (string.IsNullOrEmpty(DrivingLicenseNumber))
            errors.Add("Driving license number is required");
        if (DrivingLicenseExpiredDate == default)
            errors.Add("Driving license expired date is required");
        return (errors.Count == 0, errors.ToArray());
    }

}
