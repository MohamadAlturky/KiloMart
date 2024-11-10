namespace KiloMart.Domain.Locations.Add.Models;

public class CreateLocationRequest
{
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = string.Empty;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Name))
            errors.Add("Name is required");
        if(Longitude == default)
            errors.Add("Longitude is required");
        if(Latitude == default)
            errors.Add("Latitude is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
