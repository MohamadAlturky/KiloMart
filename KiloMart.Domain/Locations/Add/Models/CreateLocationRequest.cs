namespace KiloMart.Domain.Locations.Models;

public class CreateLocationRequest
{
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Party { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Name))
            errors.Add("Name is required");
        if (Party <= 0)
            errors.Add("Party is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
