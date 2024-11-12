namespace KiloMart.Domain.Locations.Details.Models;


public class CreateLocationDetailsRequest
{
    public string BuildingType { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string FloorNumber { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Location { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(BuildingType))
            errors.Add("Building Type is required");

        if (string.IsNullOrWhiteSpace(BuildingNumber))
            errors.Add("Building Number is required");

        if (string.IsNullOrWhiteSpace(StreetNumber))
            errors.Add("Street Number is required");

        if (string.IsNullOrWhiteSpace(PhoneNumber))
            errors.Add("Phone Number is required");
        if (string.IsNullOrWhiteSpace(FloorNumber))
            errors.Add("floor number is required");
        if (Location <= 0)
            errors.Add("Invalid Location");

        return (errors.Count == 0, errors.ToArray());
    }


}


