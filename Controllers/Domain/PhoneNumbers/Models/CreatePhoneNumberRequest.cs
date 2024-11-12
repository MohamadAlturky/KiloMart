namespace KiloMart.Domain.PhoneNumbers.Models;

public class CreatePhoneNumberRequest
{
    public string Value { get; set; } = string.Empty;


    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Value))
            errors.Add("Value is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
