namespace KiloMart.Domain.Cards.Models;

public class CardDto
{
    public int Id { get; set; }

    public string HolderName { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string SecurityCode { get; set; } = string.Empty;

    public DateTime ExpireDate { get; set; }

    public int Customer { get; set; }
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Customer <= 0)
            errors.Add("Customer is required");
        if (string.IsNullOrEmpty(HolderName))
            errors.Add("Holder name is required");
        if (string.IsNullOrEmpty(Number))
            errors.Add("Number is required");
        if (string.IsNullOrEmpty(SecurityCode))
            errors.Add("Security code is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
