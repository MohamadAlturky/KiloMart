using KiloMart.Domain.Languages.Models;

public class CreateCustomerProfileApiRequest
{

    public int Id { get; set; }
    public int Customer { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string NationalName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public Language LanguageId { get; set; }

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
        if (LanguageId == 0)
        {
            errors.Add("Language ID is required");
        }
        return (errors.Count == 0, errors.ToArray());
    }


}
