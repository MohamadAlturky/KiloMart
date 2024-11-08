namespace KiloMart.Domain.Locations.Models;

public class CreateLocationResponse
{
    public int Id { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Party { get; set; }
}
