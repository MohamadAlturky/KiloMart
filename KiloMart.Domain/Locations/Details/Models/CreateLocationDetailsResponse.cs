namespace KiloMart.Domain.Locations.Details.Models;

public class CreateLocationDetailsResponse
{
    public int Id  { get; set; }
    public string BuildingType { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string FloorNumber { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Location { get; set; }
}


