namespace KiloMart.Domain.Customers.Profile.Models;


public class CustomerProfileDto
{
    public int Id { get; set; }
    public int Customer { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Customer <= 0)
            errors.Add("Customer is required");
        if (string.IsNullOrEmpty(FirstName))
            errors.Add("First name is required");
        if (string.IsNullOrEmpty(SecondName))
            errors.Add("Second name is required");
        if (string.IsNullOrEmpty(NationalName))
            errors.Add("National name is required");
        if (string.IsNullOrEmpty(NationalId))
            errors.Add("National ID is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
