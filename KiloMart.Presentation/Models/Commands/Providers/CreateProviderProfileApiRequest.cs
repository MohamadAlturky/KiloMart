public class CreateProviderProfileApiRequest
{
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalApprovalId { get; set; }
    public string CompanyName { get; set; }
    public string OwnerName { get; set; }
    public string OwnerNationalId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // public IFormFile? NationalIqamaIDFile { get; set; }
    public IFormFile? OwnershipDocumentFile { get; set; }
    public IFormFile? OwnerNationalApprovalFile { get; set; }


    // location

    public string LocationName { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }

    public string BuildingType { get; set; }
    public string BuildingNumber { get; set; }
    public string FloorNumber { get; set; }
    public string ApartmentNumber { get; set; }
    public string StreetNumber { get; set; }
    public string PhoneNumber { get; set; }

    public (bool success, List<string> errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(FirstName))
            errors.Add("First name is required");
        if (string.IsNullOrEmpty(SecondName))
            errors.Add("Second name is required");
        if (string.IsNullOrEmpty(NationalApprovalId))
            errors.Add("National approval ID is required");
        if (string.IsNullOrEmpty(CompanyName))
            errors.Add("Company name is required");
        if (string.IsNullOrEmpty(OwnerName))
            errors.Add("Owner name is required");
        if (string.IsNullOrEmpty(OwnerNationalId))
            errors.Add("Owner national ID is required");
        if (string.IsNullOrEmpty(Email))
            errors.Add("Email is required");
        if (string.IsNullOrEmpty(Password))
            errors.Add("Password is required");

        if (string.IsNullOrWhiteSpace(LocationName))
            errors.Add("Location name is required.");
        if (Longitude < -180 || Longitude > 180)
            errors.Add("Longitude must be between -180 and 180.");
        if (Latitude < -90 || Latitude > 90)
            errors.Add("Latitude must be between -90 and 90.");

        return (errors.Count == 0, errors);
    }
}
