namespace KiloMart.Presentation.Models.Commands.Deliveries;
public class CreateDeliveryProfileApiRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = string.Empty;
    public DateTime DrivingLicenseExpiredDate { get; set; }


    public string Number { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Year { get; set; } = null!;


    public IFormFile? VehiclePhotoFile { get; set; }
    public IFormFile? DrivingLicenseFile { get; set; }
    public IFormFile? VehicleLicenseFile { get; set; }
    public IFormFile? NationalIqamaIDFile { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Number))
            errors.Add("Vehicle number is required.");
        if (string.IsNullOrWhiteSpace(Model))
            errors.Add("Vehicle model is required.");
        if (string.IsNullOrWhiteSpace(Type))
            errors.Add("Vehicle type is required.");

        if (string.IsNullOrWhiteSpace(Year))
            errors.Add("Vehicle year is required.");
        if (string.IsNullOrEmpty(FirstName))
            errors.Add("First name is required");
        if (string.IsNullOrEmpty(SecondName))
            errors.Add("Second name is required");
        if (string.IsNullOrEmpty(NationalName))
            errors.Add("National name is required");
        if (string.IsNullOrEmpty(NationalId))
            errors.Add("National ID is required");
        if (string.IsNullOrEmpty(LicenseNumber))
            errors.Add("License number is required");
        if (LicenseExpiredDate == default)
            errors.Add("License expired date is required");
        if (string.IsNullOrEmpty(DrivingLicenseNumber))
            errors.Add("Driving license number is required");
        if (DrivingLicenseExpiredDate == default)
            errors.Add("Driving license expired date is required");
        if (string.IsNullOrEmpty(Email))
            errors.Add("Email is required");
        if (string.IsNullOrEmpty(Password))
            errors.Add("Password is required");
        return (errors.Count == 0, errors.ToArray());
    }

}
