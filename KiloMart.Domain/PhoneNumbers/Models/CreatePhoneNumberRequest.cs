namespace KiloMart.Domain.PhoneNumbers.Models;

public class CreatePhoneNumberRequest
{
    public string Value { get; set; } = string.Empty;

    public int Party { get; set; }


    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Value))
            errors.Add("Value is required");
        if (Party <= 0)
            errors.Add("Party is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
