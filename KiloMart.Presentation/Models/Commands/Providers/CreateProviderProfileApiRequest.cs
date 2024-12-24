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
            
        return (errors.Count == 0, errors);
    }
}
