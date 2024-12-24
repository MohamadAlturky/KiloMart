namespace KiloMart.Presentation.Models.Commands.Customers;
public class CreateCustomerProfileApiRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

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
        if (string.IsNullOrEmpty(Email))
            errors.Add("Email is required");
        if (string.IsNullOrEmpty(Password))
            errors.Add("Password is required");
        return (errors.Count == 0, errors.ToArray());
    }


}
